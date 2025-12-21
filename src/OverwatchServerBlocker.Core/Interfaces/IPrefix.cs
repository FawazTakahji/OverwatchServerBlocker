using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.Core.Interfaces;

public interface IPrefix
{
    public Service Service { get; }
    public string Identifier { get; }
    public string? Network { get; }
}