using System.Collections.Generic;
using System.ComponentModel;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{
    /// <summary>
    /// The default implementation of the <see cref="IBindingExpression{TValue}"/> interfaces
    /// </summary>
    public class BindingExpression<TValue> : IBindingExpression<TValue>
    {
        public static readonly PropertyChangingEventArgs ValueChanging = new PropertyChangingEventArgs(nameof(Value));

        public static readonly PropertyChangedEventArgs ValueChanged = new PropertyChangedEventArgs(nameof(Value));

        private IBindingChain[] _chains;
        private IBindingChain<TValue> _lastChain;
        private TValue _cachedValue;

        public event PropertyChangingEventHandler PropertyChanging;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public BindingExpression(IReadOnlyCollection<IBindingChain> chains)
        {
            _chains = new IBindingChain[chains.Count];

            var index = 0;

            foreach (var chain in chains)
            {
                _chains[index] = chain;

                var indexClosure = index;

                chain.ValueChanged += (sender, arguments) => OnChainValueUpdate(indexClosure);

                index++;
            }

            _lastChain = (IBindingChain<TValue>)_chains[_chains.Length - 1];

            OnChainValueUpdate(0);
        }

        /// <inheritdoc/>
        public bool IsReadOnly => _lastChain.IsReadOnly;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// The values is cached to be able to perform <see cref="INotifyPropertyChanging"/>
        /// </remarks>
        public TValue Value
        {
            get => _cachedValue;
            set => _lastChain.Value = value;
        }

        /// <summary>
        /// Performs cycle update from the changed chain to the last possible
        /// </summary>
        private void OnChainValueUpdate(int index)
        {
            var lastIndex = index;

            for (int i = index + 1; i < _chains.Length; i++)
            {
                if (_chains[i].TryUpdateTarget(_chains[i - 1]))
                    lastIndex = i;
                else
                    break;
            }

            if (lastIndex + 1 == _chains.Length)
            {
                PropertyChanging.InvokeSafe(this, ValueChanging);

                _cachedValue = _lastChain.Value;

                PropertyChanged.InvokeSafe(this, ValueChanged);
            }
        }
    }
}
