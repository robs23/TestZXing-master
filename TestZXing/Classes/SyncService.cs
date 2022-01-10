using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Static;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestZXing.Models
{
    public class SyncService
    {
        public List<IOfflineKeeper> Keepers { get; set; }
        public bool IsWorking
        {
            get
            {
                return Keepers.Any(k => k.IsWorking);
            }
        }

        public SyncService()
        {
            Keepers = new List<IOfflineKeeper>();
        }

        public async Task Sync()
        {
            foreach(var keeper in Keepers)
            {
                await keeper.Sync();
            }

        }

        public async Task RestoreSyncQueue()
        {
            foreach(var keeper in Keepers)
            {
                await keeper.RestoreSyncQueue();
            }
        }
    }
}
