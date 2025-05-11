using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="ISet{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IObservableSet{T}.Count"/> can be observed.
    /// </remarks>
    public interface IObservableSet<TItem> : ISet<TItem>, IObservableCollection<TItem>
    { }
}
