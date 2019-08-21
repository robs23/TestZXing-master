using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TestZXing.CustomExceptions;
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

            string macAddress = await DependencyService.Get<IWifiHandler>().GetWifiMacAddress();

            var ApiPing = await DependencyService.Get<IWifiHandler>().PingHost();
            string pingStatus = $"{Static.Secrets.ServerIp} : ";
            if (ApiPing) { pingStatus += "Dostępny"; } else { pingStatus += "Niedostępny"; }

            WiFiInfo wi = await DependencyService.Get<IWifiHandler>().GetConnectedWifi();

            var properties = new Dictionary<string, string>
                {
                    {"Type", text},
                    {"Method", methodName},
                    {"Class", className},
                    {"User", UserName},
                    {"Połączenie internetowe", InternetConnectionStatus },
                    {"Aktywne połączenia", ActiveConnections },
                    {"SSID (BSSID) aktywnej sieci", wi.SSID + $"({wi.BSSID})" },
                    {"Adres MAC", macAddress },
                    {"Status pingu", pingStatus}
                };

            

            List<WiFiInfo> wis = await DependencyService.Get<IWifiHandler>().GetAvailableWifis(true);
            if(wis != null)
            {
                string[] status = new string[] { "" };

                foreach (WiFiInfo w in wis.OrderByDescending(i => i.Signal))
                {
                    string con = "";
                    string bssid = "";
                    if (w.IsConnected)
                    {
                        con = "(P)";
                        bssid = $" [{w.BSSID}]";
                    }
                    if(status[status.Length-1].Length + bssid.Length + w.SSID.Length + con.Length +5 > 125)
                    {
                        //create new array's item
                        Array.Resize(ref status, status.Length +1);
                    }
                    status[status.Length - 1] += w.SSID + $"{bssid} ({w.Signal}){con}, ";

                }
                for (int i = 0; i < status.Length; i++)
                {
                    properties.Add($"Status Wifi {i}", status[i]);
                }                
            }
            
            Crashes.TrackError(ex, properties);
        }

        public static async Task<HttpResponseMessage> GetPostRetryAsync(Func<Task<HttpResponseMessage>> action, TimeSpan sleepPeriod, int tryCount = 3)
        {
            int attempted = 0;
            bool pingable = false;
            CancellationTokenSource PingCts;
            CancellationTokenSource actionCts;
            HttpResponseMessage res = new HttpResponseMessage();
            Exception exc;
            Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Starting");

            if (tryCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(tryCount));

            while (true)
            {
                try
                {

                    attempted++;
                    Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Attempt "+attempted);
                    if (attempted > 1)
                    {
                        DependencyService.Get<IToaster>().LongAlert($"Próba {attempted}");
                    }
                    Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Checking preferred network");
                    WiFiInfo w = await DependencyService.Get<IWifiHandler>().ConnectPreferredWifi();
                    var formattedSsid = $"\"{Static.Secrets.PreferredWifi}\"";
                    if (w.SSID == formattedSsid)
                    {
                        tryCount = 1;
                    }
                    Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Pinging google to start");
                    PingCts = new CancellationTokenSource();
                    var ping = Task.Run(() => DependencyService.Get<IWifiHandler>().PingHost(),PingCts.Token);
                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Pinging google started. Id={ping.Id}, Status = {ping.Status}");
                    Debug.WriteLine(DateTime.Now + " 'JDE_Scan' - Http action to start");
                    actionCts = new CancellationTokenSource();
                    var resTask = Task.Run(() => action(),actionCts.Token);
                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Http action started. Id={resTask.Id}, Status = {resTask.Status}");
                    
                    Task firstFinieshed = await Task.WhenAny(ping, resTask);
                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - First action finished (Id={firstFinieshed.Id},Status={firstFinieshed.Status}). Ping status={ping.Status}, http status={resTask.Status}");
                    await Task.Delay(1000);
                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - . Ping status={ping.Status}, http status={resTask.Status}");
                    if (ping.Status == TaskStatus.RanToCompletion)
                    {
                        pingable = await ping;
                        Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Google is {pingable} ");
                        if (!pingable)
                        {
                            Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Cancelling actionCts ");
                            actionCts.Cancel();
                            Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - ActionCts cancelled");
                            Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Throwing ServerUnreachableException");
                            exc = new ServerUnreachableException();
                            throw exc;
                        }
                        else
                        {
                            res = await resTask;
                        }
                    }
                    else
                    {
                        Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Cancelling PingCts ");
                        PingCts.Cancel();
                        Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - PingCts cancelled");
                        res = await resTask;
                        Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - res={res.IsSuccessStatusCode}");
                    }


                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Exiting. Is success status code?={res.IsSuccessStatusCode}");
                    return res ; // success!
                }
                catch(Exception ex)
                {
                    --tryCount;
                    if (tryCount <= 0)
                    {
                        throw;
                    }
                    Debug.WriteLine(DateTime.Now + $" 'JDE_Scan' - Going sleep");
                    await Task.Delay(sleepPeriod);
                }
            }
        }


    }
}
