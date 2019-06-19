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
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class Error
    {
        public List<WiFiInfo> AvailableWifi { get; set; }
        public Error(Exception ex, string text, string methodName, string className)
        {
            string UserName = string.Empty;
            string InternetConnectionStatus = "";
            string ActiveConnections = "";

            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                ActiveConnections += "GSM ";
            }
            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
            {
                ActiveConnections += "WiFi ";
            }

            if(RuntimeSettings.CurrentUser != null)
            {
                UserName = RuntimeSettings.CurrentUser.FullName;
            }
            if(Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                InternetConnectionStatus = "Pełen internet";
            }else if(Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet)
            {
                InternetConnectionStatus = "Ograniczone";
            }else if(Connectivity.NetworkAccess == NetworkAccess.Local)
            {
                InternetConnectionStatus = "Lokalne";
            }else if(Connectivity.NetworkAccess == NetworkAccess.None)
            {
                InternetConnectionStatus = "Brak";
            }else if(Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                InternetConnectionStatus = "Nieznane";
            }
            else
            {
                InternetConnectionStatus = "Tego przypadku powinno nie być... Prawdopodobnie przybyło opcji w  Connectivity.NetworkAcces.. Sprawdź Error.cs";
            }

            WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi(true);


            var properties = new Dictionary<string, string>
                {
                    {"Type", text},
                    {"Method", methodName},
                    {"Class", className},
                    {"User", UserName},
                    {"Połączenie internetowe", InternetConnectionStatus },
                    {"Aktywne połączenia", ActiveConnections }

                };
            Crashes.TrackError(ex, properties);
        }
    }

    public class WiFiInfo
    {
        public string SSID { get; set; }
        public int Signal { get; set; }
        public bool IsConnected { get; set; }
    }
}
