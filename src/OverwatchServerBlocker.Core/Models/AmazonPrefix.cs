using System.Text.Json.Serialization;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Interfaces;

namespace OverwatchServerBlocker.Core.Models;

public class AmazonPrefix : IPrefix
{
    [JsonPropertyName("ip_prefix")]
    public string? Ipv4 { get; }
    [JsonPropertyName("ipv6_prefix")]
    public string? Ipv6 { get; }
    [JsonPropertyName("region")]
    public string Region { get; }
    [JsonPropertyName("service")]
    public string AWSService { get; }

    public AmazonPrefix(string? ipv4, string? ipv6, string region, string awsService)
    {
        Ipv4 = ipv4;
        Ipv6 = ipv6;
        Region = region;
        AWSService = awsService;
    }

    public Service Service => Service.Amazon;
    public string Identifier => Region;
    public string? Network => Ipv4 ?? Ipv6;
}