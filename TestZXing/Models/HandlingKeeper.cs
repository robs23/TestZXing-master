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

        public async Task<Handling> GetUsersOpenHandling(int? ProcessId=null)
        {
            string url = Secrets.ApiAddress + "GetHandlings?token=" + Secrets.TenantToken;
            if (ProcessId == null)
            {
                url += "&query=" + $"UserId={RuntimeSettings.UserId} and IsCompleted=false"; // User's open handlings
            }
            else
            {
                url += "&query=" + $"ProcessId={ProcessId} and UserId={RuntimeSettings.UserId} and IsCompleted=false";
            }
            
            DataService ds = new DataService();
            Handling nHandling = null;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    List<Handling> nHandlings = new List<Handling>();
                    nHandlings = JsonConvert.DeserializeObject<List<Handling>>(output);
                    if (nHandlings.Any())
                    {
                        nHandling = nHandlings.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetOpenHandlings", Time = DateTime.Now, Message = ex.Message };
                throw;
            }
            return nHandling;
        }

        public async Task<string> CompleteUsersHandlings()
        {
            string url = Secrets.ApiAddress + "CompleteUsersHandlings?token=" + Secrets.TenantToken + $"&UserId={RuntimeSettings.UserId}";
            string _Result = "OK";
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                }
                else
                {
                    _Result = responseMsg.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "CompleteUsersHandlings", Time = DateTime.Now, Message = ex.Message };
                throw;
            }
            return _Result;
        }
    }
}
