﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace BirdsCommon
{
    public class SyncedObservableCollection<T> : ObservableCollection<T>, IEnumerable, ICollection, IList, IReadOnlyCollection<T>, IReadOnlyList<T>, IEnumerable<T>, ICollection<T>, IList<T>, ISyncedList, ISyncedList<T>
    {
        public ReaderWriterLock ReaderWriterLocker { get; }
        private static FieldInfo listField = typeof(Collection<T>).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly SyncedList<T> list;

        public SyncedObservableCollection()
        {
            ReaderWriterLocker = new ReaderWriterLock();
            list = new SyncedList<T>(ReaderWriterLocker);
            listField.SetValue(this, list);
        }

        public SyncedObservableCollection(IList<T> list)
        {
            if (list is SyncedList<T> _list)
            {
                ReaderWriterLocker = _list.ReaderWriterLocker;
                list = _list;
            }
            else
            {
                ReaderWriterLocker = new ReaderWriterLock();
                list = new SyncedList<T>(list, ReaderWriterLocker);
            }
            listField.SetValue(this, list);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            ReaderWriterLocker.AcquireReaderLock(Timeout.InfiniteTimeSpan);
            base.OnCollectionChanged(e);
            ReaderWriterLocker.ReleaseReaderLock();
        }

        public static void CollectionSynchronization(SyncedObservableCollection<T> syncCollection, TimeSpan timeout, Action accessMethod, bool writeAccess)
        {
            //if (!(collection is SyncedObservableCollection<T> syncCollection))
            //    throw new ArgumentException("Только для SynchronizatedObservableCollection<T>", nameof(collection));

            //if (!(context is TimeSpan timeout))
            //{
            //    if (context is int millisecondsTimeout && millisecondsTimeout >= 0)
            //    {
            //        timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            //    }
            //    else
            //    {
            //        timeout = Timeout.InfiniteTimeSpan;
            //    }
            //}

            if (writeAccess)
            {
                syncCollection.ReaderWriterLocker.AcquireWriterLock(timeout);
                accessMethod();
                syncCollection.ReaderWriterLocker.ReleaseWriterLock();
            }
            else
            {
                syncCollection.ReaderWriterLocker.AcquireReaderLock(timeout);
                accessMethod();
                syncCollection.ReaderWriterLocker.ReleaseReaderLock();
            }
        }

        //protected override void InsertItem(int index, T item)
        //{
        //    ReaderWriterLocker.AcquireWriterLock(Timeout.InfiniteTimeSpan);
        //    base.InsertItem(index, item);
        //    ReaderWriterLocker.ReleaseWriterLock();
        //}

        //protected override void ClearItems()
        //{
        //    ReaderWriterLocker.AcquireWriterLock(Timeout.InfiniteTimeSpan);
        //    base.ClearItems();
        //    ReaderWriterLocker.ReleaseWriterLock();
        //}

        //protected override void MoveItem(int oldIndex, int newIndex)
        //{
        //    ReaderWriterLocker.AcquireWriterLock(Timeout.InfiniteTimeSpan);
        //    base.MoveItem(oldIndex, newIndex);
        //    ReaderWriterLocker.ReleaseWriterLock();
        //}

        //protected override void RemoveItem(int index)
        //{
        //    ReaderWriterLocker.AcquireWriterLock(Timeout.InfiniteTimeSpan);
        //    base.RemoveItem(index);
        //    ReaderWriterLocker.ReleaseWriterLock();
        //}

        //protected override void SetItem(int index, T item)
        //{
        //    ReaderWriterLocker.AcquireWriterLock(Timeout.InfiniteTimeSpan);
        //    base.SetItem(index, item);
        //    ReaderWriterLocker.ReleaseWriterLock();
        //}

        //public new T this[int index]
        //{
        //    get
        //    {
        //        ReaderWriterLocker.AcquireReaderLock(Timeout.InfiniteTimeSpan);
        //        T t = base[index];
        //        ReaderWriterLocker.ReleaseWriterLock();
        //        return t;
        //    }

        //    set => base[index] = value;
        //}


    }
}