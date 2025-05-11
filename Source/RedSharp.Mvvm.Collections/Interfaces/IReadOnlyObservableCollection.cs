using System.Collections.Generic;
using System.ComponentModel;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="IReadOnlyCollection{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReadOnlyObservableCollection{T}.Count"/> can be observed.
    /// </remarks>
    public interface IReadOnlyObservableCollection<out TItem> : IReadOnlyCollection<TItem>, IObservableEnumerable<TItem>, INotifyPropertyChanging, INotifyPropertyChanged
    { }
}
