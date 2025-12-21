using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace OverwatchServerBlocker.UI.Controls;

public class HeaderContainer : ContentControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<HeaderContainer, string?>(nameof(Header));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public HeaderContainer()
    {
        HorizontalContentAlignment = HorizontalAlignment.Left;
    }
}