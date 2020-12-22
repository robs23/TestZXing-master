using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class FileKeeper : Keeper<File>
    {
        protected override string ObjectName => "File";

        protected override string PluralizedObjectName => "Files";

        public async Task AddToUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            db.CreateTable<File>();
            if (Items.Any())
            {
                db.InsertOrReplaceAll(Items);
            }

        }

        public async Task RestoreUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            Items = new ObservableCollection<File>(db.Table<File>());
        }

        public async Task Upload()
        {
            if (Items.Any())
            {
                foreach(File f in Items)
                {
                    f.IsUploading = true;
                    var res = await f.Upload();
                    f.IsUploading = false;
                    if(res == "OK")
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

        public async Task DeleteUploaded()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            foreach (File f in Items.Where(i=>i.IsUploaded==true).ToList())
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
