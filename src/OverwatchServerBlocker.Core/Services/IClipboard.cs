namespace OverwatchServerBlocker.Core.Services;

public interface IClipboard
{
    public Task SetTextAsync(string text);
}