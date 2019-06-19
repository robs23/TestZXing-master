using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.Interfaces
{
    public interface IWifiHandler
    {
        Task<WiFiInfo> GetConnectedWifi(bool? GetSignalStrength = false);

        List<WiFiInfo> GetAvailableWifis(bool? GetSignalStrenth = false);
    }
}
