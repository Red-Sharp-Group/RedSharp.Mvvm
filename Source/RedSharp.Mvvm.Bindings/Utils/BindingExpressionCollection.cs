using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using RedSharp.General.Collections.Abstracts;
using RedSharp.General.Collections.Interfaces;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Utils
{
    /// <summary>
    /// Special version of the observable collection, that wraps binding expression
    /// </summary>
    /// <remarks>
    /// Calls <see cref="INotifyCollectionChanged.CollectionChanged"/> 
    /// with "add" and "remove" events when the value of the binding is changed.
    /// <br/>Can works with any collection not only with implementation of <see cref="INotifyCollectionChanged"/>, 
    /// but the mentioned implementation will be better for current object
    /// </remarks>
    public class BindingExpressionCollection<TItem> : NotifyCollectionChangedWrapperBase<TItem, TItem>, IReadOnlyObservableCollection<TItem>
    {
        private IReadOnlyBindingExpression<IEnumerable<TItem>> _bindingExpression;
        private int _count;

        public BindingExpressionCollection(IReadOnlyBindingExpression<IEnumerable<TItem>> bindingExpression)
        {
            ArgumentsGuard.ThrowIfNull(bindingExpression, nameof(bindingExpression));

            _bindingExpression = bindingExpression;

            _bindingExpression.PropertyChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, EventArgs arguments)
        {
            var value = _bindingExpression.Value;

            if (AssociatedCollection == value)
                return;

            AssociateCollection(value);
        }

        public int Count
        {
            get => _count;
            private set
            {
                if (_count == value)
                    return;

                RaisePropertyChanging(CountChanging);

                _count = value;

                RaisePropertyChanged(CountChanged);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (AssociatedCollection == null)
                return Enumerable.Empty<TItem>().GetEnumerator();

            return AssociatedCollection.GetEnumerator();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            if (AssociatedCollection == null)
                return Enumerable.Empty<TItem>().GetEnumerator();

            return AssociatedCollection.GetEnumerator();
        }

        private int GetCount()
        {
            if (AssociatedCollection == null)
                return 0;

            if (AssociatedCollection is ICollection<TItem> collection)
                return collection.Count;

            if (AssociatedCollection is IReadOnlyCollection<TItem> readOnlyCollection)
                return readOnlyCollection.Count;

            return AssociatedCollection.Count();
        }

        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs arguments)
        {
            Count = GetCount();

            RaiseCollectionChanged(arguments);
        }

        protected override void InternalAddItems(CollectionWrapper<TItem> items)
        {
            Count = GetCount();

            RaiseAdding(items.ToList());
        }

        protected override void InternalRemoveItems(CollectionWrapper<TItem> items)
        {
            Count = GetCount();

            RaiseRemoving(items.ToList());
        }

        protected override void InternalClearItems()
        {
            Count = GetCount();

            RaiseClearing();
        }
    }
}
