using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TestZXing.Interfaces
{
    public interface IOfflineKeeper<T>: IOfflineKeeper
    {
        ObservableCollection<T> Items { get; set; }
    }
}
