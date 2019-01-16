﻿using ModernHttpClient;
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
    public class Handling
    {
        public int HandlingId { get; set; }
        public int ProcessId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsCompleted { get; set; }
        public string Output { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionTypeName { get; set; }
        public int? Length { get; set; }

        public string Status
        {
            get
            {
                if (IsCompleted)
                {
                    return "Zakończony";
                }
                else if (IsFrozen && !(IsCompleted))
                {
                    return "Wstrzymany";
                }
                else if (IsActive)
                {
                    return "Rozpoczęty";
                }
                else
                {
                    return "Planowany";
                }
            }
            set
            {
                if (value == "Zrealizowany")
                {
                    IsCompleted = false;
                    IsActive = false;
                    IsFrozen = false;
                }
                else if (value == "Zakończony")
                {
                    IsCompleted = true;
                    IsFrozen = false;
                    IsActive = false;
                }
                else if (value == "Wstrzymany")
                {
                    IsCompleted = false;
                    IsFrozen = true;
                    IsActive = false;
                }
                else if (value == "Rozpoczęty")
                {
                    IsCompleted = false;
                    IsFrozen = false;
                    IsActive = true;
                }
                else
                {
                    IsCompleted = false;
                    IsFrozen = false;
                    IsActive = false;
                }
            }
        }

        public async Task<string> Add()
        {
            string url = Secrets.ApiAddress + "CreateHandling?token=" + Secrets.TenantToken + "&UserId=" + RuntimeSettings.UserId;
            string _Result = "OK";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serialized = JsonConvert.SerializeObject(this);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                //var httpResponse = await httpClient.PostAsync(new Uri(url), content);
                var httpResponse = await httpClient.PostAsync(new Uri(url), content);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _Result = httpResponse.ReasonPhrase;
                }
                else
                {
                    var rString = await httpResponse.Content.ReadAsStringAsync();
                    Handling rItem = new Handling();
                    rItem = JsonConvert.DeserializeObject<Handling>(rString);
                    this.HandlingId = rItem.HandlingId;
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Add", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            return _Result;
        }

        public async Task<string> Edit()
        {
            string url = Secrets.ApiAddress + "EditHandling?token=" + Secrets.TenantToken + "&id={0}&UserId={1}";
            string _Result = "OK";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serializedProduct = JsonConvert.SerializeObject(this);
                var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                var result = await httpClient.PutAsync(String.Format(url, this.HandlingId, RuntimeSettings.UserId), content);
                if (!result.IsSuccessStatusCode)
                {
                    _Result = result.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Edit", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            

            return _Result;
        }
    }
}