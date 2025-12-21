using System.Globalization;

namespace OverwatchServerBlocker.Core.Extensions;

public static class StringExtensions
{
    public static bool Contains(this string str, string value, CultureInfo culture, CompareOptions options)
    {
        return culture.CompareInfo.IndexOf(str, value, options) > -1;
    }
}