using System;
using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="ICollection{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IObservableCollection{T}.Count"/> can be observed.<br/>
    /// Implements <see cref="IReadOnlyCollection{T}"/> as well
    /// </remarks>
    public interface IObservableCollection<TItem> : ICollection<TItem>, IReadOnlyObservableCollection<TItem>
    {
        /// <summary>
        /// Adds a group of items as a single operation
        /// </summary>
        /// <remarks>
        /// More preferable for the performance than adding items one by one
        /// </remarks>
        /// <exception cref="NotSupportedException">The collection is read-only</exception>
        void AddRange(IEnumerable<TItem> items);

        /// <summary>
        /// Adds a group of items as a single operation
        /// </summary>
        /// <remarks>
        /// More preferable for the performance than removing items one by one
        /// </remarks>
        /// <exception cref="NotSupportedException">The collection is read-only</exception>
        void RemoveRange(IEnumerable<TItem> items);
    }
}
