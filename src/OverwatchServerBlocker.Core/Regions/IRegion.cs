using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.Core.Regions;

public interface IRegion
{
    public bool IsSelected { get; set; }
    public Service Service { get; }
    public string Identifier { get; }
    public string ServiceString { get; }
    public string Continent { get; }
    public string Zone { get; }
}