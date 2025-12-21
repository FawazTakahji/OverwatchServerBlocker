using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Models;

namespace OverwatchServerBlocker.Core.Services;

public interface IToastManager
{
    public void Show(string title, string? content = null, ToastAction? action = null, ToastStyle style = ToastStyle.None);
}