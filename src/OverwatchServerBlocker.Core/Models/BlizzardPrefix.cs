using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Interfaces;

namespace OverwatchServerBlocker.Core.Models;

public class BlizzardPrefix : IPrefix
{
    public Service Service => Service.Blizzard;
    public string Identifier { get; }
    public string Network { get; }

    public BlizzardPrefix(string identifier, string network)
    {
        Identifier = identifier;
        Network = network;
    }
}