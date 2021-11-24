using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using TestZXing.Static;

namespace TestZXing.Interfaces
{
    public interface IOfflineKeeper<T> where T : class, new()
    {
        //ObservableCollection<IOfflineEntity> Items { get; set; }

        //public async Task AddToUploadQueue()
        //{
        //    var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
        //    db.CreateTable<T>();
        //    if (Items.Any(i => i.IsSynced == false && i.IsSyncing == false && !db.Table<T>().Any(x => x.Id == i.Id)))
        //    {
        //        db.InsertOrReplaceAll(Items.Where(i => i.IsSynced == false && i.IsSyncing == false && !db.Table<T>().Any(x => x.Id == i.Id)));
        //    }

        //}

        //public async Task RestoreUploadQueue()
        //{
        //    var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
        //    //Items = new ObservableCollection<T>(db.Table<T>());
        //    Items = new ObservableCollection<IOfflineEntity>(db.Table<IOfflineEntity>().Table)
        //}
    }
}
