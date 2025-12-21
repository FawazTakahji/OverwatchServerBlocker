namespace OverwatchServerBlocker.Core.Models;

public class Language(string displayName, string culture)
{
    public string DisplayName { get; } = displayName;
    public string Culture { get; } = culture;
}