using System.ComponentModel;

namespace RedSharp.Mvvm.Bindings.Interfaces
{
    /// <summary>
    /// Read only implementation of the binding functionality, 
    /// represents some kind of observable object, so the <see cref="Value"/> can be observed like in view-model object
    /// </summary>
    public interface IReadOnlyBindingExpression<out TValue> : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// Current value of the last chain of the binding expression
        /// </summary>
        TValue Value { get; }
    }
}
