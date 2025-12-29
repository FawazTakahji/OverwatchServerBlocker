using System;
using System.Threading.Tasks;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.UI.Services;

public class Clipboard : IClipboard
{
    public async Task SetTextAsync(string text)
    {
        if (App.TopLevel?.Clipboard is not {} clipboard)
        {
            throw new Exception("Failed to get the clipboard service.");
        }

        await clipboard.SetTextAsync(text);
    }
}