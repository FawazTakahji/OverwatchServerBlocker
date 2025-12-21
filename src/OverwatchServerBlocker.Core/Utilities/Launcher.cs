using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OverwatchServerBlocker.Core.Utilities;

public static class Launcher
{
    public static void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "" : $"{url}",
                CreateNoWindow = true,
                UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            });
        }
    }
}