using System;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core.Extensions;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.UI.DesignTime.Services;
using OverwatchServerBlocker.UI.Extensions;

namespace OverwatchServerBlocker.UI.DesignTime;

public class Locator : MarkupExtension
{
    private static IServiceProvider? _provider;
    public required Type Type { get; init; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (_provider is not null)
        {
            return _provider.GetRequiredService(Type);
        }
        throw new NotImplementedException();
    }

    public static void BuildProvider()
    {
        IServiceCollection collection = new ServiceCollection();

        collection.AddCoreServices();
        collection.AddUIServices();

        collection.AddSingleton<IFirewallManager, DesignTimeFirewallManager>();

        _provider = collection.BuildServiceProvider();
    }
}