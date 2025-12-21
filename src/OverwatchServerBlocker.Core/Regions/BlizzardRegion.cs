using CommunityToolkit.Mvvm.ComponentModel;
using Echoes;
using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.Core.Regions;

public partial class BlizzardRegion : ObservableObject, IRegion
{
    public Service Service { get; }
    public string Identifier { get; }

    [ObservableProperty]
    private bool _isSelected;
    [ObservableProperty]
    private string _serviceString;
    [ObservableProperty]
    private string _continent;
    [ObservableProperty]
    private string _zone;

    public BlizzardRegion(string identifier, TranslationUnit continentUnit, TranslationUnit zoneUnit)
    {
        Service = Service.Blizzard;
        Identifier = identifier;

        ServiceString = Localization.Services.blizzard.CurrentValue;
        Localization.Services.blizzard.Value.Subscribe(value => ServiceString = value);

        Continent = continentUnit.CurrentValue;
        continentUnit.Value.Subscribe(value => Continent = value);

        Zone = zoneUnit.CurrentValue;
        zoneUnit.Value.Subscribe(value => Zone = value);
    }
}