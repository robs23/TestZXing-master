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
        public WiFiInfo ConnectedWifi { get; set; }

        public Error(Exception ex)
        {
             
        }
    }

    public class WiFiInfo
    {
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public int NetworkId { get; set; }
        public int Signal { get; set; }
        public bool IsConnected { get; set; }
    }
}
