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
    public class ActionTypesKeeper
    {
        public List<ActionType> Items { get; set; }

        public ActionTypesKeeper()
        {
            Items = new List<ActionType>();
        }

        public async Task Reload()
        {
            string url = Secrets.ApiAddress + "GetActionTypes?token=" + Secrets.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<ActionType>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
                throw;
            }
        }

        public async Task<ActionType> GetActionType(int id)
        {
            string url = Secrets.ApiAddress + "GetActionType?token=" + Secrets.TenantToken + "&id=" + id.ToString();
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                ActionType Item = new ActionType();
                Item = JsonConvert.DeserializeObject<ActionType>(output);
                return Item;
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetActionType), this.GetType().Name);
                throw;
            }
        }

        public async Task<ActionType> GetActionTypeByName(string name)
        {
            string url = Secrets.ApiAddress + "GetActionTypeByName?token=" + Secrets.TenantToken + "&name=" + name + "&UserId=" + RuntimeSettings.UserId;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                ActionType Item = new ActionType();
                Item = JsonConvert.DeserializeObject<ActionType>(output);
                return Item;
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetActionTypeByName), this.GetType().Name);
                throw;
            }
        }
    }
}
