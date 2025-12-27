using OverwatchServerBlocker.Core.Models;

namespace OverwatchServerBlocker.Core.Services;

public interface IAppManager
{
    public event EventHandler MainViewLoaded;

    public void InvokeMainViewLoaded(object? sender);

    public bool IsMainViewLoaded { get; }

    public void SetTheme(Theme theme);
}