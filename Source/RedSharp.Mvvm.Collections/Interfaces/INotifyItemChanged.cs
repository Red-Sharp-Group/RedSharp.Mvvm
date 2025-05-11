using RedSharp.Mvvm.Collections.Models;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);

    public interface INotifyItemChanged
    {
        event ItemChangedEventHandler ItemChanged;
    }
}
