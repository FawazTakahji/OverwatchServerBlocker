using System.Globalization;
using System.Reflection;
using Echoes;

namespace OverwatchServerBlocker.Core.Localization;

public static class Helper
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en");
    private static readonly Assembly Assembly = typeof(Helper).Assembly;

    public static string Default(this TranslationUnit unit)
    {
        return TranslationProvider.ReadTranslation(Assembly, unit.SourceFile, unit.Key, Culture);
    }
}