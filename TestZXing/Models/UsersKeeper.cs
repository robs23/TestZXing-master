using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using System.Collections.ObjectModel;
using TestZXing.Classes;
using ModernHttpClient;

namespace TestZXing.Models
{
    public class UsersKeeper
    {
        public List<User> Items { get; set; }

        public UsersKeeper()
        {
            Items = new List<User>();
        }

        public async Task Reload()
        {
            string url = RuntimeSettings.ApiAddress + "GetMechanics?token=" + RuntimeSettings.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<User>>(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
