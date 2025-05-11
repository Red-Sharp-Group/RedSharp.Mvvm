using System;
using System.Collections.Generic;
using RedSharp.Mvvm.Bindings.Abstracts;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{
    /// <summary>
    /// The chain that can transform the input value
    /// </summary>
    /// <remarks>
    /// Currently it is always read-only
    /// </remarks>
    public class ConverterChain<TInput, TOutput> : BindingChainBase<TOutput>
    {
        private Func<TInput, TOutput> _converter;
        private TOutput _value;

        public ConverterChain(Func<TInput, TOutput> converter)
        {
            _converter = converter;
        }

        /// <summary>
        /// Always true
        /// </summary>
        public override bool IsReadOnly => true;

        /// <summary>
        /// Always read-only
        /// </summary>
        public override TOutput Value
        {
            get => _value;
            set => ThrowIfReadOnly();
        }

        /// <inheritdoc/>
        public override bool TryUpdateTarget(IBindingChain previousChain)
        {
            var newValue = default(TOutput);

            if (previousChain == null)
            {
                newValue = _converter.Invoke(default);
            }
            else
            {
                var inputValue = previousChain.Value;

                if (inputValue is TInput castedValue)
                    newValue = _converter.Invoke(castedValue);
                else
                    newValue = _converter.Invoke(default);
            }

            if (EqualityComparer<TOutput>.Default.Equals(_value, newValue))
                return false;

            _value = newValue;

            return true;
        }
    }
}
