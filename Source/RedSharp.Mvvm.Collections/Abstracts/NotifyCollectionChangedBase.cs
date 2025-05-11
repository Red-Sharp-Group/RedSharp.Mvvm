using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Collections.Interfaces;
using RedSharp.Mvvm.Collections.Models;

namespace RedSharp.Mvvm.Collections.Abstracts
{
    public abstract class NotifyCollectionChangedBase<TItem> : INotifyCollectionChanged, INotifyItemChanging, INotifyItemChanged, INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// This item is better than the similarly implemented collection for <see cref="INotifyCollectionChanged"/> 
        /// as this one doesn't box the value.
        /// </summary>
        public class SingleItemList<TSingleItem> : IList<TSingleItem>
        {
            public class SingleItemEnumerator : IEnumerator<TSingleItem>
            {
                private TSingleItem _item;
                private bool _state;

                public SingleItemEnumerator(TSingleItem item)
                {
                    _item = item;
                }

                public TSingleItem Current => _item;

                object IEnumerator.Current => _item;

                public bool MoveNext()
                {
                    _state = !_state;

                    return _state;
                }

                public void Reset()
                {
                    _state = false;
                }

                public void Dispose()
                { }
            }

            private TSingleItem _item;

            public SingleItemList(TSingleItem item)
            {
                _item = item;
            }

            public TSingleItem this[int index]
            {
                get
                {
                    NumericGuard.ThrowIfNotEqual(index, 0, nameof(index));

                    return _item;
                }
                set => throw new NotSupportedException();
            }

            public int Count => 1;

            public bool IsReadOnly => true;


            public void Add(TSingleItem item) => throw new NotSupportedException();

            public void Insert(int index, TSingleItem item) => throw new NotSupportedException();

            public bool Remove(TSingleItem item) => throw new NotSupportedException();

            public void RemoveAt(int index) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();


            public bool Contains(TSingleItem item) => object.Equals(_item, item);

            public void CopyTo(TSingleItem[] array, int arrayIndex)
            {
                NumericGuard.ThrowIfGreaterOrEqual(arrayIndex, array.Length, nameof(arrayIndex));

                array[arrayIndex] = _item;
            }

            public int IndexOf(TSingleItem item)
            {
                if (Contains(item))
                    return 0;

                return -1;
            }

            public IEnumerator<TSingleItem> GetEnumerator() => new SingleItemEnumerator(_item);

            IEnumerator IEnumerable.GetEnumerator() => new SingleItemEnumerator(_item);
        }

        public static readonly PropertyChangingEventArgs CountChangingArgs = new PropertyChangingEventArgs(nameof(ICollection.Count));
        public static readonly PropertyChangedEventArgs CountChangedArgs = new PropertyChangedEventArgs(nameof(ICollection.Count));

        public static readonly PropertyChangingEventArgs IndexerChanging = new PropertyChangingEventArgs(IndexerProperty);
        public static readonly PropertyChangedEventArgs IndexerChanged = new PropertyChangedEventArgs(IndexerProperty);

        public const string IndexerProperty = "Item[]";
        public const string IndexerPropertyFormat = "Item[{0}]";

        private bool _itemNotifiesOnPropertyChanging;
        private bool _itemNotifiesOnPropertyChanged;


        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;


        /// <summary>
        /// Occurs on the beginning of property changing right before it is actually changed.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <inheritdoc/>
        public event ItemChangingEventHandler ItemChanging;

        /// <inheritdoc/>
        public event ItemChangedEventHandler ItemChanged;


        public NotifyCollectionChangedBase()
        {
            _itemNotifiesOnPropertyChanging = typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(TItem));
            _itemNotifiesOnPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TItem));
        }

        private void ReRaiseItemPropertyChanging(object sender, PropertyChangingEventArgs arguments)
        {
            RaiseItemChanging(sender, arguments.PropertyName);
        }

        private void ReRaiseItemPropertyChanged(object sender, PropertyChangedEventArgs arguments)
        {
            RaiseItemChanged(sender, arguments.PropertyName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IList ToInterfaceList(IEnumerable<TItem> items)
        {
            if (items is IList casted)
                return casted;
            else
                return items.ToList();
        }

        //====================================================================================================//
        //========================================= Items Changing ===========================================//

        protected bool ItemNotifiesOnPropertyChanging => _itemNotifiesOnPropertyChanging;

        protected bool ItemNotifiesOnPropertyChanged => _itemNotifiesOnPropertyChanged;


        protected void RaiseItemChanging(object item, string property)
        {
            if (ItemChanged == null)
                return;

            RaiseItemChanging(new ItemChangingEventArgs(item, property));
        }

        protected void RaiseItemChanging(ItemChangingEventArgs arguments)
        {
            if (ItemChanged == null)
                return;

            try
            {
                ItemChanging.Invoke(this, arguments);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        protected void RaiseItemChanged(object item, string property)
        {
            if (ItemChanged == null)
                return;

            RaiseItemChanged(new ItemChangedEventArgs(item, property));
        }

        protected void RaiseItemChanged(ItemChangedEventArgs arguments)
        {
            if (ItemChanged == null)
                return;

            try
            {
                ItemChanged.Invoke(this, arguments);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TrackChanges(TItem item)
        {
            if (item == null)
                return;

            if (_itemNotifiesOnPropertyChanging)
                ((INotifyPropertyChanging)item).PropertyChanging += ReRaiseItemPropertyChanging;

            if (_itemNotifiesOnPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += ReRaiseItemPropertyChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TrackChanges(IEnumerable<TItem> items)
        {
            if (_itemNotifiesOnPropertyChanging)
                foreach (var item in items)
                    if (item != null)
                        ((INotifyPropertyChanging)item).PropertyChanging += ReRaiseItemPropertyChanging;

            if (_itemNotifiesOnPropertyChanged)
                foreach (var item in items)
                    if (item != null)
                        ((INotifyPropertyChanged)item).PropertyChanged += ReRaiseItemPropertyChanged;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UntrackChanges(TItem item)
        {
            if (item == null)
                return;

            if (_itemNotifiesOnPropertyChanging)
                ((INotifyPropertyChanging)item).PropertyChanging -= ReRaiseItemPropertyChanging;

            if (_itemNotifiesOnPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged -= ReRaiseItemPropertyChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UntrackChanges(IEnumerable<TItem> items)
        {
            if (_itemNotifiesOnPropertyChanging)
                foreach(var item in items)
                    if (item != null)
                        ((INotifyPropertyChanging)item).PropertyChanging -= ReRaiseItemPropertyChanging;

            if (_itemNotifiesOnPropertyChanged)
                foreach (var item in items)
                    if (item != null)
                        ((INotifyPropertyChanged)item).PropertyChanged -= ReRaiseItemPropertyChanged;
        }

        //====================================================================================================//
        //======================================= Collection Changing ========================================//

        /// <summary>
        /// Invokes a <see cref="CollectionChanged"/>.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// <br/> This method is here for the possibility to re-throw the arguments, please use another methods for the direct invocation, they are optimized.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs arguments)
        {
            if (CollectionChanged == null)
                return;

            try
            {
                CollectionChanged.Invoke(this, arguments);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Has to be invoked on single item adding, possible with index (if it is an inserting).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseAdding(TItem item, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            var items = new SingleItemList<TItem>(item);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index));
        }

        /// <summary>
        /// Using for range adding.
        /// </summary>
        /// <remarks>
        /// Currently I don't know a collection that can add a range by specific index, so I do not use it for this case.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseAdding(IEnumerable<TItem> items, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ToInterfaceList(items), index));
        }

        /// <summary>
        /// Has to be invoked or set by indexer, better with index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseReplacing(TItem oldItem, TItem newItem, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            var oldItems = new SingleItemList<TItem>(oldItem);
            var newItems = new SingleItemList<TItem>(newItem);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)newItems, (IList)oldItems, index));
        }

        /// <summary>
        /// Has to be invoked or set by indexer, better with index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseReplacing(IEnumerable<TItem> oldItems, IEnumerable<TItem> newItems, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ToInterfaceList(oldItems), ToInterfaceList(newItems), index));
        }

        /// <summary>
        /// Has to be invoked on single item moving, indexes are necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseMoving(TItem item, int oldIndex, int newIndex)
        {
            if (CollectionChanged == null)
                return;

            var items = new SingleItemList<TItem>(item);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, (IList)items, newIndex, oldIndex));
        }

        /// <summary>
        /// Using for mass moving, indexes are necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseMoving(IEnumerable<TItem> items, int oldIndex, int newIndex)
        {
            if (CollectionChanged == null)
                return;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, ToInterfaceList(items), newIndex, oldIndex));
        }

        /// <summary>
        /// Has to be invoked on single item removing, possible with index (if it is a removing by index).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseRemoving(TItem item, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            var items = new SingleItemList<TItem>(item);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)items, index));
        }

        /// <summary>
        /// Using for range removing.
        /// </summary>
        /// <remarks>
        /// Currently I don't know a collection that can remove a range by specific index, so I do not use it for this case.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseRemoving(IEnumerable<TItem> items, int index = -1)
        {
            if (CollectionChanged == null)
                return;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ToInterfaceList(items), index));
        }

        /// <summary>
        /// Used for as Microsoft documentation says "dramatic changing" of the collection as clearing f.e.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseClearing()
        {
            if (CollectionChanged == null)
                return;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        //====================================================================================================//
        //======================================== Property Changing =========================================//

        /// <summary>
        /// Invokes a <see cref="PropertyChanging"/> for the input property name.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaisePropertyChanging([CallerMemberName] string name = null) => PropertyChanging.InvokeSafe(this, name);

        /// <summary>
        /// Invokes a <see cref="PropertyChanging"/> for the input arguments.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// <br/> This method is here for the possibility to re-throw the arguments, please use the another method for the direct invocation, it is optimized.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaisePropertyChanging(PropertyChangingEventArgs eventArguments) => PropertyChanging.InvokeSafe(this, eventArguments);

        /// <summary>
        /// Invokes a <see cref="PropertyChanging"/> for the indexer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseIndexerChanging(string key = null)
        {
            if (PropertyChanging == null)
                return;

            if (key != null)
                RaisePropertyChanging(string.Format(IndexerPropertyFormat, key));

            RaisePropertyChanging(IndexerChanging);
        }

        /// <summary>
        /// Invokes a <see cref="PropertyChanged"/> for the input property name.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaisePropertyChanged([CallerMemberName] string name = null) => PropertyChanged.InvokeSafe(this, name);

        /// <summary>
        /// Invokes a <see cref="PropertyChanged"/> for the input arguments.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// <br/> This method is here for the possibility to re-throw the arguments, please use the another method for the direct invocation, it is optimized.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaisePropertyChanged(PropertyChangedEventArgs eventArguments) => PropertyChanged.InvokeSafe(this, eventArguments);

        /// <summary>
        /// Invokes a <see cref="PropertyChanged"/> for the indexer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseIndexerChanged(string key = null)
        {
            if (PropertyChanged == null)
                return;

            if (key != null)
                RaisePropertyChanged(string.Format(IndexerPropertyFormat, key));

            RaisePropertyChanged(IndexerChanged);
        }
    }
}
