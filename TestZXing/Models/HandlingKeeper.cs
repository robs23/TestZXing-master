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
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Handling>>(output);
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.Reload)},
                    {"Class", this.GetType().Name},
                    {"User", RuntimeSettings.CurrentUser.FullName}
                };
                Crashes.TrackError(ex, properties);
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
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                _nHandlings = JsonConvert.DeserializeObject<ObservableCollection<Handling>>(output);
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.GetHandligngsByProcess)},
                    {"Class", this.GetType().Name},
                    {"User", RuntimeSettings.CurrentUser.FullName}
                };
                Crashes.TrackError(ex, properties);
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
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.GetUsersOpenHandling)},
                    {"Class", this.GetType().Name},
                    {"User", RuntimeSettings.CurrentUser.FullName}
                };
                Crashes.TrackError(ex, properties);
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
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.CompleteUsersHandlings)},
                    {"Class", this.GetType().Name},
                    {"User", RuntimeSettings.CurrentUser.FullName}
                };
                Crashes.TrackError(ex, properties);
                throw;
            }
            return _Result;
        }
    }
}
