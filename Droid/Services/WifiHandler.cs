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

        public WifiHandler()
        {
            this.context = Android.App.Application.Context;
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

        class WifiReceiver : BroadcastReceiver
        {
            private WifiManager wifi;
            private List<WiFiInfo> wiFiInfos;
            private List<string> wifiNetworks;
            private AutoResetEvent receiverARE;
            private Timer tmr;
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
                tmr = new Timer(Timeout, null, TIMEOUT_MILLIS, System.Threading.Timeout.Infinite);
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
                    WiFiInfo nWF = new WiFiInfo { SSID = wifinetwork.Ssid, BSSID=wifinetwork.Bssid, Signal = wifinetwork.Level, IsConnected=isConnected };
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