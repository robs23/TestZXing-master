using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Interfaces
{
    public interface IOfflineEntity
    {
        int Id { get; set; }
        bool IsSyncing { get; set; }
        bool IsSynced { get; set; }
    }
}
