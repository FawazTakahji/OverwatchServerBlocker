using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OverwatchServerBlocker.Core.ViewModels;

namespace OverwatchServerBlocker.UI.Dialogs.ViewModels;

public partial class GuideViewModel : ViewModelBase
{
    private readonly Func<Task> _copyIpRange;
    private readonly Func<Task> _copyPort;

    public GuideViewModel(Func<Task> copyIpRange, Func<Task> copyPort)
    {
        _copyIpRange = copyIpRange;
        _copyPort = copyPort;
    }

    [RelayCommand]
    private async Task CopyIpRange()
    {
        await _copyIpRange.Invoke();
    }

    [RelayCommand]
    private async Task CopyPort()
    {
        await _copyPort.Invoke();
    }
}