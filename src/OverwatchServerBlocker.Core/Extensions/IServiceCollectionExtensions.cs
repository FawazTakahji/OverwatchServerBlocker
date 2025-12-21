using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.ViewModels;
using ZLogger;

namespace OverwatchServerBlocker.Core.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection collection)
    {
        collection.AddHttpClient();

        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<ServersViewModel>();
        collection.AddSingleton<SettingsViewModel>();

        collection.AddSingleton<Navigation>();
        collection.AddSingleton<SettingsManager>();
        collection.AddSingleton<UpdateChecker>();

        Singletons.TrySetTemporaryLogsPath();
        collection.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
#if DEBUG
            logging.AddZLoggerConsole();
#else
            logging.AddZLoggerFile(Path.Combine(Singletons.LogsPath, $"{Constants.SafeTitle} - {DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss}.log"),
                o => o.UseJsonFormatter());
#endif
        });
    }
}