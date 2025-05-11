using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="IList{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IObservableList{T}.Count"/> can be observed.
    /// </remarks>
    public interface IObservableList<TItem> : IList<TItem>, IReadOnlyObservableList<TItem>, IObservableCollection<TItem>
    {
        /// <summary>
        /// Inserts a group of items as a single operation
        /// </summary>
        /// <remarks>
        /// More preferable for the performance than insert items one by one
        /// </remarks>
        /// <exception cref="NotSupportedException">The collection is read-only</exception>
        void InsertRange(int index, IEnumerable<TItem> items);

        /// <summary>
        /// Removes a group of items as a single operation at index
        /// </summary>
        /// <remarks>
        /// More preferable for the performance than removing items one by one
        /// </remarks>
        /// <exception cref="NotSupportedException">The collection is read-only</exception>
        void RemoveRange(int index, int count);
    }
}
