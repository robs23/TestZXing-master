using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class SetKeeper
    {
        public List<Set> Items { get; set; }

        public SetKeeper()
        {
            Items = new List<Set>();
        }

        public async Task Reload()
        {
            string url = Secrets.ApiAddress + "GetSets?token=" + Secrets.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Set>>(output);
            }
            catch (Exception ex)
            {
                Error nError = new Error(ex, "No connection", nameof(this.Reload), this.GetType().Name);
                throw;
            }
        }
    }
}
