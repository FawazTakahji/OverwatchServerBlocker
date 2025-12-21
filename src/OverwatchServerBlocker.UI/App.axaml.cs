using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Echoes;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.Utilities;
using OverwatchServerBlocker.Core.ViewModels;

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

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

#if RELEASE
            if (!Environment.IsPrivilegedProcess)
            {
                try
                {
                    TranslationProvider.SetCulture(CultureInfo.InstalledUICulture);
                }
                catch {}
                desktop.MainWindow = new PrivilegeWindow();
                return;
            }
#endif

            Logs.TryDeleteLogs();
            desktop.MainWindow = new MainWindow
            {
                DataContext = Singletons.ServiceProvider?.GetRequiredService<MainViewModel>()
            };

            IAppManager? appManager = Singletons.ServiceProvider?.GetRequiredService<IAppManager>();
            if (appManager is not null)
            {
                desktop.MainWindow.Loaded += (_, _) => appManager.InvokeMainWindowLoaded(this);
            }

            SettingsManager? manager = Singletons.ServiceProvider?.GetRequiredService<SettingsManager>();
            if (manager is not null)
            {
                manager.CultureChanged += ManagerOnCultureChanged;
                manager.TryLoad();
            }

            Singletons.ServiceProvider?.GetRequiredService<UpdateChecker>().CheckStartup();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ManagerOnCultureChanged(object? sender, string e)
    {
        CultureInfo info = CultureInfo.GetCultureInfo(e);
        TranslationProvider.SetCulture(info);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null } desktop)
        {
            desktop.MainWindow.FlowDirection = info.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}