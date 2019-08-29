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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                Items = JsonConvert.DeserializeObject<List<Handling>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
                throw;
            }
        }

        public async Task<ObservableCollection<Handling>> GetHandligngsByProcess(int processId)
        {
            string url = Secrets.ApiAddress + "GetHandlings?token=" + Secrets.TenantToken + $"&query=ProcessId={processId}";
            DataService ds = new DataService();
            ObservableCollection<Handling> _nHandlings = null;
            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                _nHandlings = JsonConvert.DeserializeObject<ObservableCollection<Handling>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetHandligngsByProcess), this.GetType().Name);
                throw;
            }
            return _nHandlings;

        }

        public async Task<ObservableCollection<Handling>> GetUserHandlings()
        {
            string url = Secrets.ApiAddress + "GetUserHandlings?token=" + Secrets.TenantToken + $"&UserId={RuntimeSettings.CurrentUser.UserId}&page=1&PageSize=30";
            DataService ds = new DataService();
            ObservableCollection<Handling> _nHandlings = null;
            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                string output = await ds.readStream(responseMsg);
                _nHandlings = JsonConvert.DeserializeObject<ObservableCollection<Handling>>(output);
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetHandligngsByProcess), this.GetType().Name);
                throw;
            }
            return _nHandlings;

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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
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
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetUsersOpenHandling), this.GetType().Name);
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
                HttpResponseMessage responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
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
                Static.Functions.CreateError(ex, "No connection", nameof(this.CompleteUsersHandlings), this.GetType().Name);
                throw;
            }
            return _Result;
        }
    }
}
