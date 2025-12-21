namespace OverwatchServerBlocker.Core.Models;

public class DialogCustomButton(string label, Action? action = null, object? @return = null)
{
    public string Label { get; set; } = label;
    public Action? Action { get; set; } = action;
    public object? Return { get; set; } = @return;
}