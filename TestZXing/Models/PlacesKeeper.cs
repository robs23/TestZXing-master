using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            string url = Secrets.ApiAddress + "GetPlaces?token=" + Secrets.TenantToken;
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
            string url = Secrets.ApiAddress + "GetPlace?token=" + Secrets.TenantToken + "&placeToken=" + placeToken;
            DataService ds = new DataService();
            Place nPlace = new Place();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    nPlace = JsonConvert.DeserializeObject<Place>(output);
                }
                else
                {
                    nPlace = null;
                }
                
            }
            catch (Exception ex)
            {
                nPlace = null;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetPlace", Time = DateTime.Now, Message = ex.Message };
                throw;
            }
            return nPlace;
        }

        public async Task<List<Place>> GetPlacesBySetName(string name)
        {
            string url = Secrets.ApiAddress + "GetPlacesBySetName?token=" + Secrets.TenantToken + "&name=" + name + "&UserId=" + RuntimeSettings.UserId;
            DataService ds = new DataService();
            List<Place> Places = new List<Place>();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    Places = JsonConvert.DeserializeObject<List<Place>>(output);
                }
                else
                {
                    Places = null;
                }

            }
            catch (Exception ex)
            {
                Places = null;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetPlacesBySetName", Time = DateTime.Now, Message = ex.Message };
                throw;
            }

            return Places;
        }

        public async Task<ObservableCollection<Place>> GetUsersLastPlaces()
        {
            string url = Secrets.ApiAddress + "GetUsersLastPlaces?token=" + Secrets.TenantToken + "&UserId=" + RuntimeSettings.UserId + "&distinct=true";
            DataService ds = new DataService();
            ObservableCollection<Place> Items = new ObservableCollection<Place>();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                string output = await ds.readStream(responseMsg);
                if (responseMsg.IsSuccessStatusCode)
                {
                    Items = JsonConvert.DeserializeObject<ObservableCollection<Place>>(output);
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
