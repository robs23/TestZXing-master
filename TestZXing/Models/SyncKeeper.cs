using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class SyncKeeper
    {
        public List<IOfflineKeeper> Keepers { get; set; }

        public SyncKeeper()
        {
            Keepers = new List<IOfflineKeeper>();
        }

        
    }
}
