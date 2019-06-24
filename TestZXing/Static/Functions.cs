﻿using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.Static
{
    public static class Functions
    {
        public static DateTime NextShift(DateTime now)
        {
            //DateTime now = DateTime.Now;
            
            var x = now.ToString("H:mm");
            string[] hs = Regex.Split(x, ":");
            string h = hs[0];
            int H;
            bool parsive = int.TryParse(h, out H);
            if (parsive)
            {
                if(H<14 && H >= 6)
                {
                    //1st shift
                    return new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
                }
                else if(H<22 && H >=14)
                {
                    //2nd shift
                    return new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
                }
                else
                {
                    return DateTime.Today.AddHours(30);
                }
            }
            else
            {
                return now.AddHours(1);
            }
        }

        public static async Task CreateError(Exception ex, string text, string methodName, string className)
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


            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                InternetConnectionStatus = "Pełen internet";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet)
            {
                InternetConnectionStatus = "Ograniczone";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.Local)
            {
                InternetConnectionStatus = "Lokalne";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                InternetConnectionStatus = "Brak";
            }
            else if (Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                InternetConnectionStatus = "Nieznane";
            }
            else
            {
                InternetConnectionStatus = "Tego przypadku powinno nie być... Prawdopodobnie przybyło opcji w  Connectivity.NetworkAcces.. Sprawdź Error.cs";
            }

            if (RuntimeSettings.CurrentUser != null)
            {
                UserName = RuntimeSettings.CurrentUser.FullName;
            }

            var properties = new Dictionary<string, string>
                {
                    {"Type", text},
                    {"Method", methodName},
                    {"Class", className},
                    {"User", UserName},
                    {"Połączenie internetowe", InternetConnectionStatus },
                    {"Aktywne połączenia", ActiveConnections }

                };

            WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi(true);
            if(wi != null)
            {
                properties.Add("Sieć Wifi", wi.SSID);
                properties.Add("Sygnał Wifi", wi.Signal.ToString());
            }

            Crashes.TrackError(ex, properties);
        }
    }
}
