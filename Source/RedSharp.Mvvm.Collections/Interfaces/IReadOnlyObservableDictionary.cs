using System.Collections.Generic;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Implementation of <see cref="IReadOnlyDictionary{TKey, TValue}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReadOnlyObservableDictionary{TKey, TValue}.Count"/> can be observed.
    /// </remarks>
    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyObservableCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// A new read-only collection for the dictionary keys with reactivity.
        /// </summary>
        new IObservableEnumerable<TKey> Keys { get; }

        /// <summary>
        /// A new read-only collection for the dictionary values with reactivity.
        /// </summary>
        new IObservableEnumerable<TValue> Values { get; }
    }
}
