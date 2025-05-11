using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Mvvm.Collections.Models;

namespace RedSharp.Mvvm.Collections.Interfaces
{
    /// <summary>
    /// Basic class for all "notifiable" collections, 
    /// that fix em... "mistake" from the system library, 
    /// where these two interfaces exist separately.
    /// </summary>
    public interface IObservableEnumerable<out TItem> : INotifyCollectionChanged, INotifyItemChanging, INotifyItemChanged, IEnumerable<TItem>
    { }
}
