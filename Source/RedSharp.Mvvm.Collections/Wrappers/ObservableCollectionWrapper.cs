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
    /// A simple wrapper that implements <see cref="IObservableCollection{TItem}"/>.
    /// </summary>
    /// <remarks>
    /// I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ObservableCollectionWrapper<TItem> : NotifyCollectionChangedBase<TItem>, IObservableCollection<TItem>
    {
        private ICollection<TItem> _internalCollection;

        public ObservableCollectionWrapper(ICollection<TItem> internalCollection)
        {
            ArgumentsGuard.ThrowIfNull(internalCollection, nameof(internalCollection));

            _internalCollection = internalCollection;

            TrackChanges(_internalCollection);
        }

        /// <inheritdoc/>
        public bool IsReadOnly => _internalCollection.IsReadOnly;

        /// <inheritdoc/>
        /// <remarks>
        /// Can be observed.
        /// </remarks>
        public int Count => _internalCollection.Count;

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
        public bool Contains(TItem item)
        {
            return _internalCollection.Contains(item);
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

            if (!_internalCollection.Contains(item))
                return false;

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.Remove(item);

            RaisePropertyChanged(CountChangedArgs);
            UntrackChanges(item);
            RaiseRemoving(item);

            return true;
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
