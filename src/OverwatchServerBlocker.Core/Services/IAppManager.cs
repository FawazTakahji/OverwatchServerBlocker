using OverwatchServerBlocker.Core.Models;

namespace OverwatchServerBlocker.Core.Services;

public interface IAppManager
{
    public event EventHandler MainWindowLoaded;

    public void InvokeMainWindowLoaded(object? sender);

    public bool IsMainWindowLoaded { get; }

    public void SetTheme(Theme theme);
}