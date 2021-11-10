using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.Support.V7.App;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using TestZXing.Droid.Services;
using TestZXing.Interfaces;
using TestZXing.Models;
using TestZXing.ViewModels;
using Plugin.CurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(WifiHandler))]
namespace TestZXing.Droid.Services
{
    public class WifiHandler : IWifiHandler
    {
        private Context context = null;
        private NetworkCallback _callback;
        private string _callbackStatus;
        private ConnectivityManager _connectivityManager;
        string preferredWifi = Static.Secrets.PreferredWifi;
        string prefferedWifiPassword = Static.Secrets.PrefferedWifiPassword;


        public WifiHandler()
        {
            this.context = Android.App.Application.Context;
            _connectivityManager = Application.Context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
            _callback = new NetworkCallback
            {
                NetworkAvailable = network =>
                {
                    // we are connected!
                    _callbackStatus = $"Request network connected";
                    //_connectivityManager.BindProcessToNetwork(network);
                    Static.RuntimeSettings.IsWifiRequestingFinished = true;
                },
                NetworkUnavailable = () =>
                {
                    _callbackStatus = $"Request network unavailable";
                    Static.RuntimeSettings.IsWifiRequestingFinished = true;
                }
            };
        }

        public async Task<WiFiInfo> ConnectPreferredWifi()
        {
            WiFiInfo w = null;
            
            var wifiMgr = (WifiManager)context.GetSystemService(Context.WifiService);
            var formattedSsid = $"\"{preferredWifi}\"";
            var formattedPassword = $"\"{prefferedWifiPassword}\"";

            bool wifiEnabled = await SetWifiOn();

            if (wifiEnabled)
            {
                w = await GetConnectedWifi(true);
                if (w == null || w.SSID != formattedSsid)
                {
                    //no wifi is connected or other wifi is connected
                    string bssid = await GetBestBSSID();
                    await RequestNetwork(bssid);
                    w = await GetConnectedWifi(true);
                    throw new NoPreferredConnectionException();
                } 
            }

            return w;
        }

        public async Task<List<WiFiInfo>> GetAvailableWifis(bool? GetSignalStrenth = false)
        {
            List<WiFiInfo> Wifis = null;

            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }

            if(status == PermissionStatus.Granted)
            {
                // Get a handle to the Wifi
                var wifiMgr = (WifiManager)context.GetSystemService(Context.WifiService);
                var wifiReceiver = new WifiReceiver(wifiMgr);

                await Task.Run(() =>
                {
                    // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
                    context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                    Wifis = wifiReceiver.Scan();
                });
            }

            return Wifis;

        }

        public async Task<WiFiInfo> GetConnectedWifi(bool? GetSignalStrength = false)
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }
            if (status == PermissionStatus.Granted)
            {
                WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));
                if (wifiManager != null)
                {
                    WiFiInfo currentWifi = new WiFiInfo();
                    currentWifi.SSID = wifiManager.ConnectionInfo.SSID;
                    currentWifi.BSSID = wifiManager.ConnectionInfo.BSSID;
                    currentWifi.NetworkId = wifiManager.ConnectionInfo.NetworkId;

                    if ((bool)GetSignalStrength)
                    {
                        currentWifi.Signal = wifiManager.ConnectionInfo.Rssi;
                    }
                    currentWifi.IsConnected = true;
                    return currentWifi;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetWifiMacAddress()
        {
            string res = null;

            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }

            if(status == PermissionStatus.Granted)
            {
                WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));
                res = wifiManager.ConnectionInfo.MacAddress;

                string macAddress = string.Empty;

                var all = Collections.List(Java.Net.NetworkInterface.NetworkInterfaces);

                foreach (var interfaces in all)
                {
                    if (!(interfaces as Java.Net.NetworkInterface).Name.Contains("wlan0")) continue;

                    var macBytes = (interfaces as
                    Java.Net.NetworkInterface).GetHardwareAddress();
                    if (macBytes == null) continue;

                    var sb = new System.Text.StringBuilder();
                    foreach (var b in macBytes)
                    {
                        string convertedByte = string.Empty;
                        convertedByte = (b & 0xFF).ToString("X2") + ":";

                        if (convertedByte.Length == 1)
                        {
                            convertedByte.Insert(0, "0");
                        }
                        sb.Append(convertedByte);
                    }

                    macAddress = sb.ToString().Remove(sb.Length - 1);

                    return macAddress;
                }
                return "02:00:00:00:00:00";
            }

            return res;
        }

        public bool IsWifiConnected()
        {
            var wifiMgr = (WifiManager)context.GetSystemService(Context.WifiService);
            if (wifiMgr.IsWifiEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<bool> PingHost(string host = null, int timeout=3)
        {
            bool pingable = false;
        
            if (host == null)
            {
                host = Static.Secrets.ServerIp;
            }

            Runtime runtime = Runtime.GetRuntime();
            Java.Lang.Process process = runtime.Exec($"ping -c 1 -W {timeout} {host}");
            int result = process.WaitFor();
            if (result == 0)
            {
                pingable = true;
            }

            return Task.FromResult(pingable);
        }

        public async Task<bool> SetWifiOn()
        {
            var wifiMgr = (WifiManager)context.GetSystemService(Context.WifiService);
            if (!wifiMgr.IsWifiEnabled)
            {
                Intent intent = new Intent(Android.Provider.Settings.Panel.ActionWifi);
                CrossCurrentActivity.Current.Activity.StartActivityForResult(intent, 100);
                //await result
                while (!Static.RuntimeSettings.IsWifiEnablingFinished)
                {

                }
                Static.RuntimeSettings.IsWifiRequestingFinished = false; // <== reset value
                if (!wifiMgr.IsWifiEnabled)
                {
                    //user aborted wifi enabling
                    return false;
                }
            }
            return true;
        }

        class WifiReceiver : BroadcastReceiver
        {
            private WifiManager wifi;
            private List<WiFiInfo> wiFiInfos;
            private List<string> wifiNetworks;
            private AutoResetEvent receiverARE;
            private System.Threading.Timer tmr;
            private const int TIMEOUT_MILLIS = 20000; // 20 seconds timeout
            string connectedSSID;

            public WifiReceiver(WifiManager wifi)
            {
                this.wifi = wifi;
                wifiNetworks = new List<string>();
                wiFiInfos = new List<WiFiInfo>();
                receiverARE = new AutoResetEvent(false);

                //connectedSSID = ((WifiManager)wifi).ConnectionInfo.SSID.Replace("\"","");
                connectedSSID = ((WifiManager)wifi).ConnectionInfo.BSSID;
            }

            public List<WiFiInfo> Scan()
            {
                tmr = new System.Threading.Timer(Timeout, null, TIMEOUT_MILLIS, System.Threading.Timeout.Infinite);
                wifi.StartScan();
                receiverARE.WaitOne();
                return wiFiInfos;
            }


            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanwifinetworks = wifi.ScanResults;
                foreach (ScanResult wifinetwork in scanwifinetworks)
                {
                    bool isConnected = false;
                    if(wifinetwork.Bssid == connectedSSID)
                    {
                        isConnected = true;
                    }
                    WiFiInfo nWF = new WiFiInfo { SSID = wifinetwork.Ssid, BSSID=wifinetwork.Bssid, Signal = wifinetwork.Level, IsConnected=isConnected};
                    wiFiInfos.Add(nWF);
                    //wifiNetworks.Add(wifinetwork.Ssid);
                }

                receiverARE.Set();
            }

            private void Timeout(object sender)
            {
                // NOTE release scan, which we are using now, or we throw an error?
                receiverARE.Set();
            }
        }

        public async Task SuggestNetwork()
        {
            var suggestion = new WifiNetworkSuggestion.Builder()
                .SetSsid(Static.Secrets.PreferredWifi)
                .SetWpa2Passphrase(Static.Secrets.PrefferedWifiPassword)
                .Build();

            var suggestions = new[] { suggestion };

            var wifiManager = Application.Context.GetSystemService(Context.WifiService) as WifiManager;
            var status = wifiManager.AddNetworkSuggestions(suggestions);

            var statusText = status switch
            {
                NetworkStatus.SuggestionsSuccess => "Suggestion Success",
                NetworkStatus.SuggestionsErrorAddDuplicate => "Suggestion Duplicate Added",
                NetworkStatus.SuggestionsErrorAddExceedsMaxPerApp => "Suggestion Exceeds Max Per App"
            };

            string _status = statusText;
        }

        private bool _requested;
        public async Task RequestNetwork(string bssid = null)
        {
            WifiNetworkSpecifier specifier;
            
            if(bssid != null)
            {
                specifier = new WifiNetworkSpecifier.Builder()
                .SetSsid(Static.Secrets.PreferredWifi)
                .SetBssid(MacAddress.FromString(bssid))
                .SetWpa2Passphrase(Static.Secrets.PrefferedWifiPassword)
                .Build();
            }
            else
            {
                specifier = new WifiNetworkSpecifier.Builder()
                .SetSsid(Static.Secrets.PreferredWifi)
                .SetWpa2Passphrase(Static.Secrets.PrefferedWifiPassword)
                .Build();

            }

            var request = new NetworkRequest.Builder()
                .AddTransportType(TransportType.Wifi)
                .RemoveCapability(NetCapability.Internet)
                .SetNetworkSpecifier(specifier)
                .Build();


            if (_requested)
            {
                _connectivityManager.UnregisterNetworkCallback(_callback);
            }

            _connectivityManager.RequestNetwork(request, _callback, 15000);
            _requested = true;
            DateTime startedAt = DateTime.Now;
            while (!Static.RuntimeSettings.IsWifiRequestingFinished)
            {
                //wait for any action from the user
                //but if we hit 15s of timeout, stop
                if((DateTime.Now-startedAt).TotalSeconds >= 15)
                {
                    break;
                }
            }
            Static.RuntimeSettings.IsWifiRequestingFinished = false; // <== reset value
            
        }

        public async Task<string> GetBestBSSID()
        {
            string res = null;

            List<WiFiInfo> wis = await GetAvailableWifis(true);
            if (wis == null)
            {
                Toast.MakeText(context, "Odmowa uprawnienia do lokalizacji oznacza każdorazowe ręczne potwierdzenie połączenia..", ToastLength.Short).Show();
            }
            else
            {
                string status = "";

                res = wis.Where(i => i.SSID == Static.Secrets.PreferredWifi).OrderByDescending(i => i.Signal).FirstOrDefault().BSSID;

            }
            return res;
        }

        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            public Action<Network> NetworkAvailable { get; set; }
            public Action NetworkUnavailable { get; set; }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                NetworkAvailable?.Invoke(network);

            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
                NetworkUnavailable?.Invoke();
            }
        }
    }
}