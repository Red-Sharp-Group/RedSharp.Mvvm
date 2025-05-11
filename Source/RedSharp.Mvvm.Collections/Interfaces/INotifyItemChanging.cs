using RedSharp.Mvvm.Collections.Models;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    public delegate void ItemChangingEventHandler(object sender, ItemChangingEventArgs e);

    public interface INotifyItemChanging
    {
        event ItemChangingEventHandler ItemChanging;
    }
}
