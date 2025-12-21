using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OverwatchServerBlocker.Core;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Core.ViewModels;

namespace OverwatchServerBlocker.UI.Views;

public partial class SettingsView : UserControl
{
    private SettingsManager? _settingsManager;

    public SettingsView()
    {
        InitializeComponent();
        SetWatermark();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _settingsManager?.CultureChanged -= SettingsManagerOnCultureChanged;
        base.OnUnloaded(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is SettingsViewModel viewModel)
        {
            _settingsManager = viewModel.SettingsManager;
            _settingsManager.CultureChanged += SettingsManagerOnCultureChanged;
        }
    }

    private void SettingsManagerOnCultureChanged(object? sender, string e)
    {
        SetWatermark();
    }

    private void SetWatermark()
    {
        GamePathTextBox.Watermark = $"{Core.Localization.Settings.example.CurrentValue}: {Constants.DefaultGameExecutable}";
    }
}