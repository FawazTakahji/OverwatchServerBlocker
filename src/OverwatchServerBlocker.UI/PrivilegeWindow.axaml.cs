using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Window = ShadUI.Window;

namespace OverwatchServerBlocker.UI;

public partial class PrivilegeWindow : Window
{
    public PrivilegeWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            TitleBarAnimationEnabled = false;
        }
#endif
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
        base.OnPointerPressed(e);
    }

    private void ExitButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}