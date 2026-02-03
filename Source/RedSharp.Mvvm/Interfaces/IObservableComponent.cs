using System.ComponentModel;

namespace RedSharp.Mvvm.Interfaces
{
    public interface IObservableComponent : INotifyPropertyChanging, INotifyPropertyChanged
    {
        void OwnerPropertyChanged<TValue>(TValue value, string propertyName);
    }
}
