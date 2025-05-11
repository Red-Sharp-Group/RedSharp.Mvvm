using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
    /// WARNING This collection cannot perform <see cref="INotifyCollectionChanged"/> operation 
    /// in a way that does f.e. <see cref="SortedSet{T}"/> so it cannot imitate it fully.
    /// <br/>Also, I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ObservableSetWrapper<TItem> : NotifyCollectionChangedBase<TItem>, IObservableSet<TItem>
    {
        private ISet<TItem> _internalCollection;

        /// <summary>
        /// Creates an instance of a <see cref="HashSet{T}"/> as an internal collection.
        /// </summary>
        public ObservableSetWrapper() : this(new HashSet<TItem>())
        { }

        public ObservableSetWrapper(ISet<TItem> internalCollection)
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
        void ICollection<TItem>.Add(TItem item) => Add(item);

        /// <inheritdoc/>
        public void AddRange(IEnumerable<TItem> items)
        {
            ThrowIfReadOnly();

            var itemsList = new List<TItem>();

            foreach (var item in items)
                if (!_internalCollection.Contains(item))
                    itemsList.Add(item);

            if (itemsList.Count == 0)
                return;

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in itemsList)
                _internalCollection.Add(item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(itemsList);
            RaiseAdding(itemsList);
        }

        /// <inheritdoc/>
        public bool Add(TItem item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Contains(item))
                return false;

            RaisePropertyChanging(CountChangingArgs);

            _internalCollection.Add(item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(item);
            RaiseAdding(item);

            return true;
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

            if (_internalCollection.Contains(item))
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
        public void IntersectWith(IEnumerable<TItem> other) => _internalCollection.IntersectWith(other);

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<TItem> other) => _internalCollection.IsProperSubsetOf(other);

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<TItem> other) => _internalCollection.IsProperSupersetOf(other);

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<TItem> other) => _internalCollection.IsSubsetOf(other);

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<TItem> other) => _internalCollection.IsSupersetOf(other);

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<TItem> other) => _internalCollection.Overlaps(other);

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<TItem> other) => _internalCollection.SetEquals(other);

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other, nameof(other));

            var newItems = new List<TItem>();

            foreach (var item in other)
                if (!_internalCollection.Contains(item))
                    newItems.Add(item);

            if (newItems.Count == 0)
                return;

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in newItems)
                _internalCollection.Add(item);

            RaisePropertyChanged(CountChangedArgs);
            TrackChanges(newItems);
            RaiseAdding(newItems);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other, nameof(other));

            var oldItems = new List<TItem>();

            foreach (var item in other)
                if (_internalCollection.Contains(item))
                    oldItems.Add(item);

            if (oldItems.Count == 0)
                return;

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in oldItems)
                _internalCollection.Remove(item);

            RaisePropertyChanged(CountChangedArgs);
            UntrackChanges(oldItems);
            RaiseRemoving(oldItems);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other, nameof(other));

            var newItems = new List<TItem>();
            var oldItems = new List<TItem>();

            foreach (var item in other)
            {
                if (_internalCollection.Contains(item))
                    oldItems.Add(item);
                else
                    newItems.Add(item);
            }

            if (oldItems.Count == 0 && newItems.Count == 0)
                return;

            RaisePropertyChanging(CountChangingArgs);

            foreach (var item in newItems)
                _internalCollection.Add(item);

            foreach (var item in oldItems)
                _internalCollection.Remove(item);

            RaisePropertyChanged(CountChangedArgs);
            UntrackChanges(oldItems);
            RaiseRemoving(oldItems);
            TrackChanges(newItems);
            RaiseAdding(newItems);
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
