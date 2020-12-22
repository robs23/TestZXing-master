using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using Xamarin.Essentials;
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
        [PrimaryKey]
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

        private string _UploadStatus { get; set; }
        public string UploadStatus
        {
            get
            {
                return _UploadStatus;
            }
            set
            {
                if (value != _UploadStatus)
                {
                    _UploadStatus = value;
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

        public bool IsImage
        {
            get
            {
                string[] imageFormats = { "png", "jpg", "jpeg", "gif" };
                bool res = false;

                if (!string.IsNullOrEmpty(Type))
                {
                    if (imageFormats.Contains(Type.ToLower()))
                    {
                        res = true;
                    }
                }
                else if (!string.IsNullOrEmpty(Name))
                {
                    if (imageFormats.Contains(Name.Split('.').Last().ToLower()))
                    {
                        res = true;
                    }
                }
                return res;
            }
        }

        public string ThumbnailPlaceholder
        {
            get
            {
                string res = "icon_unknown.png";
                string[] excels = { "xls", "xlsx", "xlsm" };
                string[] words = { "doc", "docx", "docm" };
                string[] videos = { "mp4", "avi" };

                if (IsImage)
                {
                    //if (IsUploaded==true)
                    //{
                    //    //prefer online thumb
                    //    //first load placeholder local img
                    //    //to be replaced wtih downloaded file
                    //    res = new Bitmap(JDE_Scanner_Desktop.Properties.Resources.Image_icon_64);
                    //}
                    //else
                    //{
                    //    //get from disk
                    //    res = Image.FromFile(Link);
                    //}
                    res = "icon_image.png";
                }
                else
                {
                    if (excels.Contains(Type.ToLower()))
                    {
                        res = "icon_excel.png";
                    }
                    if (words.Contains(Type.ToLower()))
                    {
                        res = "icon_word";
                    }
                    if (videos.Contains(Type.ToLower()))
                    {
                        res = "icon_video.png";
                    }
                    if (Type.ToLower() == "pdf")
                    {
                        res = "icon_pdf.png";
                    }
                }
                return res;
            }
        }

        public async Task<string> GetPreview()
        {
            string res = null;
            if (IsUploaded == true)
            {
                //try to get online thumb
            }
            else
            {
                res = Link;
            }
            return res;
        }


        public string Type { get; set; }

        public async override Task<string> Add(string args)
        {
            string res = "OK";

            res = await base.Add(args);

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
                    var content = new MultipartFormDataContent();
                    var file = new FileResult(Link);
                    StreamContent img = new StreamContent(await file.OpenReadAsync());
                    img.Headers.Add("Content-Type", file.ContentType);
                    content.Add(img ,"file", Name);
                    HttpResponseMessage httpResponse = await Static.Functions.GetPostRetryAsync(() => httpClient.PostAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        IsUploaded = false;//hasn't been saved
                        UploadStatus = httpResponse.ReasonPhrase;
                        _Result = httpResponse.ReasonPhrase;
                    }
                    else
                    {
                        IsUploaded = true;//has been saved successfully
                        UploadStatus = $"Wysłano o {DateTime.Now.ToString("HH:mm:ss")}";
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
