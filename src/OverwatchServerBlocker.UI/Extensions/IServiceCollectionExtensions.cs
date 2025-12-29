using Microsoft.Extensions.DependencyInjection;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.UI.Services;
using ShadUIDialogManager = ShadUI.DialogManager;
using ShadUIToastManager = ShadUI.ToastManager;

namespace OverwatchServerBlocker.UI.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddUIServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ShadUIDialogManager>();
        collection.AddSingleton<IDialogManager, DialogManager>();
        collection.AddSingleton<ShadUIToastManager>();
        collection.AddSingleton<IToastManager, ToastManager>();
        collection.AddSingleton<IAppManager, AppManager>();
        collection.AddSingleton<IStoragePicker, StoragePicker>();
        collection.AddSingleton<IClipboard, Clipboard>();
    }
}