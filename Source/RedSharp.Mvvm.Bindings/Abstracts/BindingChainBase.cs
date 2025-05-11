using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Abstracts
{
    /// <summary>
    /// Basic implementation of the <see cref="IBindingChain{TValue}"/>
    /// </summary>
    public abstract class BindingChainBase<TValue> : IBindingChain<TValue>
    {
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public abstract bool IsReadOnly { get; }


        /// <inheritdoc/>
        public abstract TValue Value { get; set; }

        /// <inheritdoc/>
        object IBindingChain.Value
        {
            get => Value;
            set
            {
                ThrowIfReadOnly();

                Value = (TValue)value;
            }
        }

        /// <inheritdoc/>
        public abstract bool TryUpdateTarget(IBindingChain previousChain);

        /// <summary>
        /// Invokes <see cref="ValueChanged"/> in try catch statement
        /// </summary>
        protected void RaiseValueChanged() => ValueChanged.InvokeSafe(this);

        /// <summary>
        /// Throws <see cref="NotSupportedException"/> if the <see cref="IsReadOnly"/> is true
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NETCOREAPP
        [StackTraceHidden]
#endif
        protected void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException();
        }
    }
}
