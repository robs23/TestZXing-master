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
    public class HandlingKeeper
    {
        public List<Handling> Items { get; set; }

        public HandlingKeeper()
        {
            Items = new List<Handling>();
        }

        public async Task Reload()
        {
            string url = Secrets.ApiAddress + "GetHandlings?token=" + Secrets.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Handling>>(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
