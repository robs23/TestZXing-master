using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TestZXing.Static;
using SQLite;

namespace TestZXing.Models
{
    public class UserLogKeeper : Keeper<UserLog>
    {
        protected override string ObjectName => "UserLog";

        protected override string PluralizedObjectName => "UserLogs";
        public bool IsWorking { get; set; } = false;

        public async Task AddToUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            db.CreateTable<UserLog>();
            if (Items.Any(i => i.IsUploaded == false && i.IsUploading == false && !db.Table<File>().Any(x => x.FileId == i.FileId)))
            {
                db.InsertOrReplaceAll(Items.Where(i => i.IsUploaded == false && i.IsUploading == false && !db.Table<File>().Any(x => x.FileId == i.FileId)));
            }

        }

        public async Task RestoreUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            Items = new ObservableCollection<File>(db.Table<File>());
        }

        public async Task Upload()
        {
            try
            {
                IsWorking = true;
                if (Items.Any())
                {
                    foreach (File f in Items)
                    {
                        f.IsUploading = true;
                        var res = await f.Upload();
                        f.IsUploading = false;
                        if (res == "OK")
                        {
                            f.IsUploaded = true;
                        }
                        else
                        {
                            f.IsUploaded = false;
                            f.UploadFailed = true;
                        }
                    }
                    await DeleteUploaded();
                }
            }
            catch (Exception ex)
            {

            }
            IsWorking = false;

        }

        public async Task DeleteUploaded()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            foreach (File f in Items.Where(i => i.IsUploaded == true).ToList())
            {
                db.Delete<File>(f.FileId);
                Items.Remove(f);
            }
            db.Close();
        }

        public async Task DeleteAll()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            db.DeleteAll<File>();
            db.Close();
        }
    }
}
