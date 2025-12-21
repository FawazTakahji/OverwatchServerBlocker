using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.Core.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyOrder(1)]
    private Theme _theme = Theme.System;

    [ObservableProperty]
    [property: JsonPropertyOrder(2)]
    private string _language = "en";

    [ObservableProperty]
    [property: JsonPropertyOrder(3)]
    private string _gamePath = Constants.DefaultGameExecutable;

    [ObservableProperty]
    [property: JsonPropertyOrder(4)]
    private bool _checkUpdatesOnStartup = true;

    [JsonPropertyOrder(5)]
    public Dictionary<Service, HashSet<string>> SelectedRegions { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Theme
{
    System,
    Light,
    Dark
}