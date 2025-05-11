using System.Collections.Generic;
using RedSharp.Mvvm.Bindings.Abstracts;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{
    /// <summary>
    /// Usually it is a first chain in the expression, returns fixed value
    /// </summary>
    public class FixedValueChain<TValue> : BindingChainBase<TValue>
    {
        private TValue _value;

        /// <summary>
        /// Always true
        /// </summary>
        public override bool IsReadOnly => true;

        /// <summary>
        /// Always read-only
        /// </summary>
        public override TValue Value
        {
            get => _value;
            set => ThrowIfReadOnly();
        }

        /// <summary>
        /// Does not work, returns false every time
        /// </summary>
        public override bool TryUpdateTarget(IBindingChain previousChain) => false;

        /// <summary>
        /// The only way to set value in this chain
        /// </summary>
        public void ForceUpdateTarget(TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(_value, value))
                return;

            _value = value;

            RaiseValueChanged();
        }
    }
}
