using System;

namespace RedSharp.Mvvm.Bindings.Interfaces
{
    /// <summary>
    /// The smallest operatable unit of binding
    /// </summary>
    public interface IBindingChain
    {
        /// <summary>
        /// Should be invoked when target of the chain changes its value
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// True if the chain can only read the value
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Target value of the chain
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Should be invoked by the <see cref="IBindingExpression{TValue}"/>
        /// <br/>If the <see cref="Value"/> was changed during this call, the <see cref="ValueChanged"/> 
        /// should not be called, instead the method should return true.
        /// </summary>
        bool TryUpdateTarget(IBindingChain previousChain);
    }

    /// <summary>
    /// The version of <see cref="IBindingChain"/> with value type defined
    /// </summary>
    public interface IBindingChain<TValue> : IBindingChain
    {
        /// <inheritdoc cref="IBindingChain.Value"/>
        new TValue Value { get; set; }
    }
}
