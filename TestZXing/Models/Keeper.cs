using ModernHttpClient;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Interfaces;
using TestZXing.Static;

namespace TestZXing.Models
{
    public abstract class Keeper<T> where T: Entity<T>, new()
    {
        public ObservableCollection<T> Items { get; set; }
        protected abstract string ObjectName { get; }
        protected abstract string PluralizedObjectName { get; }
        public string FilterString { get; set; } = null;
        public string QueryString { get; set; } = null;

        protected virtual string ArchiveString { get; set; } = null;
        public bool IsWorking { get; set; } = false;

        public bool IsOfflineKeeper { get; set; } = false;
        public bool IsSynced { get; set; }
        public string TableName 
        {
            get
            {
                return typeof(T).Name;
            }
        }

        public Keeper()
        {
            Items = new ObservableCollection<T>();
        }

        public async Task Reload(string query = null, int? page = null, int? pageSize=null)
        {
            string url = Secrets.ApiAddress + $"Get{PluralizedObjectName}?token=" + Secrets.TenantToken;
            DataService ds = new DataService();
            if (this.ArchiveString != null)
            {
                if (!this.QueryString.ContainsNullSafe("IsArchived") && !this.FilterString.ContainsNullSafe("IsArchived") && !query.ContainsNullSafe("IsArchived"))
                {
                    if (!string.IsNullOrEmpty(this.FilterString))
                    {
                        FilterString += " AND " + ArchiveString;
                    }
                    else
                    {
                        FilterString = ArchiveString;
                    }
                }
            }
            if (page != null){ url += $"&page={page}"; }
            if (pageSize != null) { url += $"&pageSize={pageSize}"; }
            if (query != null)
            {
                QueryString = query;
                url += "&query=" + query;
                if (this.FilterString != null)
                {
                    url += "AND " + this.FilterString;
                }
            }
            else
            {

                url += "&query=" + this.FilterString;
            }

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

        public async Task AddToSyncQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            db.CreateTable<T>();
            if (Items.Any(i => i.IsSynced == false && i.IsSyncing == false))
            {
                //db.InsertOrReplaceAll(Items.Where(i => i.IsSynced == false && i.IsSyncing == false));
                db.InsertAll(Items.Where(i => i.IsSynced == false && i.IsSyncing == false));
            }

        }

        public async Task RestoreSyncQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            Items = new ObservableCollection<T>(db.Table<T>());
        }

        public async Task Sync()
        {
            try
            {
                IsWorking = true;
                if (Items.Any())
                {
                    foreach (T i in Items)
                    {
                        i.IsSyncing = true;
                        string res;
                        if (i.Id > 0)
                        {
                            res = await i.Edit();
                        }
                        else
                        {

                            res = await i.Add();
                            
                        }

                        i.IsSyncing = false;
                        if (res == "OK")
                        {
                            i.IsSynced = true;
                        }
                        else
                        {
                            i.IsSynced = false;
                            i.SyncFailed = true;
                        }
                    }
                    await DeleteSynced();
                }
            }
            catch (Exception ex)
            {

            }
            IsWorking = false;

        }

        public async Task Sync(string args)
        {
            try
            {
                IsWorking = true;
                if (Items.Any())
                {
                    foreach (T i in Items)
                    {
                        i.IsSyncing = true;
                        string res;
                        if(i.Id > 0)
                        {
                            res = await i.Edit();
                        }
                        else
                        {
                            res = await i.Add(args: args);
                        }
                        
                        i.IsSyncing = false;
                        if (res == "OK")
                        {
                            i.IsSynced = true;
                        }
                        else
                        {
                            i.IsSynced = false;
                            i.SyncFailed = true;
                        }
                    }
                    await DeleteSynced();
                }
            }
            catch (Exception ex)
            {

            }
            IsWorking = false;

        }

        public async Task DeleteSynced()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            foreach (T i in Items.Where(i => i.IsSynced == true).ToList())
            {
                db.Delete<T>(i.SqliteId);
                Items.Remove(i);
            }
            db.Close();
        }

        public async Task DeleteFromSyncQueue()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);

            db.DeleteAll<T>();
            db.Close();
        }

        public async Task DeleteTable()
        {
            var db = new SQLiteConnection(RuntimeSettings.LocalDbPath);
            db.DropTable<T>();
            db.Close();
        }

        public async Task<string> IsDependentOn()
        {
            string res = null;
            var dict = new Dictionary<MemberInfo, bool>();

            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property);

            foreach (MemberInfo member in members)
            {
                var attr = member.GetCustomAttribute<ForeignKeyAttribute>(true);

                if (attr != null)
                {
                    dict.Add(member, attr.ForeignKey);
                    res = attr.Table;
                }
            }
            return res;
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

        public async Task<T> GetById(int id)
        {
            string url = Secrets.ApiAddress + $"Get{ObjectName}?token=" + Secrets.TenantToken + $"&id=" + id;
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
