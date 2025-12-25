using System.Diagnostics;

namespace OverwatchServerBlocker.Core.Utilities;

public static class Launcher
{
    public static void OpenUrl(string url)
    {
#if WINDOWS
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            CreateNoWindow = true,
            UseShellExecute = true
        });
#else
        Process.Start("xdg-open", $"\"{url}\"");
#endif
    }
}