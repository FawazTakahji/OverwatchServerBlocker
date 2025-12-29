using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Interfaces;
using OverwatchServerBlocker.Core.Localization;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    public string Route => "Settings";
    public static bool IsSplitTunnelSupported { get; } = Singletons.IsSplitTunnelSupported;

    public readonly SettingsManager SettingsManager;
    private readonly IAppManager _appManager;
    private readonly IDialogManager _dialogManager;
    private readonly IToastManager _toastManager;
    private readonly IStoragePicker _storagePicker;
    private readonly ILogger<SettingsViewModel> _logger;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RestoreCommand), nameof(ApplyCommand))]
    private Theme _theme;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RestoreCommand), nameof(ApplyCommand))]
    private string _language;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RestoreCommand), nameof(ApplyCommand))]
    private string _gamePath;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RestoreCommand), nameof(ApplyCommand))]
    private bool _checkUpdatesOnStartup;

    // App will crash if property isn't IList
    public IReadOnlyList<Language> SupportedLanguages { get; } =
    [
        new("English", "en"),
        new("العربية", "ar")
    ];

    public SettingsViewModel(
        SettingsManager settingsManager,
        IAppManager appManager,
        IDialogManager dialogManager,
        IToastManager toastManager,
        IStoragePicker storagePicker,
        ILogger<SettingsViewModel> logger)
    {
        SettingsManager = settingsManager;
        _appManager = appManager;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _storagePicker = storagePicker;
        _logger = logger;

        Theme = settingsManager.Settings.Theme; ;
        Language = settingsManager.Settings.Language;
        GamePath = settingsManager.Settings.GamePath;
        CheckUpdatesOnStartup = settingsManager.Settings.CheckUpdatesOnStartup;

        SettingsManager.SettingsLoaded += (_, _) =>
        {
            Theme = SettingsManager.Settings.Theme;
            Language = SettingsManager.Settings.Language;
            GamePath = SettingsManager.Settings.GamePath;
            CheckUpdatesOnStartup = SettingsManager.Settings.CheckUpdatesOnStartup;
        };
    }

    public void OnNavigatingAway()
    {
        Theme = SettingsManager.Settings.Theme;
    }

    partial void OnThemeChanged(Theme value)
    {
        _appManager.SetTheme(value);
    }

    [RelayCommand]
    private void SwitchTheme(Theme theme)
    {
        Theme = theme;
    }

    [RelayCommand]
    private async Task SelectGamePath()
    {
        try
        {
            IReadOnlyList<string> files = await _storagePicker.OpenFilePickerAsync(new()
            {
                Title = Localization.Settings.select_game_executable.CurrentValue,
                AllowMultiple = false,
                SuggestedStartLocation = Constants.DefaultGameFolder,
                FileTypeFilter =
                [
                    new FilePickerFileType(Localization.Settings.executable.CurrentValue)
                    {
                        Patterns = ["Overwatch.exe"]
                    }
                ]
            });

            if (files.Count < 1)
            {
                _toastManager.Show(Localization.Settings.selection_cancelled.CurrentValue);
                return;
            }

            GamePath = files[0];
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Settings.selection_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Settings.selection_error.Default());
        }
    }

    [RelayCommand]
    private void ToggleCheckUpdatesOnStartup()
    {
        CheckUpdatesOnStartup = !CheckUpdatesOnStartup;
    }

    private void NotifyAll()
    {
        ApplyCommand.NotifyCanExecuteChanged();
        RestoreCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(IsSettingsChanged))]
    private void Apply()
    {
        SettingsManager.Settings.Theme = Theme;
        SettingsManager.Settings.Language = Language;
        SettingsManager.Settings.GamePath = GamePath;
        SettingsManager.Settings.CheckUpdatesOnStartup = CheckUpdatesOnStartup;

        NotifyAll();

        try
        {
            SettingsManager.Save();
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Settings.save_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Settings.save_error.Default());
        }
    }

    [RelayCommand(CanExecute = nameof(IsSettingsChanged))]
    private void Restore()
    {
        Theme = SettingsManager.Settings.Theme;
        Language = SettingsManager.Settings.Language;
        GamePath = SettingsManager.Settings.GamePath;
        CheckUpdatesOnStartup = SettingsManager.Settings.CheckUpdatesOnStartup;

        NotifyAll();
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        Theme = Theme.System;
        Language = "en";
        GamePath = Constants.DefaultGameExecutable;
        CheckUpdatesOnStartup = true;

        NotifyAll();
    }

    private bool IsSettingsChanged()
    {
        return Theme != SettingsManager.Settings.Theme
            || Language != SettingsManager.Settings.Language
            || GamePath != SettingsManager.Settings.GamePath
            || CheckUpdatesOnStartup != SettingsManager.Settings.CheckUpdatesOnStartup;
    }

    private bool CanReset()
    {
        return SettingsManager.Settings.Theme != Theme.System
            || SettingsManager.Settings.Language != "en"
            || SettingsManager.Settings.GamePath != Constants.DefaultGameExecutable
            || !SettingsManager.Settings.CheckUpdatesOnStartup;
    }
}