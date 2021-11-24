using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Static;

namespace TestZXing.Interfaces
{
    public interface IOfflineEntity<T> where T : class, new()
    {
        //public int Id { get; set; }
        //public bool IsSyncing { get; set; }
        //public bool IsSynced { get; set; }

        //public void AddToSyncQueue()
        //{
        //    try
        //    {
        //        var connection = new SQLiteConnection(RuntimeSettings.LocalDbPath);
        //        connection.CreateTable<T>();
        //        connection.Insert(this);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}
    }
}
