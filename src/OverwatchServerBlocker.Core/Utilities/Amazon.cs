using System.Text.Json;
using System.Text.Json.Nodes;
using Echoes;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Regions;

namespace OverwatchServerBlocker.Core.Utilities;

public class Amazon
{
    private const string PrefixesUrl = "https://ip-ranges.amazonaws.com/ip-ranges.json";

    public static async Task<List<AmazonPrefix>> GetPrefixesAsync(HttpClient client)
    {
        string json = await client.GetStringAsync(PrefixesUrl);

        JsonObject? jsonObject = JsonSerializer.Deserialize<JsonObject>(json);
        if (jsonObject is null)
        {
            throw new ArgumentNullException(nameof(json), "JsonObject is null");
        }
        if (!jsonObject.TryGetPropertyValue("prefixes", out JsonNode? ipv4))
        {
            throw new ArgumentNullException(nameof(json), "ipv4 prefixes is null");
        }
        if (!jsonObject.TryGetPropertyValue("ipv6_prefixes", out JsonNode? ipv6))
        {
            throw new ArgumentNullException(nameof(json), "ipv6 prefixes is null");
        }

        AmazonPrefix[]? ipv4Prefixes = ipv4.Deserialize<AmazonPrefix[]>();
        AmazonPrefix[]? ipv6Prefixes = ipv6.Deserialize<AmazonPrefix[]>();
        if (ipv4Prefixes is null || ipv6Prefixes is null)
        {
            throw new ArgumentNullException(nameof(json), "prefixes is null");
        }

        List<AmazonPrefix> prefixes = [];
        prefixes.AddRange(ipv4Prefixes.Where(x => IsServiceAllowed(x.AWSService)).DistinctBy(x => x.Ipv4));
        prefixes.AddRange(ipv6Prefixes.Where(x => IsServiceAllowed(x.AWSService)).DistinctBy(x => x.Ipv6));

        return prefixes;
    }

    public static IEnumerable<AmazonRegion> GetRegions(IEnumerable<AmazonPrefix> prefixes)
    {
        return prefixes.DistinctBy(x => x.Region).Select(x => new AmazonRegion(x.Region));
    }

    public static TranslationUnit GetContinent(string identifier)
    {
        if (identifier.Equals("ap-southeast-2", StringComparison.OrdinalIgnoreCase)
            || identifier.Equals("ap-southeast-4", StringComparison.OrdinalIgnoreCase)
            || identifier.Equals("ap-southeast-6", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.australia;
        }

        if (identifier.StartsWith("af-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.africa;
        }

        if (identifier.StartsWith("ap-", StringComparison.OrdinalIgnoreCase)
            || identifier.StartsWith("cn-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.asia;
        }

        if (identifier.StartsWith("eu-", StringComparison.OrdinalIgnoreCase)
            || identifier.StartsWith("eusc-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.europe;
        }

        if (identifier.StartsWith("me-", StringComparison.OrdinalIgnoreCase)
            || identifier.StartsWith("il-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.middle_east;
        }

        if (identifier.StartsWith("us-", StringComparison.OrdinalIgnoreCase)
            || identifier.StartsWith("ca-", StringComparison.OrdinalIgnoreCase)
            || identifier.StartsWith("mx-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.north_america;
        }

        if (identifier.StartsWith("sa-", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.Regions.south_america;
        }

        return Localization.Regions.unknown;
    }

    public static TranslationUnit GetZone(string identifier)
    {
        return identifier switch
        {
            "af-south-1" => Localization.Regions.zaf_capetown,

            "ap-east-1" => Localization.Regions.hkg,
            "ap-east-2" => Localization.Regions.twn_taipei,

            "ap-northeast-1" => Localization.Regions.jpn_tokyo,
            "ap-northeast-2" => Localization.Regions.kor_seoul,
            "ap-northeast-3" => Localization.Regions.jpn_osaka,

            "ap-south-1" => Localization.Regions.ind_mumbai,
            "ap-south-2" => Localization.Regions.ind_hyderabad,

            "ap-southeast-1" => Localization.Regions.sgp,
            "ap-southeast-2" => Localization.Regions.aus_sydney,
            "ap-southeast-3" => Localization.Regions.idn_jakarta,
            "ap-southeast-4" => Localization.Regions.aus_melbourne,
            "ap-southeast-5" => Localization.Regions.mys_kuala_lumpur,
            "ap-southeast-6" => Localization.Regions.nzl_auckland,
            "ap-southeast-7" => Localization.Regions.tha_bangkok,

            "ca-central-1" => Localization.Regions.can_montreal,
            "ca-west-1" => Localization.Regions.can_calgary,

            "cn-north-1" => Localization.Regions.chn_beijing,
            "cn-northwest-1" => Localization.Regions.chn_ningxia,

            "eu-central-1" => Localization.Regions.edu_frankfurt,
            "eu-central-2" => Localization.Regions.che_zurich,

            "eu-north-1" => Localization.Regions.swe_stockholm,

            "eu-south-1" => Localization.Regions.ita_milan,
            "eu-south-2" => Localization.Regions.esp_madrid,

            "eu-west-1" => Localization.Regions.irl_dublin,
            "eu-west-2" => Localization.Regions.gbr_london,
            "eu-west-3" => Localization.Regions.fra_paris,

            "eusc-de-east-1" => Localization.Regions.edu_frankfurt,

            "il-central-1" => Localization.Regions.isr_telaviv,
            "me-central-1" => Localization.Regions.are_dubai,
            "me-south-1" => Localization.Regions.bhr_manama,
            "me-west-1" => Localization.Regions.isr_telaviv,

            "mx-central-1" => Localization.Regions.mex_mexicocity,

            "sa-east-1" => Localization.Regions.bra_saopaulo,
            "sa-west-1" => Localization.Regions.chl_santiago,

            "us-east-1" => Localization.Regions.usa_virginia,
            "us-east-2" or "us-gov-east-1" => Localization.Regions.usa_ohio,
            "us-gov-west-1" => Localization.Regions.usa_washington,
            "us-west-1" => Localization.Regions.usa_california,
            "us-west-2" => Localization.Regions.usa_oregon,

            _ => Localization.Regions.unknown
        };
    }

    private static bool IsServiceAllowed(string service)
    {
        // return service switch
        // {
        //     "AMAZON_APPFLOW"
        //         or "AMAZON_CONNECT"
        //         or "API_GATEWAY"
        //         or "AURORA_DSQL"
        //         or "CHIME_MEETINGS"
        //         or "CHIME_VOICECONNECTOR"
        //         or "CLOUD9"
        //         or "CLOUDFRONT"
        //         or "CLOUDFRONT_ORIGIN_FACING"
        //         or "CODEBUILD"
        //         or "DYNAMODB"
        //         or "EBS"
        //         or "EC2_INSTANCE_CONNECT"
        //         or "IVS_LOW_LATENCY"
        //         or "IVS_REALTIME"
        //         or "KINESIS_VIDEO_STREAMS"
        //         or "MEDIA_PACKAGE_V2"
        //         or "ROUTE53"
        //         or "ROUTE53_HEALTHCHECKS"
        //         or "ROUTE53_HEALTHCHECKS_PUBLISHING"
        //         or "ROUTE53_RESOLVER"
        //         or "S3"
        //         or "WORKSPACES_GATEWAYS"
        //         => false,
        //     _ => true
        // };

        return service switch
        {
            "AMAZON" or "EC2" => true,
            _ => false,
        };
    }
}