using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.UI.Services;

public class Clipboard : IClipboard
{
    public async Task SetTextAsync(string text)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
        {
            throw new Exception("Failed to get the application lifetime.");
        }
        if (lifetime.MainWindow is null)
        {
            throw new Exception("Failed to get the main window.");
        }
        if (lifetime.MainWindow.Clipboard is null)
        {
            throw new Exception("Failed to get the clipboard.");
        }

        await lifetime.MainWindow.Clipboard.SetTextAsync(text);
    }
}