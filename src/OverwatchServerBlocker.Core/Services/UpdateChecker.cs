using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Localization;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Utilities;

#if RELEASE
using AsyncAwaitBestPractices;
#endif

namespace OverwatchServerBlocker.Core.Services;

public partial class UpdateChecker : ObservableObject
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SettingsManager _settingsManager;
    private readonly ILogger<UpdateChecker> _logger;
    private readonly IDialogManager _dialogManager;
    private readonly IToastManager _toastManager;

    [ObservableProperty]
    private bool _isCheckingUpdate;

    public UpdateChecker(
        IHttpClientFactory httpClientFactory,
        SettingsManager settingsManager,
        ILogger<UpdateChecker> logger,
        IDialogManager dialogManager,
        IToastManager toastManager)
    {
        _httpClientFactory = httpClientFactory;
        _settingsManager = settingsManager;
        _logger = logger;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    public void CheckStartup()
    {
#if RELEASE
        if (_settingsManager.Settings.CheckUpdatesOnStartup)
        {
            CheckForUpdates().SafeFireAndForget(x =>
            {
                _logger.LogError(x, "An error occurred while checking for updates on startup.");
            });
        }
#endif
    }

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        if (IsCheckingUpdate)
        {
            return;
        }

        try
        {
            IsCheckingUpdate = true;
            Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (currentVersion is null)
            {
                throw new Exception(Localization.Misc.error_app_version.CurrentValue);
            }

            HttpClient client = _httpClientFactory.CreateClient();
            GithubRelease release = await Github.GetLatestRelease(client);
            if (!Version.TryParse(release.Version, out Version? releaseVersion))
            {
                _toastManager.Show(Localization.Misc.error_parse_version.CurrentValue, style: ToastStyle.Error);
                return;
            }

            if (releaseVersion.CompareTo(currentVersion) < 1)
            {
                _toastManager.Show(Localization.Misc.no_updates.CurrentValue);
                return;
            }

            string changes = release.GetLocalizedChange(_settingsManager.Settings.Language);

            _dialogManager.ShowCustom(
                Localization.Misc.update_available.CurrentValue,
                $"{Localization.Misc.whats_new.CurrentValue} {release.Version}:{Environment.NewLine}{Environment.NewLine}{changes}",
                primaryButton: new DialogCustomButton(Localization.Misc.update.CurrentValue, () => TryOpenUrl(release.Release)),
                secondaryButton: new DialogCustomButton(Localization.Misc.close.CurrentValue));
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Misc.error_check_update.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Misc.error_check_update.Default());
        }
        finally
        {
            IsCheckingUpdate = false;
        }
    }

    private void TryOpenUrl(string url)
    {
        try
        {
            Launcher.OpenUrl(url);
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Misc.error_open_url.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Misc.error_open_url.Default());
        }
    }

}