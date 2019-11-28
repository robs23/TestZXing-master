using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
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
        public List<T> Items { get; set; }
        protected abstract string ObjectName { get; }
        protected abstract string PluralizedObjectName { get; }

        public Keeper()
        {
            Items = new List<T>();
        }

        public async Task Reload(string query=null)
        {
            string url = Secrets.ApiAddress + $"Get{PluralizedObjectName}?token=" + Secrets.TenantToken;
            DataService ds = new DataService();
            if (query != null)
            {
                url += $"&query={query}";
            }
            

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                Items = JsonConvert.DeserializeObject<List<T>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
                throw;
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
    }
}
