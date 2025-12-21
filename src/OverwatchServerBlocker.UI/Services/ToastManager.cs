using System;
using System.Collections.Generic;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Services;
using ShadUI;
using ShadUIToastManager = ShadUI.ToastManager;

namespace OverwatchServerBlocker.UI.Services;

public class ToastManager : IToastManager
{
    public ShadUIToastManager Manager { get; }
    private readonly IAppManager _appManager;

    private readonly List<(ToastBuilder builder, ToastStyle style)> _toasts = [];

    public ToastManager(ShadUIToastManager manager, IAppManager appManager)
    {
        Manager = manager;
        _appManager = appManager;

        _appManager.MainWindowLoaded += OnMainWindowLoaded;
    }

    private void OnMainWindowLoaded(object? sender, EventArgs e)
    {
        foreach ((ToastBuilder builder, ToastStyle style) toast in _toasts)
        {
            Show(toast.builder, toast.style);
        }

        _toasts.Clear();
    }

    public void Show(string title, string? content = null, ToastAction? action = null, ToastStyle style = ToastStyle.None)
    {
        ToastBuilder builder = Manager.CreateToast(title);
        if (content is not null)
        {
            builder.WithContent(content);
        }
        if (action is not null)
        {
            builder.WithAction(action.Label, action.Action);
        }

        if (_appManager.IsMainWindowLoaded)
        {
            Show(builder, style);
        }
        else
        {
            _toasts.Add((builder, style));
        }
    }

    private static void Show(ToastBuilder builder, ToastStyle style)
    {
        switch (style)
        {
            case ToastStyle.Info:
                builder.ShowInfo();
                break;
            case ToastStyle.Success:
                builder.ShowSuccess();
                break;
            case ToastStyle.Warning:
                builder.ShowWarning();
                break;
            case ToastStyle.Error:
                builder.ShowError();
                break;
            case ToastStyle.None:
            default:
                builder.Show();
                break;
        }
    }
}