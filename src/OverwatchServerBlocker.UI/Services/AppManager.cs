using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.UI.Services;

public class AppManager : IAppManager
{
    public event EventHandler? MainWindowLoaded;

    public void InvokeMainWindowLoaded(object? sender)
    {
        MainWindowLoaded?.Invoke(sender, EventArgs.Empty);
    }

    public bool IsMainWindowLoaded => Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow.IsLoaded: true };

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
}