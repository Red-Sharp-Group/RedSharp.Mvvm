using System;

namespace RedSharp.Mvvm.Bindings.Interfaces
{
    /// <summary>
    /// General implementation of the binding functionality, 
    /// represents some kind of observable object, so the <see cref="Value"/> can be observed like in view-model object
    /// </summary>
    /// <remarks>
    /// <see cref="IsReadOnly"/> property is immutable
    /// </remarks>
    public interface IBindingExpression<TValue> : IReadOnlyBindingExpression<TValue>
    {
        /// <summary>
        /// True if the <see cref="Value"/> is only readable
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyBindingExpression{TValue}.Value"/>
        /// </summary>
        /// <exception cref="NotSupportedException">If the <see cref="IsReadOnly"/> is true</exception>
        new TValue Value { get; set; } 
    }
}
