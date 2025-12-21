using System.Text.Json.Serialization;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Interfaces;

namespace OverwatchServerBlocker.Core.Models;

public class GooglePrefix : IPrefix
{
    [JsonPropertyName("scope")]
    public string Scope { get; }
    [JsonPropertyName("ipv4Prefix")]
    public string? Ipv4 { get; }
    [JsonPropertyName("ipv6Prefix")]
    public string? Ipv6 { get; }

    public GooglePrefix(string scope, string? ipv4, string? ipv6)
    {
        Scope = scope;
        Ipv4 = ipv4;
        Ipv6 = ipv6;
    }

    public Service Service => Service.Google;
    public string Identifier => Scope;
    public string? Network => Ipv4 ?? Ipv6;
}
