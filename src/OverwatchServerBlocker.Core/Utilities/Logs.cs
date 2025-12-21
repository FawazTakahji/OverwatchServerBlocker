using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OverwatchServerBlocker.Core.Utilities;

public static class Logs
{
    private static readonly Regex Regex = new(Constants.SafeTitle + @" - (\d{4}-\d{2}-\d{2})_\d{2}-\d{2}-\d{2}\.log$");

    public static void TryDeleteLogs()
    {
        ILogger? logger = Singletons.ServiceProvider?.GetService<ILoggerFactory>()?.CreateLogger(typeof(Logs));
        string[] files;

        try
        {
            files = Directory.GetFiles(Singletons.LogsPath);
        }
        catch (Exception e)
        {
            logger?.LogError(e, "An error occurred while getting the log files.");
            return;
        }

        DateTimeOffset weekAgo = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7));
        foreach (string file in files)
        {
            Match match = Regex.Match(file);
            if (!match.Success
                || !DateTimeOffset.TryParse(match.Groups[1].Value, out DateTimeOffset date)
                || date > weekAgo)
            {
                continue;
            }

            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                logger?.LogError(e, "An error occurred while deleting the log file.");
            }
        }
    }
}