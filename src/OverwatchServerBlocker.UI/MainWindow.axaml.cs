using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using OverwatchServerBlocker.Core.ViewModels;
using OverwatchServerBlocker.UI.Services;
using Window = ShadUI.Window;

namespace OverwatchServerBlocker.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            TitleBarAnimationEnabled = false;
        }
#endif
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        string? version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);
        if (string.IsNullOrEmpty(version))
        {
            return;
        }

        StackPanel? panel = e.NameScope.Find<StackPanel>("AppTitlePanel");
        panel?.Children.Add(new TextBlock
        {
            Text = version,
            Classes = { "Muted" },
            VerticalAlignment = VerticalAlignment.Center
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not MainViewModel mainViewModel)
        {
            return;
        }

        if (mainViewModel.DialogManager is DialogManager dialogManager)
        {
            DialogHost.Manager = dialogManager.Manager;
        }

        if (mainViewModel.ToastManager is ToastManager toastManager)
        {
            ToastHost.Manager = toastManager.Manager;
        }
    }
}