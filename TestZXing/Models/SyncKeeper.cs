using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Interfaces;

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
