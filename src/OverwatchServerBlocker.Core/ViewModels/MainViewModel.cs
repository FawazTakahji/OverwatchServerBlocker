using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Localization;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.Utilities;

namespace OverwatchServerBlocker.Core.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger;
    public IDialogManager DialogManager { get; }
    public IToastManager ToastManager { get; }
    public Navigation Navigation { get; }
    public UpdateChecker UpdateChecker { get; }

    public MainViewModel(
        ILogger<MainViewModel> logger,
        IDialogManager dialogManager,
        IToastManager toastManager,
        Navigation navigation,
        UpdateChecker updateChecker)
    {
        _logger = logger;
        DialogManager = dialogManager;
        ToastManager = toastManager;
        Navigation = navigation;
        UpdateChecker = updateChecker;
    }

    [RelayCommand]
    private void OpenRepository()
    {
        TryOpenUrl($"https://github.com/{Constants.Repository}");
    }

    private void TryOpenUrl(string url)
    {
        try
        {
            Launcher.OpenUrl(url);
        }
        catch (Exception e)
        {
            DialogManager.Show(Localization.Misc.error_open_url.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Misc.error_open_url.Default());
        }
    }
}