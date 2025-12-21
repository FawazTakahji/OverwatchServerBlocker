using CommunityToolkit.Mvvm.ComponentModel;
using Echoes;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Utilities;

namespace OverwatchServerBlocker.Core.Regions;

public partial class GoogleRegion: ObservableObject, IRegion
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

    public GoogleRegion(string identifier)
    {
        Service = Service.Google;
        Identifier = identifier;

        ServiceString = Localization.Services.google.CurrentValue;
        Localization.Services.google.Value.Subscribe(value => ServiceString = value);

        TranslationUnit continentUnit = Google.GetContinent(identifier);
        Continent = continentUnit.CurrentValue;
        continentUnit.Value.Subscribe(value => Continent = value);

        TranslationUnit zoneUnit = Google.GetZone(identifier);
        Zone = zoneUnit.CurrentValue;
        zoneUnit.Value.Subscribe(value => Zone = value);
    }
}