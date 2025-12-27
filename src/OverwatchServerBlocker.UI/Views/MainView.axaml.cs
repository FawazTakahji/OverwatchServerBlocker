using Avalonia.Controls;
using OverwatchServerBlocker.Core.ViewModels;
using OverwatchServerBlocker.UI.Services;
using System;
using System.Reflection;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace OverwatchServerBlocker.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        string? version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);
        VersionLabel.Text = version;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            if (mainViewModel.DialogManager is DialogManager dialogManager)
            {
                DialogHost.Manager = dialogManager.Manager;
            }

            if (mainViewModel.ToastManager is ToastManager toastManager)
            {
                ToastHost.Manager = toastManager.Manager;
            }
        }

        base.OnDataContextChanged(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.AppManager.InvokeMainViewLoaded(this);
        }

        base.OnLoaded(e);
    }
}