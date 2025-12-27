using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Echoes;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.ViewModels;

#if RELEASE
using OverwatchServerBlocker.Core.Utilities;
#else
using Avalonia.Controls;
#endif

namespace OverwatchServerBlocker.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            DesignTime.Locator.BuildProvider();
        }
#endif
        foreach (var plugin in BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray())
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }

#if RELEASE
        Logs.TryDeleteLogs();
#endif

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DesktopStartup(desktop);
        }

        SettingsManager? manager = Singletons.ServiceProvider?.GetRequiredService<SettingsManager>();
        if (manager is not null)
        {
            manager.CultureChanged += ManagerOnCultureChanged;
            manager.TryLoad();
        }

        Singletons.ServiceProvider?.GetRequiredService<UpdateChecker>().CheckStartup();

        base.OnFrameworkInitializationCompleted();
    }

    private void DesktopStartup(IClassicDesktopStyleApplicationLifetime lifetime)
    {
#if WINDOWS
        lifetime.MainWindow = new ShadMainWindow
        {
            DataContext = Singletons.ServiceProvider?.GetRequiredService<MainViewModel>()
        };
#else
        lifetime.MainWindow = new MainWindow
        {
            DataContext = Singletons.ServiceProvider?.GetRequiredService<MainViewModel>()
        };
#endif
    }

    private void ManagerOnCultureChanged(object? sender, string e)
    {
        CultureInfo info = CultureInfo.GetCultureInfo(e);
        TranslationProvider.SetCulture(info);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null } desktop)
        {
            desktop.MainWindow.FlowDirection = info.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime { MainView: not null } singleView)
        {
            singleView.MainView.FlowDirection = info.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}