using ModernHttpClient;
using Newtonsoft.Json;
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
    public class Error
    {
        public int ErrorId { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
        public int App { get; set; }//0-pc, 1-android
        public string Class { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }

        public async Task Add()
        {
            string url = Secrets.ApiAddress + "CreateError?token=" + Secrets.TenantToken;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serialized = JsonConvert.SerializeObject(this);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                var httpResponse = await httpClient.PostAsync(new Uri(url), content);                    
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
