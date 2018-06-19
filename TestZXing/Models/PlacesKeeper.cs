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
    public class PlacesKeeper
    {
        public List<Place> Items { get; set; }

        public PlacesKeeper()
        {
            Items = new List<Place>();
        }

        public async Task Reload()
        {
            string url = RuntimeSettings.ApiAddress + "GetPlaces?token=" + RuntimeSettings.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Place>>(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Place> GetPlace(string placeToken)
        {
            string url = RuntimeSettings.ApiAddress + "GetPlace?token=" + RuntimeSettings.TenantToken + "&placeToken=" + placeToken;
            DataService ds = new DataService();
            Place nPlace = new Place();
            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                nPlace = JsonConvert.DeserializeObject<Place>(output);
            }
            catch (Exception ex)
            {
                nPlace = null;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetPlace", Time = DateTime.Now, Message = ex.Message };
            }
            return nPlace;
        }
    }
}
