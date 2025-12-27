using Avalonia.Controls;
using Window = ShadUI.Window;

namespace OverwatchServerBlocker.UI;

public partial class ShadMainWindow : Window
{
    public ShadMainWindow()
    {
        InitializeComponent();

#if DEBUG
        if (Design.IsDesignMode)
        {
            TitleBarAnimationEnabled = false;
        }
#endif
    }
}