using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class File : Entity<File>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int FileId { get; set; }
        public override int Id
        {
            set => value = FileId;
            get => FileId;
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Link { get; set; }
        public string _Source { get; set; }
        public string Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if(_Source != value)
                {
                    _Source = value;
                }
                OnPropertyChanged();
            }
        }

        private bool? _IsUploaded { get; set; } = false;

        public bool? IsUploaded
        { get 
            {
                return _IsUploaded;
            }
            set
            {
                if(value != _IsUploaded)
                {
                    _IsUploaded = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _IsUploading { get; set; } = false;
        public bool IsUploading
        {
            get
            {
                return _IsUploading;
            }
            set
            {
                if (value != _IsUploading)
                {
                    _IsUploading = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _UploadFailed { get; set; } = false;

        public bool UploadFailed
        {
            get
            {
                return _UploadFailed;
            }
            set
            {
                if (value != _UploadFailed)
                {
                    _UploadFailed = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Type { get; set; }

        public async override Task<string> Add()
        {
            string res = "OK";

            res = await base.Add();


            if (res == "OK")
            {
                try
                {
                    File _this = JsonConvert.DeserializeObject<File>(AddedItem);
                    this.FileId = _this.FileId;
                    this.TenantId = _this.TenantId;
                    this.Token = _this.Token;
                    this.CreatedOn = _this.CreatedOn;
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
            }
            return res;

        }

        public async Task<string> Upload()
        {
            using (var client = new HttpClient())
            {
                string _Result = "OK";
                string url = Secrets.ApiAddress + $"UploadFile?token=" + Secrets.TenantToken + "&fileToken=" + this.Token;

                try
                {
                    HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                    var serialized = JsonConvert.SerializeObject(this);
                    var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await Static.Functions.GetPostRetryAsync(() => httpClient.PostAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        IsUploaded = false;//hasn't been saved
                        _Result = httpResponse.ReasonPhrase;
                    }
                    else
                    {
                        IsUploaded = true;//has been saved successfully
                        var rString = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                    _Result = ex.Message;
                    Static.Functions.CreateError(ex, "No connection", nameof(this.Add), this.GetType().Name);
                }
                return _Result;

            }
        }

        public async Task RemoveFromUploadQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            db.Delete<File>(FileId);
            db.Close();
        }

    }
}
