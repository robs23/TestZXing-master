using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Static;

namespace TestZXing.Models
{
    public abstract class Keeper<T>
    {
        public ObservableCollection<T> Items { get; set; }
        protected abstract string ObjectName { get; }
        protected abstract string PluralizedObjectName { get; }

        public Keeper()
        {
            Items = new ObservableCollection<T>();
        }

        public async Task Reload(string query = null, int? page = null, int? pageSize=null)
        {
            string url = Secrets.ApiAddress + $"Get{PluralizedObjectName}?token=" + Secrets.TenantToken;
            DataService ds = new DataService();
            if (page != null){ url += $"&page={page}"; }
            if (pageSize != null) { url += $"&pageSize={pageSize}"; }
            if (query != null){ url += $"&query={query}"; }

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    Items = JsonConvert.DeserializeObject<ObservableCollection<T>>(output);
                }
                else
                {
                    Items.Clear();
                }
                
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
                throw;
            }
        }

        public async Task Remove(List<int> ids)
        {
            string result = "OK";
            if (result == "OK")
            {
                List<Task<string>> ListsOfTasks = new List<Task<string>>();

                foreach (int id in ids)
                {
                    ListsOfTasks.Add(_Remomve(id));
                }

                string response = "";
                IEnumerable<string> res = await Task.WhenAll<string>(ListsOfTasks);
                if (res.Any())
                {
                    if (res.Where(r => r != "OK").Any())
                    {
                        response = string.Join("; ", res.Where(r => r != "OK"));
                    }
                }
            }
        }

        private async Task<string> _Remomve(int id)
        {
            using (var client = new HttpClient())
            {
                string url = Secrets.ApiAddress + $"Delete{ObjectName}?token=" + Secrets.TenantToken + "&id={0}&UserId={1}";
                var result = await client.DeleteAsync(String.Format(url, id, RuntimeSettings.UserId));
                if (!result.IsSuccessStatusCode)
                {
                    return String.Format("Serwer zwrócił błąd przy próbie usunięcia pozycji {0}. Wiadomość: " + result.ReasonPhrase, id);
                }
                return "OK";
            }
        }

        public async Task CreateLocalBackup()
        {
            var db = new SQLiteConnection(Static.RuntimeSettings.LocalDbPath);
            //db.Execute("DELETE FROM Part");
            db.CreateTable<T>();
            await this.Reload();
            db.InsertOrReplaceAll(Items);

        }

        public async Task<T> GetByToken(string Token)
        {
            string url = Secrets.ApiAddress + $"Get{ObjectName}?token=" + Secrets.TenantToken + $"&{ObjectName}sToken=" + Token;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    return JsonConvert.DeserializeObject<T>(output);
                }
                else
                {
                    return default(T);
                }

            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetByToken), this.GetType().Name);
                throw;
            }
        }
    }
}
