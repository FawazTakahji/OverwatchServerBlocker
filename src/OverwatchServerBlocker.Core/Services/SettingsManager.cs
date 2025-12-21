using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using OverwatchServerBlocker.Core.Extensions;
using OverwatchServerBlocker.Core.Localization;
using Settings = OverwatchServerBlocker.Core.Models.Settings;

namespace OverwatchServerBlocker.Core.Services;

public partial class SettingsManager : ObservableObject
{
    private readonly IDialogManager _dialogManager;
    private readonly ILogger<SettingsManager> _logger;

    private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.SettingsFile);
    public event EventHandler? SettingsLoaded;
    public event EventHandler<string>? CultureChanged;

    [ObservableProperty]
    private Settings _settings = new();

    public SettingsManager(IDialogManager dialogManager, ILogger<SettingsManager> logger)
    {
        _dialogManager = dialogManager;
        _logger = logger;
    }

    private void Load()
    {
        if (!File.Exists(_path))
        {
            Settings.SelectedRegions.Initialize();
            SettingsLoaded?.Invoke(this, EventArgs.Empty);
            return;
        }

        string json = File.ReadAllText(_path);
        Settings = JsonSerializer.Deserialize<Settings>(json) ?? throw new Exception("Deserialized settings object is null");
        Settings.SelectedRegions.Initialize();
        SettingsLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(Settings, Singletons.JsonSerializerOptions);
        File.WriteAllText(_path, json);
    }

    public void TryLoad()
    {
        try
        {
            Load();
        }
        catch (Exception e)
        {
            _dialogManager.Show(Localization.Settings.load_error.CurrentValue, e.Message);
            _logger.LogError(e, "{Message}", Localization.Settings.load_error.Default());
        }
        finally
        {
            CultureChanged?.Invoke(this, Settings.Language);
            Settings.PropertyChanged += OnPropertyChanged;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.Language))
        {
            CultureChanged?.Invoke(this, Settings.Language);
        }
    }
}