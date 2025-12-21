using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core.Extensions;

namespace OverwatchServerBlocker.Core;

public static class Singletons
{
    public static string UpdateOS = string.Empty;
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        WriteIndented = true
    };

    public static string LogsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    public static IServiceProvider? ServiceProvider;

    [MemberNotNull(nameof(ServiceProvider))]
    public static void BuildServiceProvider(IServiceCollection collection)
    {
        collection.AddCoreServices();
        ServiceProvider = collection.BuildServiceProvider();
    }

    public static void TrySetTemporaryLogsPath()
    {
        try
        {
            LogsPath = Path.GetTempPath();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while getting the temporary logs path: {e}");
        }
    }
}