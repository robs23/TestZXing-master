using Microsoft.AppCenter.Crashes;
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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                Items = JsonConvert.DeserializeObject<List<Place>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
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
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetPlace), this.GetType().Name);
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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
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
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetPlacesBySetName), this.GetType().Name);
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
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 10), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                if (responseMsg.IsSuccessStatusCode)
                {
                    Items = JsonConvert.DeserializeObject<ObservableCollection<Place>>(output);
                }
                return Items;
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetUsersLastPlaces), this.GetType().Name);
                throw;
            }
        }
    }
}
