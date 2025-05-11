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
    /// A simple wrapper that implements <see cref="IObservableDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// WARNING This collection cannot perform <see cref="INotifyCollectionChanged"/> operation 
    /// in a way that does f.e. <see cref="SortedList{TKey, TValue}"/> so it cannot imitate it fully.
    /// <br/>Also, I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ObservableDictionaryWrapper<TKey, TValue> : NotifyCollectionChangedBase<KeyValuePair<TKey, TValue>>, IObservableDictionary<TKey, TValue>
    {
        /// <summary>
        /// Special collection that allows to raise events from outside.
        /// </summary>
        /// <remarks>
        /// This sweet object allows to use protected methods by external objects.
        /// <br/>This is a typical "Public Morozov" anti-pattern (search Pavlik Morozov story), 
        /// but as it is a private type for anyone but <see cref="ObservableDictionaryWrapper{TKey, TValue}"/> this is OK.
        /// <br/> I also don't want to comment the methods, just because they all contain only one line and their names say enough.
        /// </remarks>
        private class InternalObservableCollectionWrapper<TItem> : ObservableCollectionWrapper<TItem>
        {
            public InternalObservableCollectionWrapper(ICollection<TItem> collection) : base(collection)
            { }

            public void InternalCountRaisePropertyChanging() => RaisePropertyChanging(CountChangingArgs);

            public void InternalCountRaisePropertyChanged() => RaisePropertyChanged(CountChangedArgs);


            public void InternalRaiseAdding(TItem item)
            {
                TrackChanges(item);
                RaiseAdding(item);
            }

            public void InternalRaiseAdding(IEnumerable<TItem> items) 
            { 
                TrackChanges(items);
                RaiseAdding(items); 
            }


            public void InternalRaiseReplacing(TItem oldItem, TItem newItem, int index = -1) 
            {
                UntrackChanges(oldItem);
                TrackChanges(newItem);
                RaiseReplacing(oldItem, newItem, index); 
            }


            public void InternalRaiseRemoving(TItem item) 
            { 
                UntrackChanges(item);
                RaiseRemoving(item); 
            }

            public void InternalRaiseRemoving(IEnumerable<TItem> items) 
            { 
                UntrackChanges(items);
                RaiseRemoving(items); 
            }


            public void InternalUntracking() => UntrackChanges(this);

            public void InternalRaiseClearing() => RaiseClearing();
        }

        private IDictionary<TKey, TValue> _internalCollection;

        private InternalObservableCollectionWrapper<TKey> _keys;
        private InternalObservableCollectionWrapper<TValue> _values;

        /// <summary>
        /// Creates an instance of a <see cref="Dictionary{TKey, TValue}"/> as an internal collection.
        /// </summary>
        public ObservableDictionaryWrapper() : this(new Dictionary<TKey, TValue>())
        { }

        public ObservableDictionaryWrapper(IDictionary<TKey, TValue> internalCollection)
        {
            ArgumentsGuard.ThrowIfNull(internalCollection, nameof(internalCollection));

            _internalCollection = internalCollection;

            _keys = new InternalObservableCollectionWrapper<TKey>(_internalCollection.Keys);
            _values = new InternalObservableCollectionWrapper<TValue>(_internalCollection.Values);
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get => _internalCollection[key];
            set
            {
                ThrowIfReadOnly();

                if (!_internalCollection.ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    var oldItem = _internalCollection[key];

                    var index = key is string ? key.ToString() : null;

                    RaiseIndexerChanging(index);

                    _internalCollection[key] = value;

                    RaiseIndexerChanged(index);
                    RaiseReplacing(new KeyValuePair<TKey, TValue>(key, oldItem), new KeyValuePair<TKey, TValue>(key, value));

                    _values.InternalRaiseReplacing(oldItem, value);
                }
            }
        }

        /// <inheritdoc/>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;

        /// <inheritdoc/>
        IObservableCollection<TKey> IObservableDictionary<TKey, TValue>.Keys => _keys;

        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys;

        /// <inheritdoc/>
        IObservableEnumerable<TKey> IReadOnlyObservableDictionary<TKey, TValue>.Keys => _keys;

        /// <summary>
        /// The explicit <see cref="ReactiveCollection{TItem}"/> that represents keys of the dictionary.
        /// </summary>
        public ObservableCollectionWrapper<TKey> Keys => _keys;


        /// <inheritdoc/>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _values;

        /// <inheritdoc/>
        IObservableCollection<TValue> IObservableDictionary<TKey, TValue>.Values => _values;

        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _values;

        /// <inheritdoc/>
        IObservableEnumerable<TValue> IReadOnlyObservableDictionary<TKey, TValue>.Values => _values;

        /// <summary>
        /// The explicit <see cref="ReactiveCollection{TItem}"/> that represents values of the dictionary.
        /// </summary>
        public ObservableCollectionWrapper<TValue> Values => _values;

        /// <inheritdoc/>
        public bool IsReadOnly => _internalCollection.IsReadOnly;

        /// <inheritdoc/>
        public int Count => _internalCollection.Count;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            ThrowIfReadOnly();

            ArgumentsDebug.ThrowIfNull(key, nameof(key));

            if (_internalCollection.ContainsKey(key))
                throw new ArgumentException("The key is already presented.");

            RaiseCountChanging();

            _internalCollection.Add(key, value);

            RaiseCountChanged();

            InternalRaiseAdding(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            ThrowIfReadOnly();

            var itemsList = new List<KeyValuePair<TKey, TValue>>();

            RaiseCountChanging();

            foreach (var item in items)
            {
                if (!_internalCollection.ContainsKey(item.Key))
                {
                    _internalCollection.Add(item);

                    itemsList.Add(item);
                }
            }

            RaiseCountChanged();

            if (itemsList.Count > 0)
                InternalRaiseAdding(itemsList);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalCollection.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_internalCollection.TryGetValue(item.Key, out TValue value))
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            else
                return false;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _internalCollection.ContainsKey(key);
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            if (!items.Any())
                return;

            var itemsList = new List<KeyValuePair<TKey, TValue>>();

            RaiseCountChanging();

            foreach (var item in items)
                if (_internalCollection.Remove(item))
                    itemsList.Add(item);

            RaiseCountChanged();

            if (itemsList.Count > 0)
                InternalRaiseRemoving(itemsList);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return false;

            if (_internalCollection.TryGetValue(key, out TValue value))
            {
                RaiseCountChanging();

                _internalCollection.Remove(key);

                RaiseCountChanged();

                InternalRaiseRemoving(new KeyValuePair<TKey, TValue>(key, value));

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return false;

            if (_internalCollection.TryGetValue(item.Key, out TValue value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                RaiseCountChanging();

                _internalCollection.Remove(item.Key);

                RaiseCountChanged();

                InternalRaiseRemoving(item);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            _keys.InternalUntracking();
            _values.InternalUntracking();

            RaiseCountChanging();

            _internalCollection.Clear();

            RaiseCountChanged();

            InternalRaiseClearing();
        }


        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException(Resource.Exception_ReadOnlyCollection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseCountChanging()
        {
            _keys.InternalCountRaisePropertyChanging();
            _values.InternalCountRaisePropertyChanging();

            RaisePropertyChanging(CountChangingArgs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseCountChanged()
        {
            _keys.InternalCountRaisePropertyChanged();
            _values.InternalCountRaisePropertyChanged();

            RaisePropertyChanged(CountChangedArgs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseAdding(KeyValuePair<TKey, TValue> item)
        {
            _keys.InternalRaiseAdding(item.Key);
            _values.InternalRaiseAdding(item.Value);

            RaiseAdding(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseAdding(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            _keys.InternalRaiseAdding(items.Select(item => item.Key));
            _values.InternalRaiseAdding(items.Select(item => item.Value));

            RaiseAdding(items);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseReplacing(TKey key, TValue oldItem, TValue newItem)
        {
            _values.InternalRaiseReplacing(oldItem, newItem);

            RaiseReplacing(new KeyValuePair<TKey, TValue>(key, oldItem), new KeyValuePair<TKey, TValue>(key, newItem));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseRemoving(KeyValuePair<TKey, TValue> item)
        {
            _keys.InternalRaiseRemoving(item.Key);
            _values.InternalRaiseRemoving(item.Value);

            RaiseRemoving(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseRemoving(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            _keys.InternalRaiseRemoving(items.Select(item => item.Key));
            _values.InternalRaiseRemoving(items.Select(item => item.Value));

            RaiseRemoving(items);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRaiseClearing()
        {
            _keys.InternalRaiseClearing();
            _values.InternalRaiseClearing();

            RaiseClearing();
        }
    }
}
