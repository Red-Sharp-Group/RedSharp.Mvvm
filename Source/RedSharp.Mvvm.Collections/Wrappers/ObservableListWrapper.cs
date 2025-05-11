using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Collections.Abstracts;
using RedSharp.Mvvm.Collections.Interfaces;
using RedSharp.Mvvm.Collections.Properties;

namespace RedSharp.Mvvm.Collections.Wrappers
{
    /// <summary>
    /// A simple wrapper that implements <see cref="IObservableList{TItem}"/>.
    /// </summary>
    /// <remarks>
    /// I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ObservableListWrapper<TItem> : NotifyCollectionChangedBase<TItem>, IObservableList<TItem>
    {
        private IList<TItem> _internalCollection;

        /// <summary>
        /// Creates an instance of a <see cref="List{T}"/> as an internal collection.
        /// </summary>
        public ObservableListWrapper() : this(new List<TItem>())
        { }

        /// <summary>
        /// Creates an instance of a <see cref="List{T}"/> as an internal collection with given capacity.
        /// </summary>
        public ObservableListWrapper(int capacity) : this(new List<TItem>(capacity))
        { }

        public ObservableListWrapper(IList<TItem> internalCollection)
        {
            ArgumentsGuard.ThrowIfNull(internalCollection, nameof(internalCollection));

            _internalCollection = internalCollection;

            TrackChanges(_internalCollection);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Can be observed.
        /// </remarks>
        public int Count => _internalCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _internalCollection.IsReadOnly;

        /// <inheritdoc/>
        public TItem this[int index]
        {
            get => _internalCollection[index];
            set
            {
                ThrowIfReadOnly();

                var oldItem = _internalCollection[index];

                if (EqualityComparer<TItem>.Default.Equals(oldItem, value))
                    return;

                var key = index.ToString();

                RaiseIndexerChanging(key);

                _internalCollection[index] = value;

                RaiseIndexerChanged(key);
                UntrackChanges(oldItem);
                TrackChanges(value);
                RaiseReplacing(oldItem, value, index);
            }
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<TItem> items)
        {
            ThrowIfReadOnly();

            if (!items.Any())
                return;

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in items)
                _internalCollection.Add(item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(items);
            RaiseAdding(items);
        }

        /// <inheritdoc/>
        public void Add(TItem item)
        {
            ThrowIfReadOnly();

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.Add(item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(item);
            RaiseAdding(item);
        }

        /// <inheritdoc/>
        public void InsertRange(int index, IEnumerable<TItem> items)
        {
            ThrowIfReadOnly();

            NumericGuard.ThrowIfLessZero(index, nameof(index));
            NumericGuard.ThrowIfGreater(index, _internalCollection.Count, nameof(index));

            if (index == _internalCollection.Count)
            {
                AddRange(items);
            }
            else
            {
                if (!items.Any())
                    return;

                RaisePropertyChanging(CountChangingArgs);

                if (_internalCollection is List<TItem> list)
                {
                    list.InsertRange(index, items);
                }
                else
                {
                    var tempIndex = index;

                    foreach (var item in items)
                    {
                        _internalCollection.Insert(tempIndex, item);

                        tempIndex++;
                    }
                }

                RaisePropertyChanged(CountChangedArgs);
                TrackChanges(items);
                RaiseAdding(items, index);
            }
        }

        /// <inheritdoc/>
        public bool Contains(TItem item)
        {
            return _internalCollection.Contains(item);
        }

        /// <inheritdoc/>
        public int IndexOf(TItem item)
        {
            return _internalCollection.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, TItem item)
        {
            ThrowIfReadOnly();

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.Insert(index, item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(item);
            RaiseAdding(item, index);
        }

        /// <inheritdoc/>
        public void RemoveRange(int index, int count)
        {
            ThrowIfReadOnly();

            NumericGuard.ThrowIfLessZero(index, nameof(index));
            NumericGuard.ThrowIfLessZero(count, nameof(count));
            NumericGuard.ThrowIfGreaterOrEqual(index, _internalCollection.Count, nameof(index));
            NumericGuard.ThrowIfGreater(index + count, _internalCollection.Count, nameof(count));

            if (count == 0)
                return;

            if (index == 0 && count == _internalCollection.Count)
            {
                Clear();
            }
            else
            {
                var items = new List<TItem>();

                for (var i = index; i < index + count; i++)
                    items.Add(_internalCollection[i]);

                RaisePropertyChanging(CountChangingArgs);

                if (_internalCollection is List<TItem> list)
                {
                    list.RemoveRange(index, count);
                }
                else
                {
                    for (var i = 0; i < count; i++)
                        items.RemoveAt(index);
                }

                RaisePropertyChanged(CountChangedArgs);
                UntrackChanges(items);
                RaiseRemoving(items, index);
            }
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<TItem> items)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            if (!items.Any())
                return;

            var itemsList = new List<TItem>();

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in items)
                if (_internalCollection.Remove(item))
                    itemsList.Add(item);

            RaisePropertyChanged(CountChangedArgs);

            if (itemsList.Count > 0)
            {
                UntrackChanges(itemsList);
                RaiseAdding(itemsList);
            }
        }

        /// <inheritdoc/>
        public bool Remove(TItem item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return false;

            var index = _internalCollection.IndexOf(item);

            if (index == -1)
                return false;

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.RemoveAt(index);

            RaisePropertyChanged(CountChangedArgs);
            UntrackChanges(item);
            RaiseRemoving(item, index);

            return true;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            var item = _internalCollection[index];

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.RemoveAt(index);

            RaisePropertyChanged(CountChangedArgs);
            UntrackChanges(item);
            RaiseRemoving(item, index);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            UntrackChanges(_internalCollection);

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.Clear();

            RaisePropertyChanged(CountChangedArgs);
            RaiseClearing();
        }

        /// <inheritdoc/>
        public IEnumerator<TItem> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException(Resource.Exception_ReadOnlyCollection);
        }
    }
}
