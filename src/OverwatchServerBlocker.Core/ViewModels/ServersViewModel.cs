using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Echoes;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Extensions;
using OverwatchServerBlocker.Core.Interfaces;
using OverwatchServerBlocker.Core.Localization;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Regions;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.Utilities;
using Timer = System.Timers.Timer;

namespace OverwatchServerBlocker.Core.ViewModels;

public partial class ServersViewModel : ViewModelBase, INavigable
{
    public string Route => "Servers";
    public static bool IsFirewallSupported { get; } = Singletons.IsFirewallSupported;

    private readonly ILogger<ServersViewModel> _logger;
    private readonly SettingsManager _settingsManager;
    private readonly IClipboard _clipboard;
    private readonly IDialogManager _dialogManager;
    private readonly IToastManager _toastManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFirewallManager _firewallManager;

    [ObservableProperty]
    private bool _isLoadingRegions;
    [ObservableProperty]
    private List<IRegion> _regions = [];
    [ObservableProperty]
    private ObservableCollection<IRegion> _selectedRegions = [];
    [ObservableProperty]
    private bool? _isAllSelected = false;
    [ObservableProperty]
    private Service _selectedProviders = Service.Amazon | Service.Google | Service.Blizzard;
    [ObservableProperty]
    private string _filterText = string.Empty;

    private static readonly IEnumerable<IRegion> BlizzardRegions = Blizzard.GetRegions();
    private static readonly IEnumerable<IPrefix> BlizzardPrefixes = Blizzard.GetPrefixes();
    private IEnumerable<IPrefix> _prefixes = BlizzardPrefixes;

    public event EventHandler? RegionsChanged;
    public event EventHandler? FiltersChanged;
    private readonly Timer _textTimer;

    [ObservableProperty]
    private bool _isModifyingRule;

    public ServersViewModel(
        ILogger<ServersViewModel> logger,
        IClipboard clipboard,
        SettingsManager settingsManager,
        IDialogManager dialogManager,
        IToastManager toastManager,
        IHttpClientFactory httpClientFactory,
        IFirewallManager firewallManager)
    {
        _logger = logger;
        _clipboard = clipboard;
        _settingsManager = settingsManager;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _httpClientFactory = httpClientFactory;
        _firewallManager = firewallManager;

        _textTimer = new Timer(150)
        {
            AutoReset = false
        };
        _textTimer.Elapsed += TextTimerOnElapsed;

        _settingsManager.SettingsLoaded += SettingsManagerOnSettingsLoaded;
    }

    [RelayCommand]
    private async Task Refresh(Action? onRegionsLoad = null)
    {
        try
        {
            IsLoadingRegions = true;
            HttpClient client = _httpClientFactory.CreateClient();

            List<GooglePrefix> googlePrefixes = [];
            List<AmazonPrefix> amazonPrefixes = [];
            List<Task> tasks = [GetGooglePrefixes(client, googlePrefixes), GetAmazonPrefixes(client, amazonPrefixes)];
            await Task.WhenAll(tasks);
            if (tasks.All(x => x.Status != TaskStatus.RanToCompletion))
            {
                return;
            }

            _prefixes = BlizzardPrefixes
                .Concat(googlePrefixes)
                .Concat(amazonPrefixes);
            Regions = BlizzardRegions
                .Concat(Google.GetRegions(googlePrefixes))
                .Concat(Amazon.GetRegions(amazonPrefixes))
                .ToList();
            onRegionsLoad?.Invoke();

            SelectedRegions = new ObservableCollection<IRegion>(Regions.Where(x => SelectedRegions.Any(y => y.Identifier == x.Identifier)));
            foreach (IRegion region in SelectedRegions)
            {
                region.IsSelected = true;
            }
            SetIsAllSelected(Regions.Count, SelectedRegions.Count);
        }
        finally
        {
            IsLoadingRegions = false;
        }
    }

    [RelayCommand]
    private void ShowManualSetup()
    {
        if (!Regions.Any(x => x.IsSelected))
        {
            _toastManager.Show(Localization.Servers.no_regions_selected.CurrentValue, style: ToastStyle.Warning);
            return;
        }

        _dialogManager.ShowGuide(CopyRanges, CopyPorts);

        if (IsFirewallSupported)
        {
            SaveSelectedRegions();
        }
    }

    [RelayCommand]
    private async Task ApplyRule()
    {
        try
        {
            if (!Regions.Any(x => x.IsSelected))
            {
                _toastManager.Show(Localization.Servers.no_regions_selected.CurrentValue, style: ToastStyle.Warning);
                return;
            }
            if (!File.Exists(_settingsManager.Settings.GamePath))
            {
                _toastManager.Show(
                    Localization.Servers.executable_not_found.CurrentValue,
                    Localization.Servers.select_correct_location.CurrentValue,
                    style: ToastStyle.Error);
                return;
            }

            IsModifyingRule = true;
            HashSet<string> addresses = GetSelectedAddresses();

            IFirewallRule[] rules = _firewallManager.GetRules();
            IFirewallRule? existingRule = rules.FirstOrDefault(x => x.Name == Constants.RuleName);
            if (existingRule is not null)
            {
                await Task.Run(() =>
                {
                    existingRule.IsEnabled = true;
                    existingRule.ApplicationPath = _settingsManager.Settings.GamePath;
                    existingRule.Description = Constants.RuleDescription;
                    existingRule.Action = FirewallRuleAction.Block;
                    existingRule.Direction = FirewallRuleDirection.Outbound;
                    existingRule.Protocol = FirewallRuleProtocol.UDP;
                    existingRule.RemoteAddresses = addresses.ToArray();
                    existingRule.RemotePorts = Constants.RulePorts;
                });

                _toastManager.Show(Localization.Servers.rule_applied.CurrentValue,
                    Localization.Servers.relaunch_game.CurrentValue,
                    style: ToastStyle.Success);
                return;
            }

            FirewallRule rule = new(Constants.RuleName, FirewallRuleAction.Block, FirewallRuleDirection.Outbound)
            {
                ApplicationPath = _settingsManager.Settings.GamePath,
                Description = Constants.RuleDescription,
                Protocol = FirewallRuleProtocol.UDP,
                RemoteAddresses = addresses.ToArray(),
                RemotePorts = Constants.RulePorts
            };
            _firewallManager.AddRule(rule);
            _toastManager.Show(Localization.Servers.rule_applied.CurrentValue,
                Localization.Servers.relaunch_game.CurrentValue,
                style: ToastStyle.Success);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.apply_rule_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.apply_rule_error.Default());
        }
        finally
        {
            SaveSelectedRegions();
            IsModifyingRule = false;
        }
    }

    [RelayCommand]
    private async Task RemoveRule()
    {
        try
        {
            IsModifyingRule = true;
            await Task.Run(() =>
            {
                IEnumerable<IFirewallRule> rules = _firewallManager.GetRules().Where(x => x.Name == Constants.RuleName);
                foreach (IFirewallRule rule in rules)
                {
                    _firewallManager.RemoveRule(rule);
                }
            });

            _toastManager.Show(Localization.Servers.rule_removed.CurrentValue, style: ToastStyle.Success);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.remove_rule_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.remove_rule_error.Default());
        }
        finally
        {
            IsModifyingRule = false;
        }
    }

    private async Task CopyRanges()
    {
        string addresses = string.Join(Environment.NewLine, GetSelectedAddresses());

        try
        {
            await _clipboard.SetTextAsync(addresses);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.copy_ranges_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.copy_ranges_error.Default());
            return;
        }

        _toastManager.Show(Localization.Servers.copied_ranges.CurrentValue, style: ToastStyle.Success);
    }

    private async Task CopyPorts()
    {
        string ports = string.Join(Environment.NewLine, Constants.RulePorts.Split(','));
        try
        {
            await _clipboard.SetTextAsync(ports);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.copy_ports_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.copy_ports_error.Default());
            return;
        }

        _toastManager.Show(Localization.Servers.copied_ports.CurrentValue, style: ToastStyle.Success);
    }

    private HashSet<string> GetSelectedAddresses()
    {
        HashSet<string> addresses = [];
        foreach (IRegion region in Regions.Where(r => r.IsSelected))
        {
            foreach (IPrefix prefix in _prefixes)
            {
                if (prefix.Service == region.Service
                    && prefix.Identifier == region.Identifier
                    && prefix.Network != null)
                {
                    addresses.Add(prefix.Network);
                }
            }
        }

        return addresses;
    }

    [RelayCommand]
    private void SaveSelectedRegions(bool notify = false)
    {
        Dictionary<Service, HashSet<string>> selectedRegions = new();
        selectedRegions.Initialize();

        foreach (IRegion region in Regions)
        {
            if (!region.IsSelected)
            {
                continue;
            }

            selectedRegions[region.Service].Add(region.Identifier);
        }

        _settingsManager.Settings.SelectedRegions = selectedRegions;

        try
        {
            _settingsManager.Save();
            if (notify)
            {
                _toastManager.Show(Localization.Settings.saved.CurrentValue, style: ToastStyle.Success);
            }
        }
        catch (Exception e)
        {
            _toastManager.Show(Localization.Settings.save_error.CurrentValue);
            _logger.LogError(e, "{Message}", Localization.Settings.save_error.Default());
        }
    }

    public void SetRegionSelection(IRegion region, bool isSelected)
    {
        region.IsSelected = isSelected;

        if (isSelected && !SelectedRegions.Contains(region))
        {
            SelectedRegions.Add(region);
        }
        else if (!isSelected)
        {
            SelectedRegions.Remove(region);
        }

        SetIsAllSelected(Regions.Count, SelectedRegions.Count);
    }

    [RelayCommand]
    private void ToggleAll()
    {
        if (IsAllSelected is null || IsAllSelected.Value)
        {
            foreach (IRegion region in Regions)
            {
                region.IsSelected = false;
            }

            SelectedRegions.Clear();
            IsAllSelected = false;
        }
        else
        {
            foreach (IRegion region in Regions)
            {
                region.IsSelected = true;
                if (!SelectedRegions.Contains(region))
                {
                    SelectedRegions.Add(region);
                }
            }

            IsAllSelected = true;
        }
    }

    private void SetIsAllSelected(int regions, int selectedRegions)
    {
        if (regions == selectedRegions)
        {
            IsAllSelected = true;
        }
        else if (selectedRegions < 1)
        {
            IsAllSelected = false;
        }
        else
        {
            IsAllSelected = null;
        }
    }

    [RelayCommand]
    private void ToggleProvider(Service provider)
    {
        if (SelectedProviders.HasFlag(provider))
        {
            SelectedProviders &= ~provider;
        }
        else
        {
            SelectedProviders |= provider;
        }
    }

    [RelayCommand]
    private void SetFilterText(string text)
    {
        FilterText = text;
    }

    public bool Filter(object o)
    {
        if (o is not IRegion region)
        {
            return false;
        }

        if (!SelectedProviders.HasFlag(region.Service))
        {
            return false;
        }

        if (string.IsNullOrEmpty(FilterText))
        {
            return true;
        }

        if (region.Zone.Contains(FilterText, TranslationProvider.Culture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace))
        {
            return true;
        }

        if (region.Identifier.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task GetGooglePrefixes(HttpClient client, List<GooglePrefix> prefixes)
    {
        try
        {
            IEnumerable<GooglePrefix> googlePrefixes = await Google.GetPrefixesAsync(client);
            prefixes.AddRange(googlePrefixes);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.load_google_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.load_google_error.Default());
        }
    }

    private async Task GetAmazonPrefixes(HttpClient client, List<AmazonPrefix> prefixes)
    {
        try
        {
            IEnumerable<AmazonPrefix> amazonPrefixes = await Amazon.GetPrefixesAsync(client);
            prefixes.AddRange(amazonPrefixes);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Servers.load_amazon_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Servers.load_amazon_error.Default());
        }
    }

    private void SettingsManagerOnSettingsLoaded(object? sender, EventArgs e)
    {
        Refresh(() =>
        {
            foreach (IRegion region in Regions)
            {
                if (SelectedRegions.Contains(region))
                {
                    continue;
                }

                if (_settingsManager.Settings.SelectedRegions[region.Service].Contains(region.Identifier))
                {
                    SelectedRegions.Add(region);
                }
            }
        }).SafeFireAndForget(x =>
        {
            _logger.LogError(x, "An error occurred while initially loading regions.");
        });
    }

    private void TextTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnFilterTextChanged(string value)
    {
        _textTimer.Stop();
        _textTimer.Start();
    }

    partial void OnSelectedProvidersChanged(Service value)
    {
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnRegionsChanged(List<IRegion> value)
    {
        RegionsChanged?.Invoke(this, EventArgs.Empty);
    }
}