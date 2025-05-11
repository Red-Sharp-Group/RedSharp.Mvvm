using System.ComponentModel;

namespace RedSharp.Mvvm.Collections.Models
{
    public class ItemChangingEventArgs : PropertyChangingEventArgs
    {
        public ItemChangingEventArgs(object item, string property) : base(property)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
