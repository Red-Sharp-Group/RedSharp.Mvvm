using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Mvvm.Collections.Interfaces;
using RedSharp.Mvvm.Collections.Models;

namespace RedSharp.Mvvm.Collections.Abstracts
{
    public abstract class NotifyCollectionChangedWrapperBase<TInput, TOutput> : NotifyCollectionChangedBase<TOutput>
    {
        public struct CollectionWrapper<TCastInput> : IEnumerable<TCastInput>
        {
            public class CastingEnumerator : IEnumerator<TCastInput>
            {
                private IEnumerator _enumerator;

                public CastingEnumerator(IEnumerator enumerator)
                {
                    _enumerator = enumerator;
                }

                public TCastInput Current => (TCastInput)_enumerator.Current;

                object IEnumerator.Current => _enumerator.Current;

                public bool MoveNext() => _enumerator.MoveNext();

                public void Reset() => _enumerator.Reset();

                public void Dispose()
                { }
            }

            public ICollection _collection;

            public CollectionWrapper(ICollection collection)
            {
                _collection = collection;
            }

            public CollectionWrapper(IEnumerable<TCastInput> enumerable)
            {
                if (enumerable is ICollection)
                    _collection = (ICollection)enumerable;
                else
                    _collection = enumerable.ToList();
            }

            public int Count => _collection.Count;

            public IEnumerator<TCastInput> GetEnumerator()
            {
                var enumerator = _collection.GetEnumerator();

                if (enumerator is IEnumerator<TCastInput> casted)
                    return casted;
                else
                    return new CastingEnumerator(enumerator);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _collection.GetEnumerator();
            }
        }

        private IEnumerable<TInput> _originalCollection;

        protected IEnumerable<TInput> AssociatedCollection => _originalCollection;

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs arguments)
        {
            switch (arguments.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (arguments.NewItems != null)
                        InternalAddItems(new CollectionWrapper<TInput>(arguments.NewItems), arguments.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (arguments.OldItems != null && arguments.NewItems != null)
                        InternalReplaceItems(new CollectionWrapper<TInput>(arguments.OldItems), new CollectionWrapper<TInput>(arguments.NewItems), arguments.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (arguments.NewItems != null)
                        InternalMoveItems(new CollectionWrapper<TInput>(arguments.NewItems), arguments.OldStartingIndex, arguments.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (arguments.OldItems != null)
                        InternalRemoveItems(new CollectionWrapper<TInput>(arguments.NewItems), arguments.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    InternalClearItems();
                    break;
            }
        }

        protected virtual void OnItemChanging(object sender, ItemChangingEventArgs arguments)
        { }

        protected virtual void OnItemChanged(object sender, ItemChangedEventArgs arguments)
        { }

        protected void AssociateCollection(IEnumerable<TInput> collection)
        {
            if (_originalCollection != null)
                DissociateCollection();

            _originalCollection = collection;

            if (_originalCollection == null)
                return;

            if (_originalCollection is INotifyCollectionChanged)
                ((INotifyCollectionChanged)_originalCollection).CollectionChanged += OnCollectionChanged;

            if (_originalCollection is INotifyItemChanging)
                ((INotifyItemChanging)_originalCollection).ItemChanging += OnItemChanging;

            if (_originalCollection is INotifyItemChanged)
                ((INotifyItemChanged)_originalCollection).ItemChanged += OnItemChanged;

            InternalAddItems(new CollectionWrapper<TInput>(collection), -1);
        }

        protected void DissociateCollection()
        {
            if (_originalCollection == null)
                return;

            if (_originalCollection is INotifyCollectionChanged)
                ((INotifyCollectionChanged)_originalCollection).CollectionChanged -= OnCollectionChanged;

            if (_originalCollection is INotifyItemChanging)
                ((INotifyItemChanging)_originalCollection).ItemChanging -= OnItemChanging;

            if (_originalCollection is INotifyItemChanged)
                ((INotifyItemChanged)_originalCollection).ItemChanged -= OnItemChanged;

            InternalClearItems();

            _originalCollection = null;
        }

        protected abstract void InternalAddItems(CollectionWrapper<TInput> items, int startIndex);
        
        protected abstract void InternalReplaceItems(CollectionWrapper<TInput> oldItems, CollectionWrapper<TInput> newItems, int startIndex);

        protected abstract void InternalMoveItems(CollectionWrapper<TInput> items, int oldStartIndex, int newStartIndex);

        protected abstract void InternalRemoveItems(CollectionWrapper<TInput> items, int startIndex);

        protected abstract void InternalClearItems();
    }
}
