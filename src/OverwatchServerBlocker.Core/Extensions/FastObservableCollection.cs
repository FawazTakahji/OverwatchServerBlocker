using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OverwatchServerBlocker.Core.Extensions;

public class FastObservableCollection<T> : ObservableCollection<T>
{
    private bool _suppressEvent;

    public void RemoveRange(IEnumerable<T> items)
    {
        _suppressEvent = true;
        foreach (T item in items)
        {
            Remove(item);
        }
        _suppressEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    public void AddRange(IEnumerable<T> items)
    {
        _suppressEvent = true;
        foreach (T item in items)
        {
            Add(item);
        }
        _suppressEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    public void RemoveAddRange(IEnumerable<T> addedItems, IEnumerable<T> removedItems)
    {
        _suppressEvent = true;

        foreach (T removedItem in removedItems)
        {
            Remove(removedItem);
        }

        foreach (T addedItem in addedItems)
        {
            Add(addedItem);
        }

        _suppressEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_suppressEvent)
        {
            return;
        }

        base.OnPropertyChanged(e);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_suppressEvent)
        {
            return;
        }

        base.OnCollectionChanged(e);
    }
}