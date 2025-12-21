using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using OverwatchServerBlocker.Core.Regions;
using OverwatchServerBlocker.Core.ViewModels;

namespace OverwatchServerBlocker.UI.Views;

public partial class ServersView : UserControl
{
    private ServersViewModel? _viewModel;
    private DataGridCollectionView? _collectionView;
    private IRegion? _lastSelected;

    public ServersView()
    {
        InitializeComponent();

        ServersGrid.TemplateApplied += ServersGridOnTemplateApplied;
        ServersGrid.CellPointerPressed += ServersGridOnCellPointerPressed;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _viewModel?.PropertyChanged -= ViewModelOnRegionsChanged;
        _viewModel?.FiltersChanged -= ViewModelOnFiltersChanged;

        base.OnUnloaded(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        _viewModel?.PropertyChanged -= ViewModelOnRegionsChanged;
        _viewModel?.FiltersChanged -= ViewModelOnFiltersChanged;

        if (DataContext is ServersViewModel viewModel)
        {
            SetItemsSource(viewModel);

            _viewModel = viewModel;
            _viewModel.RegionsChanged += ViewModelOnRegionsChanged;
            _viewModel.FiltersChanged += ViewModelOnFiltersChanged;
        }

        base.OnDataContextChanged(e);
    }

    private void ServersGridOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (e.NameScope.Find<DataGridRowsPresenter>("PART_RowsPresenter") is { Parent: ScrollViewer scrollViewer })
        {
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }

    private void ServersGridOnCellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.Row.DataContext is not IRegion region)
        {
            return;
        }

        if (_lastSelected is null || !e.PointerPressedEventArgs.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            ToggleSelection(region);
            return;
        }
        int? lastIndex = _collectionView?.IndexOf(_lastSelected);
        if (_collectionView is null || lastIndex is null || lastIndex == -1)
        {
            ToggleSelection(region);
            return;
        }

        int start = Math.Min(lastIndex.Value, e.Row.Index);
        int end = Math.Max(lastIndex.Value, e.Row.Index);

        for (int i = start; i <= end; i++)
        {
            if (_collectionView[i] is IRegion r)
            {
                _viewModel?.SetRegionSelection(r, true);
            }
        }

        _lastSelected = region;
    }

    private void SetItemsSource(ServersViewModel viewModel)
    {
        _lastSelected = null;

        _collectionView = new DataGridCollectionView(viewModel.Regions.OrderBy(r => r.Continent).ThenBy(r => r.Zone))
        {
            GroupDescriptions = { new DataGridPathGroupDescription(nameof(IRegion.Continent)) },
            Filter = viewModel.Filter
        };
        ServersGrid.ItemsSource = _collectionView;
    }

    private void ToggleSelection(IRegion region)
    {
        _viewModel?.SetRegionSelection(region, !region.IsSelected);
        _lastSelected = region;
    }

    private void ViewModelOnFiltersChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            _collectionView?.Refresh();
        });
    }

    private void ViewModelOnRegionsChanged(object? sender, EventArgs e)
    {
        if (_viewModel is not null)
        {
            SetItemsSource(_viewModel);
        }
    }
}