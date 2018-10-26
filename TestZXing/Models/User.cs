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
    public class User
    {
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
        public string FullName { get
            {
                return Name + " " + Surname;
            } }

        public async void Login()
        {
            try
            {
                //App.Current.Properties.Add("UserId", UserId);
                //App.Current.Properties.Add("UserExpirationTime")
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                string url = Secrets.ApiAddress + "LogIn?token=" + Secrets.TenantToken + "&id=";
                var serializedProduct = JsonConvert.SerializeObject(this);
                var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                var result = await httpClient.PutAsync(String.Format("{0}{1}", new Uri(url), this.UserId), content);
            }catch(Exception ex)
            {
                Error Error = new Error { TenantId = RuntimeSettings.TenantId, UserId = RuntimeSettings.UserId, App = 1, Class = this.GetType().Name, Method = "Login", Time = DateTime.Now, Message = ex.Message };
                await Error.Add();
            }
            
        }
    }
}
