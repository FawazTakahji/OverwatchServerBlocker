namespace OverwatchServerBlocker.Core;

public static class Constants
{
    public const string SafeTitle = "OverwatchServerBlocker";
    public const string GithubUser = "FawazTakahji";
    public const string Repository = $"{GithubUser}/{SafeTitle}";
    public const string UpdatesURL = $"https://{GithubUser}.github.io/{SafeTitle}/updates/";
    public const string SettingsFile = "settings.json";
    public const string DefaultGameFolder = @"C:\Program Files (x86)\Overwatch\_retail_";
    public const string DefaultGameExecutable = $"{DefaultGameFolder}\\Overwatch.exe";

    public const string RuleName = $"{GithubUser}/{SafeTitle}";
    public const string RuleDescription = "Block Overwatch servers, Don't edit this rule yourself.";
    public const string RulePorts = "26503-36503,3724";
}