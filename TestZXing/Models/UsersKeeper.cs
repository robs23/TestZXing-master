using Newtonsoft.Json;
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
using Xamarin.Forms;

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
                var responseMsg =  await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
                if (!responseMsg.IsSuccessStatusCode)
                {
                    _Result = responseMsg.ReasonPhrase;
                }
                string output = await ds.readStream(responseMsg);
                Items = JsonConvert.DeserializeObject<List<User>>(output);
            }catch(Exception ex)
            {
                Static.Functions.CreateError(ex, "No connection", nameof(this.Reload), this.GetType().Name);
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
                var responseMsg = await Static.Functions.GetPostRetryAsync(() => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)), TimeSpan.FromSeconds(3));
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
                Static.Functions.CreateError(ex, "No connection", nameof(this.GetUser), this.GetType().Name);
                nUser = null;
                throw;
            }
            return nUser;
        }

        public async Task SaveUserCredentials(User user)
        {
            Application.Current.Properties["RememberedUser"] = JsonConvert.SerializeObject(user);
            Application.Current.Properties["UserRememberedAt"] = DateTime.Now.ToLongDateString();
        }

        public User GetUserFromCredentials()
        {
            User User = null;
            if (Application.Current.Properties.ContainsKey("RememberedUser"))
            {
                if (!string.IsNullOrWhiteSpace(Application.Current.Properties["RememberedUser"].ToString()))
                {
                    User = JsonConvert.DeserializeObject<User>(Application.Current.Properties["RememberedUser"].ToString());
                }
            }
            
            return User;
        }

    }
}
