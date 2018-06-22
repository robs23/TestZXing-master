using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class ErrorsKeeper
    {
        public List<Error> Items { get; set; }

        public async Task Save()
        {
            string url = RuntimeSettings.ApiAddress + "CreateError?token=" + RuntimeSettings.TenantToken;
            Error NewError = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Save", Time = DateTime.Now };

            var ItemsSerialized = JsonConvert.SerializeObject(Items);

            NewError.Message = ItemsSerialized;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serialized = JsonConvert.SerializeObject(NewError);
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
