using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Essentials;

namespace TestZXing.Models
{
    public abstract class Entity<T> where T: class, new()//Entity<T>, new()
    {
        public abstract int Id { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? LmBy { get; set; }
        public string LmByName { get; set; }
        public DateTime? LmOn { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string AddedItem { get; set; }

        public bool IsSaved { get; set; } = false;

        public bool? IsArchived { get; set; } = false;

        public bool? IsSyncing { get; set; } = false;

        public bool? IsSynced { get; set; } = false;

        public virtual async Task<string> Add()
        {
            using (var client = new HttpClient())
            {
                string _Result = "OK";
                string url = Secrets.ApiAddress + $"Create{typeof(T).Name}?token=" + Secrets.TenantToken + "&UserId=" + RuntimeSettings.UserId;

                try
                {
                    HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                    var serialized = JsonConvert.SerializeObject(this);
                    var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await Static.Functions.GetPostRetryAsync(() => httpClient.PostAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        IsSaved = false;//hasn't been saved
                        _Result = httpResponse.ReasonPhrase;
                    }
                    else
                    {
                        IsSaved = true;//has been saved successfully
                        var rString = await httpResponse.Content.ReadAsStringAsync();
                        AddedItem = rString;
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

        public virtual async Task<string> Add(string args)
        {
            using (var client = new HttpClient())
            {
                string _Result = "OK";
                string url = Secrets.ApiAddress + $"Create{typeof(T).Name}?token={Secrets.TenantToken}&UserId={RuntimeSettings.CurrentUser.UserId}&{args}";

                try
                {
                    HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                    var serialized = JsonConvert.SerializeObject(this);
                    var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await Static.Functions.GetPostRetryAsync(() => httpClient.PostAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        IsSaved = false;//hasn't been saved
                        _Result = httpResponse.ReasonPhrase;
                    }
                    else
                    {
                        IsSaved = true;//has been saved successfully
                        var rString = await httpResponse.Content.ReadAsStringAsync();
                        AddedItem = rString;
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

        public virtual async Task<string> Add(string attachmentPath, string args)
        {
            using (var client = new HttpClient())
            {
                string _Result = "OK";
                var serialized = JsonConvert.SerializeObject(this);
                string url = Secrets.ApiAddress + $"Create{typeof(T).Name}?token={Secrets.TenantToken}&{typeof(T).Name}Json={serialized}&UserId={RuntimeSettings.CurrentUser.UserId}&{args}";

                try
                {
                    HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });

                    var content = new MultipartFormDataContent();
                    var file = new FileResult(attachmentPath);
                    StreamContent img = new StreamContent(await file.OpenReadAsync());
                    img.Headers.Add("Content-Type", file.ContentType);
                    content.Add(img, "file", file.FileName);

                    HttpResponseMessage httpResponse = await Static.Functions.GetPostRetryAsync(() => httpClient.PostAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        IsSaved = false;//hasn't been saved
                        _Result = httpResponse.ReasonPhrase;
                    }
                    else
                    {
                        IsSaved = true;//has been saved successfully
                        var rString = await httpResponse.Content.ReadAsStringAsync();
                        AddedItem = rString;
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

        public async Task<string> Edit()
        {
            string url = Secrets.ApiAddress + $"Edit{typeof(T).Name}?token=" + Secrets.TenantToken + $"&id={this.Id}&UserId={RuntimeSettings.CurrentUser.UserId}";
            string _Result = "OK";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serializedProduct = JsonConvert.SerializeObject(this);
                var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await Static.Functions.GetPostRetryAsync(() => httpClient.PutAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                if (!result.IsSuccessStatusCode)
                {
                    IsSaved = false; //hasn't been saved
                    _Result = result.ReasonPhrase;
                }
                else
                {
                    IsSaved = true;//has been saved successfully
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Static.Functions.CreateError(ex, "No connection", nameof(this.Edit), this.GetType().Name);
            }


            return _Result;
        }

        public async Task<string> Edit(string attachmentPath)
        {
            string _Result = "OK";

            if (string.IsNullOrEmpty(attachmentPath))
            {
                //_Result = await Edit();
            }
            else
            {
                if (!System.IO.File.Exists(attachmentPath))
                {
                    _Result = $"Plik w lokacji {attachmentPath} nie istnieje lub program nie ma uprawnień dostępu do tej lokacji..";
                }
                else
                {
                    try
                    {
                        var serialized = JsonConvert.SerializeObject(this);
                        string url = Secrets.ApiAddress + $"Edit{typeof(T).Name}?token=" + Secrets.TenantToken + $"&id={this.Id}&UserId={RuntimeSettings.CurrentUser.UserId}&{typeof(T).Name}Json={serialized}";
                        HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                        var content = new MultipartFormDataContent();
                        var file = new FileResult(attachmentPath);
                        StreamContent img = new StreamContent(await file.OpenReadAsync());
                        img.Headers.Add("Content-Type", file.ContentType);
                        content.Add(img, "file", file.FileName);
                        HttpResponseMessage result = await Static.Functions.GetPostRetryAsync(() => httpClient.PutAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                        if (!result.IsSuccessStatusCode)
                        {
                            IsSaved = false; //hasn't been saved
                            _Result = result.ReasonPhrase;
                        }
                        else
                        {
                            IsSaved = true;//has been saved successfully
                        }
                    }
                    catch (Exception ex)
                    {
                        _Result = ex.Message;
                        Static.Functions.CreateError(ex, "No connection", nameof(this.Edit), this.GetType().Name);
                    }
                }

            }
            
            return _Result;
        }



        public async Task<string> Remove()
        {
            string _Result = "OK";
            string url = Secrets.ApiAddress + $"Delete{typeof(T).Name}?token=" + Secrets.TenantToken + $"&id={this.Id}&UserId={RuntimeSettings.CurrentUser.UserId}";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage result = await Static.Functions.GetPostRetryAsync(() => httpClient.DeleteAsync(new Uri(url)), TimeSpan.FromSeconds(3));
                if (!result.IsSuccessStatusCode)
                {
                    if(result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        IsSaved = true;
                    }
                    else
                    {
                        IsSaved = false;
                        _Result = String.Format("Serwer zwrócił błąd przy próbie usunięcia pozycji {0}. Wiadomość: " + result.ReasonPhrase, this.Id);
                    }
                    
                }
                else
                {
                    IsSaved = true;
                }
            }catch(Exception ex)
            {
                _Result = ex.Message;
                Static.Functions.CreateError(ex, "No connection", nameof(this.Remove), this.GetType().Name);
            }

            return _Result;
        }

        //public void AddToSyncQueue()
        //{
        //    try
        //    {
        //        var connection = new SQLiteConnection(RuntimeSettings.LocalDbPath);
        //        connection.CreateTable<T>();
        //        connection.Insert(this);
        //    }catch(Exception ex)
        //    {
        //        throw;
        //    }
            

        //    //IRepository<T> repo = new Repository<T>(connection);

        //    //var userLogId = repo.Insert(new T());
        //}

    }
}
