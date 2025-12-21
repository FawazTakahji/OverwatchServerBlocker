using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using OverwatchServerBlocker.Core.Interfaces;
using OverwatchServerBlocker.Core.ViewModels;
using OverwatchServerBlocker.UI.Views;

namespace OverwatchServerBlocker.UI;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        UserControl? view = param switch
        {
            ServersViewModel serversViewModel => new ServersView { DataContext = serversViewModel },
            SettingsViewModel settingsViewModel => new SettingsView { DataContext = settingsViewModel },
            _ => null
        };

        if (view is null)
        {
            return new TextBlock { Text = "View Not Found" };
        }

        view.DataContext = param;
        if (param is INavigable)
        {
            view.Unloaded += ViewOnUnloaded;
        }

        return view;
    }

    private static void ViewOnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is UserControl view)
        {
            view.Unloaded -= ViewOnUnloaded;
            if (view.DataContext is INavigable navigable)
            {
                navigable.OnNavigatingAway();
            }
        }
    }

    public bool Match(object? data)
    {
        return data is INavigable;
    }
}