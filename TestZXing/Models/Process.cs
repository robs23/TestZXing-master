using ModernHttpClient;
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
    public class Process
    {
        public int ProcessId { get; set; }
        public string Description { get; set; }
        public DateTime? StartedOn { get; set; }
        public int? StartedBy { get; set; }
        public string StartedByName { get; set; }
        public DateTime? FinishedOn { get; set; }
        public int? FinishedBy { get; set; }
        public string FinishedByName { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccessfull { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string Output { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Status
        {
            get
            {
                if (IsSuccessfull)
                {
                    return "Zrealizowany";
                }
                else if (IsCompleted)
                {
                    return "Zakończony";
                }
                else if (IsFrozen && !(IsSuccessfull || IsCompleted))
                {
                    return "Wstrzymany";
                }
                else if (IsActive)
                {
                    return "Rozpoczęty";
                }
                else
                {
                    return "Nierozpoczęty";
                }
            }
            set
            {
                if (value == "Zrealizowany")
                {
                    IsSuccessfull = true;
                    IsCompleted = false;
                    IsActive = false;
                    IsFrozen = false;
                }
                else if (value == "Zakończony")
                {
                    IsSuccessfull = false;
                    IsCompleted = true;
                    IsFrozen = false;
                    IsActive = false;
                }
                else if (value == "Wstrzymany")
                {
                    IsSuccessfull = false;
                    IsCompleted = false;
                    IsFrozen = true;
                    IsActive = false;
                }
                else if (value == "Rozpoczęty")
                {
                    IsSuccessfull = false;
                    IsCompleted = false;
                    IsFrozen = false;
                    IsActive = true;
                }
                else
                {
                    IsSuccessfull = false;
                    IsCompleted = false;
                    IsFrozen = false;
                    IsActive = false;
                }
            }
        }
        public override string ToString()
        {
            string str = "";
            str += "ProcessId={0}, Description={1}, StartedBy={2}, StartedByName={3}, StartedOn={4}, FinishedBy={5}, FinishedByName={6}, FinishedOn={7}";
            str += ", ActionTypeId={8}, ActionTypeName={9}, Status={10}, PlaceId={11}, PlaceName={12}, Output={13}, CreatedOn={14}";
            str += ", CreatedBy={15}, CreatedByName={16}";
            str = string.Format(str, ProcessId, Description, StartedBy, StartedByName, StartedOn, FinishedBy, FinishedByName, FinishedOn, ActionTypeId, ActionTypeName, Status, PlaceId, PlaceName, Output, CreatedOn, CreatedBy, CreatedByName);
            return str;
        }

        public async Task Add()
        {
            string url = RuntimeSettings.ApiAddress + "CreateProcess?token=" + RuntimeSettings.TenantToken;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serialized = JsonConvert.SerializeObject(this);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                var httpResponse = await httpClient.PostAsync(new Uri(url), content);
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Add", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
                throw;
            }
            finally
            {

            }
        }

        public async Task Edit()
        {
            string url = RuntimeSettings.ApiAddress + "EditProcess?token=" + RuntimeSettings.TenantToken + "&id=";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var serializedProduct = JsonConvert.SerializeObject(this);
                var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                var result = await httpClient.PutAsync(String.Format("{0}{1}", new Uri(url), this.ProcessId), content);
            }
            catch (Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Edit", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }

        }
    }
}
