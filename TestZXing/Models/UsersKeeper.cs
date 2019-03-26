﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using System.Collections.ObjectModel;
using TestZXing.Classes;
using ModernHttpClient;
using Microsoft.AppCenter.Crashes;

namespace TestZXing.Models
{
    public class UsersKeeper
    {
        public List<User> Items { get; set; }

        public UsersKeeper()
        {
            Items = new List<User>();
        }

        public async Task<string> Reload()
        {
            string url = Secrets.ApiAddress + "GetMechanics?token=" + Secrets.TenantToken;
            DataService ds = new DataService();
            string _Result = "OK";
            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (!responseMsg.IsSuccessStatusCode)
                {
                    _Result = responseMsg.ReasonPhrase;
                }
                string output = await ds.readStream(responseMsg);
                Items = JsonConvert.DeserializeObject<List<User>>(output);
            }catch(Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.Reload)},
                    {"Class", this.GetType().Name}
                };
                Crashes.TrackError(ex, properties);
                _Result = ex.Message;
            }
            

            return _Result;
        }

        public async Task<User> GetUser(int id)
        {
            string url = Secrets.ApiAddress + "GetUser?token=" + Secrets.TenantToken + "&id=" + id.ToString();
            DataService ds = new DataService();
            User nUser = new User();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    nUser = JsonConvert.DeserializeObject<User>(output);
                }
                else
                {
                    nUser = null;
                }

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    {"Type", "No connection"},
                    {"Method",nameof(this.GetUser)},
                    {"Class", this.GetType().Name}
                };
                Crashes.TrackError(ex, properties);
                nUser = null;
                throw;
            }
            return nUser;
        }

    }
}
