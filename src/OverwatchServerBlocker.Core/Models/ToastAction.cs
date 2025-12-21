namespace OverwatchServerBlocker.Core.Models;

public class ToastAction(string label, Action action)
{
    public readonly string Label = label;
    public readonly Action Action = action;
}