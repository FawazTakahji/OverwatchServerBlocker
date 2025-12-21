using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Regions;

namespace OverwatchServerBlocker.Core.Utilities;

public static class Blizzard
{
    public static IEnumerable<BlizzardRegion> GetRegions()
    {
        return
        [
            new BlizzardRegion("icn1", Localization.Regions.asia, Localization.Regions.kor_incheon),
            new BlizzardRegion("tpe1", Localization.Regions.asia, Localization.Regions.twn_taipei),
            new BlizzardRegion("syd2", Localization.Regions.australia, Localization.Regions.aus_sydney),
            new BlizzardRegion("ams1", Localization.Regions.europe, Localization.Regions.nld_amsterdam),
            new BlizzardRegion("las1", Localization.Regions.north_america, Localization.Regions.usa_nevada),
            new BlizzardRegion("ord1", Localization.Regions.north_america, Localization.Regions.usa_illinois)
        ];
    }

    public static IEnumerable<BlizzardPrefix> GetPrefixes()
    {
        return
        [
            new BlizzardPrefix("icn1", "110.45.208.0/24"),
            new BlizzardPrefix("icn1", "117.52.6.0/24"),
            new BlizzardPrefix("icn1", "117.52.26.0/23"),
            new BlizzardPrefix("icn1", "117.52.28.0/23"),
            new BlizzardPrefix("icn1", "117.52.33.0/24"),
            new BlizzardPrefix("icn1", "117.52.34.0/23"),
            new BlizzardPrefix("icn1", "117.52.36.0/23"),
            new BlizzardPrefix("icn1", "121.254.137.0/24"),
            new BlizzardPrefix("icn1", "121.254.206.0/23"),
            new BlizzardPrefix("icn1", "121.254.218.0/24"),
            new BlizzardPrefix("icn1", "182.162.31.0/24"),
            new BlizzardPrefix("tpe1", "5.42.160.0/22"),
            new BlizzardPrefix("tpe1", "5.42.164.0/22"),
            new BlizzardPrefix("syd2", "158.115.196.0/23"),
            new BlizzardPrefix("ams1", "64.224.26.0/23"),
            new BlizzardPrefix("las1", "64.224.24.0/23"),
            new BlizzardPrefix("ord1", "64.224.0.0/21"),
            new BlizzardPrefix("ord1", "24.105.40.0/21")
        ];
    }
}