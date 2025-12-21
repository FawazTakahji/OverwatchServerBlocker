using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Models;

namespace OverwatchServerBlocker.Core.Services;

public interface IDialogManager
{
    public void Show(string title, string message, DialogButtons buttons = DialogButtons.Ok);

    public void ShowCustom(
        string title,
        string message,
        DialogCustomButton primaryButton,
        DialogCustomButton? secondaryButton = null,
        DialogCustomButton? tertiaryButton = null);

    public Task<DialogResult> ShowAsync(string title, string message, DialogButtons buttons = DialogButtons.Ok);

    public Task<object?> ShowCustomAsync(
        string title,
        string message,
        DialogCustomButton primaryButton,
        DialogCustomButton? secondaryButton = null,
        DialogCustomButton? tertiaryButton = null);
}