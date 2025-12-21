using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OverwatchServerBlocker.Core.Interfaces;
using OverwatchServerBlocker.Core.ViewModels;

namespace OverwatchServerBlocker.Core.Services;

public partial class Navigation : ObservableObject
{
    [ObservableProperty]
    private INavigable _currentPage;

    private readonly ServersViewModel _serversViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    public Navigation(ServersViewModel serversViewModel, SettingsViewModel settingsViewModel)
    {
        _serversViewModel = serversViewModel;
        _settingsViewModel = settingsViewModel;

        CurrentPage = _serversViewModel;
    }

    [RelayCommand]
    public void Navigate(Pages page)
    {
        switch (page)
        {
            case Pages.Servers:
                CurrentPage = _serversViewModel;
                break;
            case Pages.Settings:
                CurrentPage = _settingsViewModel;
                break;
        }
    }
}

public enum Pages
{
    Servers,
    Settings
}