using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Interfaces;

namespace TestZXing.Models
{
    public class SyncKeeper
    {
        List<IOfflineKeeper> Keepers = new List<IOfflineKeeper>();
    }
}
