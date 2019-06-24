using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
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
        public int? SetId { get; set; }
        public string SetName { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public string Output { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string InitialDiagnosis { get; set; }
        public string RepairActions { get; set; }
        public int? Length { get; set; }
        public string Reason { get; set; }
        public string MesId { get; set; }
        public DateTime? MesDate { get; set; }
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
                    return "Planowany";
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
        public ProcessStatus? LastStatus { get; set; }
        public int? LastStatusBy { get; set; }
        public string LastStatusByName { get; set; }
        public DateTime? LastStatusOn { get; set; }
        public int? OpenHandlings { get; set; }
        public int? AllHandlings { get; set; }

        public override string ToString()
        {
            string str = "";
            str += "ProcessId={0}, Description={1}, StartedBy={2}, StartedByName={3}, StartedOn={4}, FinishedBy={5}, FinishedByName={6}, FinishedOn={7}";
            str += ", ActionTypeId={8}, ActionTypeName={9}, Status={10}, PlaceId={11}, PlaceName={12}, Output={13}, CreatedOn={14}";
            str += ", CreatedBy={15}, CreatedByName={16}";
            str = string.Format(str, ProcessId, Description, StartedBy, StartedByName, StartedOn, FinishedBy, FinishedByName, FinishedOn, ActionTypeId, ActionTypeName, Status, PlaceId, PlaceName, Output, CreatedOn, CreatedBy, CreatedByName);
            return str;
        }

        public async Task<string> Add()
        {
            string url = Secrets.ApiAddress + "CreateProcess?token=" + Secrets.TenantToken + "&UserId=" + RuntimeSettings.UserId;
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
                    Process rItem = new Process();
                    rItem = JsonConvert.DeserializeObject<Process>(rString);
                    this.ProcessId = rItem.ProcessId;
                }
            }
            catch (Exception ex)
            {
                _Result = ex.Message;
                Static.Functions.CreateError(ex, "No connection", nameof(this.Add), this.GetType().Name);
            }
            return _Result;
        }

        public async Task<string> Edit()
        {
            string url = Secrets.ApiAddress + "EditProcess?token=" + Secrets.TenantToken + "&id={0}&UserId={1}";
            string _Result = "OK";
            if (!string.IsNullOrEmpty(this.MesId))
            {
                //is it ending request?
                if(this.Status == "Zakończony")
                {
                    //try to update MES only if it has MesId!
                    _Result = await CreateTpmEntry();
                }
            }

            if (_Result == "OK")
            {
                try
                {
                    HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                    var serializedProduct = JsonConvert.SerializeObject(this);
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    var result = await httpClient.PutAsync(String.Format(url, this.ProcessId, RuntimeSettings.UserId), content);
                    if (!result.IsSuccessStatusCode)
                    {
                        _Result = result.ReasonPhrase;
                    }
                }
                catch (Exception ex)
                {
                    _Result = ex.Message;
                    Static.Functions.CreateError(ex, "No connection", nameof(this.Edit), this.GetType().Name);
                }
            }

            return _Result;
        }

        private async Task<string> CreateTpmEntry()
        {
            string url = Secrets.MesApiAddress + "CreateTpmEntry";
            string _Result = "OK";

            UsersKeeper uKeeper = new UsersKeeper();
            User myUser = await uKeeper.GetUser(RuntimeSettings.UserId);
            User manager = await uKeeper.GetUser((int)this.StartedBy);

            if (myUser != null && manager != null)
            {
                if (!string.IsNullOrEmpty(myUser.MesLogin) && !string.IsNullOrEmpty(manager.MesLogin))
                {
                    string adjustment = "N";
                    if (this.ActionTypeName == "Regulacja")
                    {
                        adjustment = "T";
                    }
                    TpmEntry tpm = new TpmEntry()
                    {
                        Number = this.MesId,
                        Manager = manager.MesLogin,
                        FinishedBy = myUser.MesLogin,
                        StartDate = (DateTime)this.StartedOn,
                        EndDate = (DateTime)this.FinishedOn,
                        InitialDiagnosis = this.InitialDiagnosis,
                        RepairActions = this.RepairActions,
                        Status = "AC",
                        IsAdjustment = adjustment
                    };
                    try
                    {
                        HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 5), EnableUntrustedCertificates = true, DisableCaching = true });
                        var serializedProduct = JsonConvert.SerializeObject(tpm);
                        var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                        var result = await httpClient.PostAsync(url, content);
                        if (!result.IsSuccessStatusCode)
                        {
                            _Result = result.ReasonPhrase;
                        }
                    }
                    catch (Exception ex)
                    {
                        _Result = "Nie udało się połączyć z serwerem MES. Upewnij się, że masz połączenie Wi-fi z siecią lokalną, inne sieci nie mają dostęu do serwera MES.";
                        Static.Functions.CreateError(ex, "No MES connection", nameof(this.CreateTpmEntry), this.GetType().Name);
                    }
                }
                else
                {
                    _Result = "Użytkownik rozpoczynający lub użytkownik kończący nie ma przypisanego loginu MES! Oba loginy muszą być przypisane by kontynuować!";
                }
                
            }
            else
            {
                _Result = "Nie można połączyć się z bazą danych by pobrać loginy użytkowników! Sprawdź swoje połączenie internetowe i spóbuj jeszcze raz";     
            }
            return _Result;
        }

        public async Task<List<Handling>> GetOpenHandlings()
        {
            string url = Secrets.ApiAddress + "GetHandlings?token=" + Secrets.TenantToken + "&query=" + $"ProcessId={ProcessId} and IsCompleted=false";
            DataService ds = new DataService();
            List<Handling> nHandlings = null;

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await httpClient.SendAsync(request);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    nHandlings = new List<Handling>();
                    nHandlings = JsonConvert.DeserializeObject<List<Handling>>(output);
                }
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetOpenHandlings), this.GetType().Name);
                throw;
            }
            return nHandlings;
        }
    }

    public enum ProcessStatus
    {
        Brak,
        Planowany,
        Rozpoczęty,
        Wstrzymany,
        Wznowiony,
        Zakończony
    }
}
