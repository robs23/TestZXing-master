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

        public bool? IsUploaded { get; set; } = false;

        public string Type { get; set; }

        public async Task<string> Upload()
        {
            using (var client = new HttpClient())
            {
                string _Result = "OK";
                string url = Secrets.ApiAddress + $"UploadFile?token=" + Secrets.TenantToken + "&UserId=" + RuntimeSettings.UserId;

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

    }
}
