using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TestZXing.Droid.Services;
using TestZXing.Interfaces;
using TestZXing.Models;

[assembly: Xamarin.Forms.Dependency(typeof(WifiHandler))]
namespace TestZXing.Droid.Services
{
    public class WifiHandler : IWifiHandler
    {
        public List<WiFiInfo> GetAvailableWifis(bool? GetSignalStrenth = false)
        {
            throw new NotImplementedException();
        }

        public WiFiInfo GetConnectedWifi(bool? GetSignalStrength = false)
        {
            WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));
            if (wifiManager != null)
            {
                WiFiInfo currentWifi = new WiFiInfo();
                currentWifi.SSID = wifiManager.ConnectionInfo.SSID;

                if ((bool)GetSignalStrength)
                {
                    currentWifi.Signal = WifiManager.CalculateSignalLevel(((ScanResult)currentWifi.SSID).Level, 5);
                }
                currentWifi.IsConnected = true;
                return currentWifi;
            }
            else
            {
                return null;
            }
        }
    }
}