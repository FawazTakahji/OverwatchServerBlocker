using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OverwatchServerBlocker.Core.Enums;
using OverwatchServerBlocker.Core.Models;
using OverwatchServerBlocker.Core.Services;
using ShadUI;
using ShadUIDialogManager = ShadUI.DialogManager;

namespace OverwatchServerBlocker.UI.Services;

public class DialogManager : IDialogManager
{
    public ShadUIDialogManager Manager { get; }
    private readonly IAppManager _appManager;

    private readonly List<SimpleDialogBuilder> _dialogs = [];

    public DialogManager(ShadUIDialogManager manager, IAppManager appManager)
    {
        Manager = manager;
        _appManager = appManager;

        _appManager.MainWindowLoaded += OnMainWindowLoaded;
    }

    private void OnMainWindowLoaded(object? sender, EventArgs e)
    {
        foreach (SimpleDialogBuilder dialog in _dialogs)
        {
            dialog.Show();
        }

        _dialogs.Clear();
    }

    public void Show(string title, string message, DialogButtons buttons = DialogButtons.Ok)
    {
        SimpleDialogBuilder builder = Manager.CreateDialog(title, message).WithCancelButton("", null);
        switch (buttons)
        {
            case DialogButtons.Ok:
                builder.WithPrimaryButton(Core.Localization.Misc.ok.CurrentValue, null);
                break;
            case DialogButtons.YesNo:
                builder.WithPrimaryButton(Core.Localization.Misc.yes.CurrentValue, null);
                builder.WithSecondaryButton(Core.Localization.Misc.no.CurrentValue, null);
                break;
            case DialogButtons.OkCancel:
                builder.WithPrimaryButton(Core.Localization.Misc.ok.CurrentValue, null);
                builder.WithCancelButton(Core.Localization.Misc.cancel.CurrentValue, null);
                break;
            case DialogButtons.YesNoCancel:
                builder.WithPrimaryButton(Core.Localization.Misc.yes.CurrentValue, null);
                builder.WithSecondaryButton(Core.Localization.Misc.no.CurrentValue, null);
                builder.WithCancelButton(Core.Localization.Misc.cancel.CurrentValue, null);
                break;
        }

        if (_appManager.IsMainWindowLoaded)
        {
            builder.Show();
        }
        else
        {
            _dialogs.Add(builder);
        }
    }

    public void ShowCustom(
        string title,
        string message,
        DialogCustomButton primaryButton,
        DialogCustomButton? secondaryButton = null,
        DialogCustomButton? tertiaryButton = null)
    {
        SimpleDialogBuilder builder = Manager.CreateDialog(title, message).WithCancelButton("", null);
        builder.WithPrimaryButton(primaryButton.Label, primaryButton.Action);
        if (secondaryButton is not null)
        {
            builder.WithSecondaryButton(secondaryButton.Label, secondaryButton.Action);
        }
        if (tertiaryButton is not null)
        {
            builder.WithCancelButton(tertiaryButton.Label, tertiaryButton.Action ?? (() => {}));
        }

        if (_appManager.IsMainWindowLoaded)
        {
            builder.Show();
        }
        else
        {
            _dialogs.Add(builder);
        }
    }

    public async Task<DialogResult> ShowAsync(string title, string message, DialogButtons buttons = DialogButtons.Ok)
    {
        TaskCompletionSource<DialogResult> tcs = new();

        SimpleDialogBuilder builder = Manager.CreateDialog(title, message).WithCancelButton("", null);
        switch (buttons)
        {
            case DialogButtons.Ok:
                builder.WithPrimaryButton(Core.Localization.Misc.ok.CurrentValue, () => tcs.SetResult(DialogResult.Ok));
                break;
            case DialogButtons.YesNo:
                builder.WithPrimaryButton(Core.Localization.Misc.yes.CurrentValue, () => tcs.SetResult(DialogResult.Yes));
                builder.WithSecondaryButton(Core.Localization.Misc.no.CurrentValue, () => tcs.SetResult(DialogResult.No));
                break;
            case DialogButtons.OkCancel:
                builder.WithPrimaryButton(Core.Localization.Misc.ok.CurrentValue, () => tcs.SetResult(DialogResult.Ok));
                builder.WithCancelButton(Core.Localization.Misc.cancel.CurrentValue, () => tcs.SetResult(DialogResult.Cancel));
                break;
            case DialogButtons.YesNoCancel:
                builder.WithPrimaryButton(Core.Localization.Misc.yes.CurrentValue, () => tcs.SetResult(DialogResult.Yes));
                builder.WithSecondaryButton(Core.Localization.Misc.no.CurrentValue, () => tcs.SetResult(DialogResult.No));
                builder.WithCancelButton(Core.Localization.Misc.cancel.CurrentValue, () => tcs.SetResult(DialogResult.Cancel));
                break;
        }

        if (_appManager.IsMainWindowLoaded)
        {
            builder.Show();
        }
        else
        {
            _dialogs.Add(builder);
        }

        return await tcs.Task;
    }

    public async Task<object?> ShowCustomAsync(
        string title,
        string message,
        DialogCustomButton primaryButton,
        DialogCustomButton? secondaryButton = null,
        DialogCustomButton? tertiaryButton = null)
    {
        TaskCompletionSource<object?> tcs = new();

        SimpleDialogBuilder builder = Manager.CreateDialog(title, message).WithCancelButton("", null);
        builder.WithPrimaryButton(primaryButton.Label, () =>
        {
            primaryButton.Action?.Invoke();
            tcs.SetResult(primaryButton.Return);
        });
        if (secondaryButton is not null)
        {
            builder.WithSecondaryButton(secondaryButton.Label, () =>
            {
                secondaryButton.Action?.Invoke();
                tcs.SetResult(secondaryButton.Return);
            });
        }
        if (tertiaryButton is not null)
        {
            builder.WithCancelButton(tertiaryButton.Label, () =>
            {
                tertiaryButton.Action?.Invoke();
                tcs.SetResult(tertiaryButton.Return);
            });
        }

        if (_appManager.IsMainWindowLoaded)
        {
            builder.Show();
        }
        else
        {
            _dialogs.Add(builder);
        }

        return await tcs.Task;
    }
}