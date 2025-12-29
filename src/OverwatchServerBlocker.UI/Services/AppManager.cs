using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.UI.Services;

public class AppManager : IAppManager
{
    public event EventHandler? MainViewLoaded;

    public void InvokeMainViewLoaded(object? sender)
    {
        MainViewLoaded?.Invoke(sender, EventArgs.Empty);
    }

    public bool IsMainViewLoaded => CheckView();

    public void SetTheme(Theme theme)
    {
        if (Application.Current is null)
        {
            return;
        }


        Application.Current.RequestedThemeVariant = theme switch
        {
            Theme.System => ThemeVariant.Default,
            Theme.Light => ThemeVariant.Light,
            Theme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    private static bool CheckView()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Content is Control { IsLoaded: true };
        }
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            return singleView.MainView is { IsLoaded: true };
        }

        return false;
    }
}