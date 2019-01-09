﻿using ModernHttpClient;
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
    public class ProcessKeeper
    {
        public List<Process> Items { get; set; }

        public ProcessKeeper()
        {
            Items = new List<Process>();
        }

        public async Task Reload()
        {
            string url = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken;
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Items = JsonConvert.DeserializeObject<List<Process>>(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Process> GetProcess(int id)
        {
            string url = Secrets.ApiAddress + "GetProcess?token=" + Secrets.TenantToken + "&id=" + id.ToString();
            DataService ds = new DataService();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                string output = await ds.readStream(await httpClient.SendAsync(request));
                Process Item = new Process();
                Item = JsonConvert.DeserializeObject<Process>(output);
                return Item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Process> GetProcess(string mesId)
        {
            string url = Secrets.ApiAddress + "GetProcess?token=" + Secrets.TenantToken + "&mesId=" + mesId;
            DataService ds = new DataService();
            Process nProcess = new Process();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    nProcess = JsonConvert.DeserializeObject<Process>(output);
                }
                else
                {
                    nProcess = null;
                }

            }
            catch (Exception ex)
            {
                nProcess = null;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetProcess", Time = DateTime.Now, Message = ex.Message };
                throw;
            }
            return nProcess;
        }

        public async Task<Process> GetOpenProcessesOfTypeAndResource(int actionType, int resource)
        {
            string url = Secrets.ApiAddress + "GetProcesses?token=" + Secrets.TenantToken + "query=" + "PlaceId=" + + resource + " and(IsActive=true or IsFrozen=true) and ActionTypeId=" + actionType + " and IsCompleted=false";
            DataService ds = new DataService();
            Process nProcess = new Process();

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    List<Process> nProcesses = new List<Process>();
                    nProcesses = JsonConvert.DeserializeObject<List<Process>>(output);
                    if (nProcesses.Any())
                    {
                        //there are already some open processes of this type, let's continue one them
                        //by adding new handling to it
                        nProcess = nProcesses.FirstOrDefault();
                    }
                }
                else
                {
                    nProcess = null;
                }

            }
            catch (Exception ex)
            {
                nProcess = null;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "GetProcess", Time = DateTime.Now, Message = ex.Message };
                throw;
            }
            return nProcess;
        }
    }
}
