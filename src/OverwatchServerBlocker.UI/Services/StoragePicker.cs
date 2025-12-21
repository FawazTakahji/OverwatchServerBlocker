using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using OverwatchServerBlocker.Core.Services;
using FilePickerOpenOptions = OverwatchServerBlocker.Core.Services.FilePickerOpenOptions;
using AvaFilePickerOpenOptions = Avalonia.Platform.Storage.FilePickerOpenOptions;
using FilePickerFileType = Avalonia.Platform.Storage.FilePickerFileType;

namespace OverwatchServerBlocker.UI.Services;

public class StoragePicker : IStoragePicker
{
    public async Task<IReadOnlyList<string>> OpenFilePickerAsync(FilePickerOpenOptions options)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new Exception("Missing desktop lifetime reference");
        }
        if (desktop.MainWindow is not MainWindow mainWindow)
        {
            throw new Exception("Missing top-level reference");
        }

        AvaFilePickerOpenOptions avaOptions = new()
        {
            Title = options.Title,
            AllowMultiple = options.AllowMultiple,
            SuggestedStartLocation = options.SuggestedStartLocation is null ? null : await mainWindow.StorageProvider.TryGetFolderFromPathAsync(options.SuggestedStartLocation),
            FileTypeFilter = options.FileTypeFilter is { } filter
                ? filter.Select(f => new FilePickerFileType(f.Name)
                {
                    Patterns = f.Patterns
                }).ToArray()
                : null
        };

        IReadOnlyList<IStorageFile> files = await mainWindow.StorageProvider.OpenFilePickerAsync(avaOptions);
        return files.Select(f => f.Path.LocalPath).ToArray();
    }
}