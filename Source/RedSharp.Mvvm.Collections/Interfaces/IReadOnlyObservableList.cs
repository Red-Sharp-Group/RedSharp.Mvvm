using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="IReadOnlyList{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReadOnlyObservableList{T}.Count"/> can be observed.
    /// </remarks>
    public interface IReadOnlyObservableList<out TItem> : IReadOnlyList<TItem>, IReadOnlyObservableCollection<TItem>
    { }
}
