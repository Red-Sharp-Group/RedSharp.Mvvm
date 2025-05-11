using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="IDictionary{TKey, TValue}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IObservableDictionary{TKey, TValue}.Count"/> can be observed.
    /// </remarks>
    public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>, IObservableCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// A new read-only collection for the dictionary keys with reactivity.
        /// </summary>
        new IObservableCollection<TKey> Keys { get; }

        /// <summary>
        /// A new read-only collection for the dictionary values with reactivity.
        /// </summary>
        new IObservableCollection<TValue> Values { get; }
    }
}
