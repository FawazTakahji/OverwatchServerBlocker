using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.UI;
using OverwatchServerBlocker.UI.Extensions;
using OverwatchServerBlocker.Windows.Services;

namespace OverwatchServerBlocker.Windows;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        Singletons.IsFirewallSupported = true;
        Singletons.IsSplitTunnelSupported = true;
        Singletons.UpdateOS = "windows.json";

        IServiceCollection collection = new ServiceCollection();
        collection.AddSingleton<IFirewallManager, FirewallManager>();
        collection.AddUIServices();
        Singletons.BuildServiceProvider(collection);

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
