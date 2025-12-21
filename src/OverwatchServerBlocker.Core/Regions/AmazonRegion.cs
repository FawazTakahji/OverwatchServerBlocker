using CommunityToolkit.Mvvm.ComponentModel;
using Echoes;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Utilities;

namespace OverwatchServerBlocker.Core.Regions;

public partial class AmazonRegion : ObservableObject, IRegion
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

    public AmazonRegion(string identifier)
    {
        Service = Service.Amazon;
        Identifier = identifier;

        ServiceString = Localization.Services.amazon.CurrentValue;
        Localization.Services.amazon.Value.Subscribe(value => ServiceString = value);

        TranslationUnit continentUnit = Amazon.GetContinent(identifier);
        Continent = continentUnit.CurrentValue;
        continentUnit.Value.Subscribe(value => Continent = value);

        TranslationUnit zoneUnit = Amazon.GetZone(identifier);
        Zone = zoneUnit.CurrentValue;
        zoneUnit.Value.Subscribe(value => Zone = value);
    }
}