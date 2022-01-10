using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class FileKeeper : Keeper<File>, IOfflineKeeper, IOfflineKeeper<File>
    {
        protected override string ObjectName => "File";

        protected override string PluralizedObjectName => "Files";
        public int? PartId { get; set; }
        public int? PlaceId { get; set; }
        public int? ProcessId { get; set; }
        public int? UserLogId { get; set; }
        public bool UploadKeeper { get; set; }

        public FileKeeper(int? partId = null, int? placeId = null, int? processId = null, int? userLogId = null, bool uploadKeeper = false)
        {
            PartId = partId;
            PlaceId = placeId;
            ProcessId = processId;
            UserLogId = userLogId;
            UploadKeeper = uploadKeeper;
        }

        public async Task Initialize()
        {
            string args = null;
            if (UploadKeeper)
            {
                //it's uploadKeeper. Show all files that wait for upload in local Db
                RestoreUploadQueue();
            }
            else if (PartId != null || PlaceId != null || ProcessId != null)
            {
                if (PartId != null)
                {
                    args = $"PartId={PartId}";
                }
                else if (PlaceId != null)
                {
                    args = $"PlaceId={PlaceId}";
                }
                else if (ProcessId != null)
                {
                    args = $"ProcessId={ProcessId}";
                }
                //Bring from the cloud Db
                try
                {
                    await this.Reload(args);
                }
                catch (Exception)
                {
                    await App.Current.MainPage.DisplayAlert(RuntimeSettings.ConnectionErrorTitle, RuntimeSettings.ConnectionErrorText, "OK");
                }
            }
        }

        public async Task AddToUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            db.CreateTable<File>();
            if (Items.Any(i=>i.IsUploaded==false && i.IsUploading==false && !db.Table<File>().Any(x => x.FileId==i.FileId)))
            {
                db.InsertOrReplaceAll(Items.Where(i=>i.IsUploaded==false && i.IsUploading==false && !db.Table<File>().Any(x => x.FileId == i.FileId)));
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
            }catch(Exception ex)
            {

            }
            IsWorking = false;
            
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

        public async Task Sync()
        {
            if (IsOfflineKeeper)
            {
                //add args
                string field = "UserLogId";
                await base.Sync($"{field}={UserLogId}");
            }
        }
    }
}
