using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        if (App.TopLevel?.StorageProvider is not {} storageProvider)
        {
            throw new Exception("Failed to get the storage provider service.");
        }

        AvaFilePickerOpenOptions avaOptions = new()
        {
            Title = options.Title,
            AllowMultiple = options.AllowMultiple,
            SuggestedStartLocation = options.SuggestedStartLocation is null ? null : await storageProvider.TryGetFolderFromPathAsync(options.SuggestedStartLocation),
            FileTypeFilter = options.FileTypeFilter is { } filter
                ? filter.Select(f => new FilePickerFileType(f.Name)
                {
                    Patterns = f.Patterns
                }).ToArray()
                : null
        };

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(avaOptions);
        return files.Select(f => f.Path.LocalPath).ToArray();
    }
}