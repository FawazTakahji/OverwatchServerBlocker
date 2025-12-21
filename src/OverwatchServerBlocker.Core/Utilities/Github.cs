using System.Text.Json;
using OverwatchServerBlocker.Core.Models;

namespace OverwatchServerBlocker.Core.Utilities;

public static class Github
{
    public static async Task<GithubRelease> GetLatestRelease(HttpClient client)
    {
        string response = await client.GetStringAsync(Constants.UpdatesURL + Singletons.UpdateOS);
        return JsonSerializer.Deserialize<GithubRelease>(response) ?? throw new Exception("Failed to deserialize the release JSON.");
    }
}