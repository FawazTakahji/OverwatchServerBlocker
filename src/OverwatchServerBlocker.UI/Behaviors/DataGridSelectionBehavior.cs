// https://tyrrrz.me/blog/wpf-listbox-selecteditems-twoway-binding

// using System.Collections;
// using System.Collections.Specialized;
// using System.Linq;
// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Data;
// using Avalonia.Xaml.Interactivity;
// using OverwatchServerBlocker.Core.Extensions;
//
// namespace OverwatchServerBlocker.UI.Behaviors;
//
// public class DataGridSelectionBehavior<T> : Behavior<DataGrid>
// {
//     public static readonly StyledProperty<IList?> SelectedItemsProperty =
//         AvaloniaProperty.Register<DataGridSelectionBehavior<T>, IList?>(
//             nameof(SelectedItems),
//             defaultValue: null,
//             defaultBindingMode: BindingMode.TwoWay);
//
//     private bool _viewHandled;
//     private bool _modelHandled;
//
//     private INotifyCollectionChanged? _itemsSourceCollection;
//     private INotifyCollectionChanged? _selectedItemsCollection;
//
//     public IList? SelectedItems
//     {
//         get => GetValue(SelectedItemsProperty);
//         set => SetValue(SelectedItemsProperty, value);
//     }
//
//     static DataGridSelectionBehavior()
//     {
//         SelectedItemsProperty.Changed.AddClassHandler<DataGridSelectionBehavior<T>>((x, e) =>
//         {
//             x.OnSelectedItemsPropertyChanged(e);
//         });
//     }
//
//     protected override void OnAttached()
//     {
//         base.OnAttached();
//         if (AssociatedObject == null)
//         {
//             return;
//         }
//
//         AssociatedObject.SelectionChanged += OnDataGridSelectionChanged;
//         AssociatedObject.PropertyChanged += OnDataGridPropertyChanged;
//
//         if (AssociatedObject.ItemsSource != null)
//         {
//             UpdateItemsSourceSubscription(AssociatedObject.ItemsSource);
//         }
//     }
//
//     protected override void OnDetaching()
//     {
//         if (AssociatedObject != null)
//         {
//             AssociatedObject.SelectionChanged -= OnDataGridSelectionChanged;
//             AssociatedObject.PropertyChanged -= OnDataGridPropertyChanged;
//         }
//
//         UnsubscribeFromItemsSource();
//         UnsubscribeFromSelectedItems();
//
//         base.OnDetaching();
//     }
//
//     private void OnSelectedItemsPropertyChanged(AvaloniaPropertyChangedEventArgs args)
//     {
//         if (args.Sender is not DataGridSelectionBehavior<T> behavior)
//         {
//             return;
//         }
//
//         behavior.UnsubscribeFromSelectedItems();
//
//         if (args.NewValue is INotifyCollectionChanged incc)
//         {
//             behavior._selectedItemsCollection = incc;
//             incc.CollectionChanged += behavior.OnSelectedItemsCollectionChanged;
//         }
//
//         if (!behavior._modelHandled)
//         {
//             behavior._modelHandled = true;
//             behavior.SyncViewModelToView();
//             behavior._modelHandled = false;
//         }
//     }
//
//     private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
//     {
//         if (_modelHandled)
//         {
//             return;
//         }
//
//         _modelHandled = true;
//         SyncViewModelToView();
//         _modelHandled = false;
//     }
//
//     private void SyncViewModelToView()
//     {
//         if (AssociatedObject == null || SelectedItems == null)
//         {
//             return;
//         }
//
//         _viewHandled = true;
//         AssociatedObject.SelectedItems.Clear();
//
//         foreach (var item in SelectedItems)
//         {
//             AssociatedObject.SelectedItems.Add(item);
//         }
//         _viewHandled = false;
//     }
//
//     private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs args)
//     {
//         if (_viewHandled || AssociatedObject?.ItemsSource == null || SelectedItems == null)
//         {
//             return;
//         }
//
//         _modelHandled = true;
//
//         if (SelectedItems is FastObservableCollection<T> collection)
//         {
//             collection.RemoveAddRange(args.AddedItems.OfType<T>().Where(x => !SelectedItems.Contains(x)), args.RemovedItems.OfType<T>());
//         }
//         else
//         {
//             foreach (var item in args.RemovedItems)
//             {
//                 SelectedItems.Remove(item);
//             }
//
//             foreach (var item in args.AddedItems)
//             {
//                 if (!SelectedItems.Contains(item))
//                 {
//                     SelectedItems.Add(item);
//                 }
//             }
//         }
//
//         _modelHandled = false;
//     }
//
//     private void OnDataGridPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
//     {
//         if (e.Property == DataGrid.ItemsSourceProperty)
//         {
//             UpdateItemsSourceSubscription(e.NewValue as IEnumerable);
//         }
//     }
//
//     private void UpdateItemsSourceSubscription(IEnumerable? newSource)
//     {
//         UnsubscribeFromItemsSource();
//
//         if (newSource is INotifyCollectionChanged incc)
//         {
//             _itemsSourceCollection = incc;
//             incc.CollectionChanged += OnItemsSourceCollectionChanged;
//         }
//
//         SyncViewModelToView();
//     }
//
//     private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
//     {
//         if (_viewHandled)
//         {
//             return;
//         }
//
//         SyncViewModelToView();
//     }
//
//     private void UnsubscribeFromItemsSource()
//     {
//         if (_itemsSourceCollection == null)
//         {
//             return;
//         }
//
//         _itemsSourceCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
//         _itemsSourceCollection = null;
//     }
//
//     private void UnsubscribeFromSelectedItems()
//     {
//         if (_selectedItemsCollection == null)
//         {
//             return;
//         }
//
//         _selectedItemsCollection.CollectionChanged -= OnSelectedItemsCollectionChanged;
//         _selectedItemsCollection = null;
//     }
// }