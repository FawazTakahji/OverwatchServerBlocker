using System.Text.Json.Serialization;

namespace OverwatchServerBlocker.Core.Models;

public class GithubRelease
{
    [JsonPropertyName("version"), JsonRequired]
    public string Version { get; init; }
    [JsonPropertyName("changes"), JsonRequired]
    public Dictionary<string, string> Changes { get; init; }
    [JsonPropertyName("release"), JsonRequired]
    public string Release { get; init; }

    public string GetLocalizedChange(string key)
    {
        if (Changes.Count < 1)
        {
            return string.Empty;
        }

        return Changes.TryGetValue(key, out string? value) ? value : Changes.First().Value;
    }
}