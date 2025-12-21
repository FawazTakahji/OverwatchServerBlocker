namespace OverwatchServerBlocker.Core.Services;

public interface IStoragePicker
{
    Task<IReadOnlyList<string>> OpenFilePickerAsync(FilePickerOpenOptions options);
}

public class FilePickerOptions
{
    public string? Title { get; set; }
    public string? SuggestedStartLocation { get; set; }
}

public class FilePickerOpenOptions : FilePickerOptions
{
    public bool AllowMultiple { get; set; }
    public IReadOnlyList<FilePickerFileType>? FileTypeFilter { get; set; }
}

public class FilePickerFileType(string name = "")
{
    public string Name { get; } = name;
    public IReadOnlyList<string>? Patterns { get; set; }
}