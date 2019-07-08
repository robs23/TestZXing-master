using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using TestZXing.Droid.Services;
using TestZXing.Interfaces;
using TestZXing.Models;

[assembly: Xamarin.Forms.Dependency(typeof(WifiHandler))]
namespace TestZXing.Droid.Services
{
    public class WifiHandler : IWifiHandler
    {
        private Context context = null;
        string PreferredWifi = "Pakowanie1";
        string PrefferedWifiPassword = "gienas1980";

        public WifiHandler()
        {
            this.context = Android.App.Application.Context;
        }

        public async Task<(bool,string)> ConnectPreferredWifi()
        {
            var res = (true, "Ok");
            List<WiFiInfo> wis = await GetAvailableWifis();
            WiFiInfo w;
            
            if (wis.Any())
            {
                if (!wis.Where(i => i.SSID == PreferredWifi).Any())
                {
                    res = (false, $"Jesteś poza zasięgiem sieci {PreferredWifi}");
                }
                else
                {
                    if(!wis.Where(i=>i.SSID==PreferredWifi && i.Signal >= -73).Any())
                    {
                        res = (false, $"Zasięg sieci {PreferredWifi} jest zbyt słaby by nawiązać stabilne połączenie..");
                    }
                    else
                    {
                        //if we got this far, we should be able to connect successfully
                        w = wis.Where(i => i.SSID == PreferredWifi).OrderByDescending(i => i.Signal).FirstOrDefault();
                        if (w != null)
                        {
                            var wifiMgr = (WifiManager)context.GetSystemService(Context.WifiService);
                            var formattedSsid = $"\"{w.SSID}\"";
                            var formattedPassword = $"\"{PrefferedWifiPassword}\"";
                            var formattedBssid = w.BSSID;

                            //if (wifiMgr.ConfiguredNetworks.Any())
                            //{
                            //    if (wifiMgr.ConfiguredNetworks.Any(i => i.Ssid == formattedSsid))
                            //    {
                            //        foreach(var net in wifiMgr.ConfiguredNetworks.Where(i => i.Ssid == formattedSsid))
                            //        {
                            //            int netId = net.NetworkId;
                            //            bool isRemoved = wifiMgr.RemoveNetwork(netId);
                            //            wifiMgr.SaveConfiguration();
                            //        }
                                    
                            //    }
                            //}
                            var wifiConfig = new WifiConfiguration
                            {
                                Ssid = formattedSsid,
                                PreSharedKey = formattedPassword,
                                Bssid = formattedBssid
                            };
                            var addNetwork = wifiMgr.AddNetwork(wifiConfig);
                            var network = wifiMgr.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == formattedSsid);

                            if (network == null)
                            {
                                res = (false, $"Nie udało się połączyć z {PreferredWifi}..");
                            }
                            else
                            {
                                //wifiMgr.Disconnect();
                                if (!wifiMgr.IsWifiEnabled)
                                {
                                    wifiMgr.SetWifiEnabled(true);
                                }
                                
                                var enableNetwork = wifiMgr.EnableNetwork(network.NetworkId, true);
                                wifiMgr.Reconnect();
                                res = (true, $"Połączono z {PreferredWifi} [{w.BSSID}]");
                            }
                        }
                    }
                }
            }

            return res;
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
    }
}