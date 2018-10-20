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
    public class ProcessKeeper
    {
        public List<Process> Items { get; set; }

        public ProcessKeeper()
        {
            Items = new List<Process>();
        }

        public async Task Reload()
        {
            string url = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Process>>(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Process> GetProcess(int id)
        {
            string url = Secrets.ApiAddress + "GetProcess?token=" + Secrets.TenantToken + "&id=" + id.ToString();
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Process Item = new Process();
                Item = JsonConvert.DeserializeObject<Process>(output);
                return Item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
