using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.Stubs;
using OverwatchServerBlocker.UI;
using OverwatchServerBlocker.UI.Extensions;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddSingleton<IFirewallManager, FirewallManagerStub>();
        collection.AddUIServices();
        Singletons.BuildServiceProvider(collection);

        return AppBuilder.Configure<App>();
    }
}