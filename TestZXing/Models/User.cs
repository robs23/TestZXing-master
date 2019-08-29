using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public bool IsMechanic { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastLoggedOn { get; set; }
        public string MesLogin { get; set; }
        private bool? _IsWorking { get; set; }
        public string FullName { get
            {
                return Name + " " + Surname;
            } }
        public string Icon { get
            {
                if (IsWorking==null)
                {
                    return "circle_yellow.png";
                }
                else if((bool)IsWorking==true)
                {
                    return "circle_green.png";
                }
                else
                {
                    return "circle_red.png";
                }
            } }
        public bool? IsWorking { get
            {
                return _IsWorking;
            } set
            {
                if (value != _IsWorking)
                {
                    _IsWorking = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        public async void Login()
        {
            try
            {
                //App.Current.Properties.Add("UserId", UserId);
                //App.Current.Properties.Add("UserExpirationTime")
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                string url = Secrets.ApiAddress + "LogIn?token=" + Secrets.TenantToken + "&id=" + this.UserId;
                var serializedProduct = JsonConvert.SerializeObject(this);
                var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                var result  = await Static.Functions.GetPostRetryAsync(() => httpClient.PutAsync(new Uri(url), content), TimeSpan.FromSeconds(3));
                DataService ds = new DataService();
                url = Secrets.ApiAddress + "IsUserWorking?token=" + Secrets.TenantToken + "&UserId=" + this.UserId;
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    IsWorking = JsonConvert.DeserializeObject<bool>(output);
                }
                OnPropertyChanged(nameof(IsWorking));
                OnPropertyChanged(nameof(Icon));
            }
            catch(Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Login), this.GetType().Name);
            }
            
        }

        public async Task UpdateStatus()
        {
            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                DataService ds = new DataService();
                string url = Secrets.ApiAddress + "IsUserWorking?token=" + Secrets.TenantToken + "&UserId=" + this.UserId;
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                if (responseMsg.IsSuccessStatusCode)
                {
                    string output = await ds.readStream(responseMsg);
                    IsWorking = JsonConvert.DeserializeObject<bool>(output);
                }
                OnPropertyChanged(nameof(IsWorking));
                OnPropertyChanged(nameof(Icon));
            }
            catch (Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Login), this.GetType().Name);
            }

        }

    }
}
