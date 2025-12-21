using System.Text.Json;
using System.Text.Json.Nodes;
using Echoes;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Regions;

namespace OverwatchServerBlocker.Core.Utilities;

public class Google
{
    private const string PrefixesUrl = "https://www.gstatic.com/ipranges/cloud.json";

    public static async Task<IEnumerable<GooglePrefix>> GetPrefixesAsync(HttpClient client)
    {
        string json = await client.GetStringAsync(PrefixesUrl);
        return ParseJson(json);
    }

    private static IEnumerable<GooglePrefix> ParseJson(string json)
    {
        JsonObject? jsonObject = JsonSerializer.Deserialize<JsonObject>(json);
        if (jsonObject is null)
        {
            throw new ArgumentNullException(nameof(json), "JsonObject is null");
        }
        if (!jsonObject.TryGetPropertyValue("prefixes", out JsonNode? prefixes))
        {
            throw new ArgumentNullException(nameof(json), "prefixes is null");
        }
        return prefixes.Deserialize<IEnumerable<GooglePrefix>>() ?? throw new ArgumentNullException(nameof(json), "prefixes is null");
    }

    public static IEnumerable<GoogleRegion> GetRegions(IEnumerable<GooglePrefix> prefixes)
    {
        return prefixes.DistinctBy(x => x.Scope).Select(x => new GoogleRegion(x.Scope));
    }

    public static TranslationUnit GetContinent(string identifier)
    {
        if (identifier.StartsWith("africa-"))
        {
            return Localization.Regions.africa;
        }

        if (identifier.StartsWith("asia-"))
        {
            return Localization.Regions.asia;
        }

        if (identifier.StartsWith("australia-"))
        {
            return Localization.Regions.australia;
        }

        if (identifier.StartsWith("europe-"))
        {
            return Localization.Regions.europe;
        }

        if (identifier.StartsWith("me-"))
        {
            return Localization.Regions.middle_east;
        }

        if (identifier.StartsWith("northamerica-") || identifier.StartsWith("us-"))
        {
            return Localization.Regions.north_america;
        }

        if (identifier.StartsWith("southamerica-"))
        {
            return Localization.Regions.south_america;
        }

        return Localization.Regions.unknown;
    }

    public static TranslationUnit GetZone(string identifier)
    {
        return identifier switch
        {
            "africa-south1" => Localization.Regions.zaf_johannesburg,

            "asia-east1" => Localization.Regions.twn_changhua,
            "asia-east2" => Localization.Regions.hkg,

            "asia-northeast1" => Localization.Regions.jpn_tokyo,
            "asia-northeast2" => Localization.Regions.jpn_osaka,
            "asia-northeast3" => Localization.Regions.kor_seoul,

            "asia-south1" => Localization.Regions.ind_mumbai,
            "asia-south2" => Localization.Regions.ind_delhi,

            "asia-southeast1" => Localization.Regions.sgp_jurong,
            "asia-southeast2" => Localization.Regions.idn_jakarta,
            "asia-southeast3" => Localization.Regions.tha_bangkok,

            "australia-southeast1" => Localization.Regions.aus_sydney,
            "australia-southeast2" => Localization.Regions.aus_melbourne,

            "europe-central2" => Localization.Regions.pol_warsaw,

            "europe-north1" => Localization.Regions.fin_hamina,
            "europe-north2" => Localization.Regions.swe_stockholm,

            "europe-southwest1" => Localization.Regions.esp_madrid,

            "europe-west1" => Localization.Regions.bel_ghislain,
            "europe-west2" => Localization.Regions.gbr_london,
            "europe-west3" => Localization.Regions.edu_frankfurt,
            "europe-west4" => Localization.Regions.nld_eemshaven,
            "europe-west6" => Localization.Regions.che_zurich,
            "europe-west8" => Localization.Regions.ita_milan,
            "europe-west9" => Localization.Regions.fra_paris,
            "europe-west10" => Localization.Regions.edu_berlin,
            "europe-west12" => Localization.Regions.ita_turin,
            "europe-west15" => Localization.Regions.edu_heringsdorf,

            "me-central1" => Localization.Regions.qat_doha,
            "me-central2" => Localization.Regions.sau_dammam,
            "me-west1" => Localization.Regions.isr_telaviv,

            "northamerica-northeast1" => Localization.Regions.can_montreal,
            "northamerica-northeast2" => Localization.Regions.can_toronto,

            "northamerica-south1" => Localization.Regions.mex_queretaro,
            "southamerica-east1" => Localization.Regions.bra_saopaulo,
            "southamerica-west1" => Localization.Regions.chl_santiago,

            "us-central1" => Localization.Regions.usa_iowa,
            "us-central2" => Localization.Regions.usa_oklahoma,

            "us-east1" => Localization.Regions.usa_southcarolina,
            "us-east4" => Localization.Regions.usa_virginia,
            "us-east5" => Localization.Regions.usa_ohio,
            "us-east7" => Localization.Regions.usa_alabama,

            "us-south1" => Localization.Regions.usa_texas,

            "us-west1" => Localization.Regions.usa_oregon,
            "us-west2" => Localization.Regions.usa_california,
            "us-west3" => Localization.Regions.usa_utah,
            "us-west4" => Localization.Regions.usa_nevada,
            "us-west8" => Localization.Regions.usa_arizona,

            _ => Localization.Regions.unknown
        };
    }
}