using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using IconPacks.Avalonia.FontAwesome;
using OverwatchServerBlocker.Core.Enums;

namespace OverwatchServerBlocker.UI.Converters;

public static class BooleanConverters
{
    public static readonly IValueConverter SidebarPadding =
        new FuncValueConverter<bool, Thickness>(value => value ? new Thickness(16, 8) : new Thickness(8));

    public static readonly IValueConverter SidebarTogglerHorizontalAlignment =
        new FuncValueConverter<bool, HorizontalAlignment>(value =>
            value ? HorizontalAlignment.Stretch : HorizontalAlignment.Center);

    public static readonly IValueConverter ObjectEquals = new ObjectEquals();
    public static readonly IValueConverter ObjectNotEquals = new ObjectNotEquals();

    public static readonly IValueConverter IsProviderEnabled =
        new FuncValueConverter<Service, Service, bool>((providers, provider) => providers.HasFlag(provider));

    public static readonly IValueConverter ToggleIcon =
        new FuncValueConverter<bool, PackIconFontAwesomeKind>(value => value ? PackIconFontAwesomeKind.CheckSolid : PackIconFontAwesomeKind.XmarkSolid);
}

public class ObjectEquals : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(true) == true ? parameter : BindingOperations.DoNothing;
    }
}

public class ObjectNotEquals : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !value?.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(true) == true ? BindingOperations.DoNothing : parameter;
    }
}