using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.Interfaces
{
    public interface IOfflineEntity
    {
        int Id { get; set; }
        string OfflineDescription { get;}
        string SyncStatus { get; set; }
    }
}
