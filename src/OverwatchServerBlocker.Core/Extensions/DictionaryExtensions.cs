using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.Core.Extensions;

public static class DictionaryExtensions
{
    public static void Initialize(this Dictionary<Service, HashSet<string>> dictionary)
    {
        foreach (Service service in Enum.GetValues<Service>())
        {
            if (!dictionary.ContainsKey(service))
            {
                dictionary.Add(service, []);
            }
        }
    }
}