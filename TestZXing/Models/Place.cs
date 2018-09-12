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

    public class Place
    {
        public int PlaceId { get; set; }
        public string Number1 { get; set; }
        public string Number2 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public string Priority { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string PlaceToken { get; set; }
        public List<Process> Processes { get; set; } = new List<Process>();

        public async Task<List<Process>> GetProcesses(bool active = false)
        {
            string url = RuntimeSettings.ApiAddress + "GetProcesses?token=" + RuntimeSettings.TenantToken + "&PlaceId=" + PlaceId.ToString() + "&active=" + active;
            DataService ds = new DataService();
            List<Process> Items = new List<Process>();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                string output = await ds.readStream(responseMsg);
                if (responseMsg.IsSuccessStatusCode)
                {
                    Items = JsonConvert.DeserializeObject<List<Process>>(output);
                }
                return Items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

