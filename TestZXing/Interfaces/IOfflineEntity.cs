using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.Interfaces
{
    public interface IOfflineEntity<T> where T: Entity<T>, new()
    {
        int Id { get; set; }
        string OfflineDescription { get;}
        string SyncStatus { get; set; }
    }
}
