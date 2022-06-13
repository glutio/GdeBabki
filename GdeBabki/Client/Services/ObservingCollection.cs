using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GdeBabki.Client.Services
{
    public class ObservingList<TItem>: List<TItem>, IDisposable
    {
        object sender;
        INotifyCollectionChanged eventSource;
        Func<TItem, object> keySelector;

        // ObservingCollection(accountsApi.Accounts, accountsApi, e => e.Id) 
        public ObservingList(object sender, INotifyCollectionChanged eventSource, Func<TItem, object> keySelector)
        {
            this.sender = sender;
            this.keySelector = keySelector;
            this.eventSource = eventSource;
            eventSource.CollectionChanged += EventSource_CollectionChanged;
        }

        private void EventSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender == this.sender)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            Console.WriteLine(typeof(TItem));
                            Console.WriteLine(item);
                            Add((TItem)item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            var i = 0;
                            while (i < Count)
                            {
                                if (keySelector((TItem)item).Equals(keySelector(this[i])))
                                {
                                    RemoveAt(i);
                                    continue;
                                }
                                i++;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (var i = 0; i < Count; i++)
                        {
                            for (var old = 0; old < e.OldItems.Count; old++)
                            {
                                if (keySelector(this[i]).Equals(keySelector((TItem)e.OldItems[old])))
                                {
                                    RemoveAt(i);
                                    Insert(i, (TItem)e.NewItems[old]);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void Dispose()
        {
            eventSource.CollectionChanged -= EventSource_CollectionChanged;
        }
    }
}
