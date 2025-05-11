using System.ComponentModel;

namespace RedSharp.Mvvm.Collections.Models
{
    public class ItemChangedEventArgs : PropertyChangedEventArgs
    {
        public ItemChangedEventArgs(object item, string property) : base(property)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
