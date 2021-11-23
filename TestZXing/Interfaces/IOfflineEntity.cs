using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Interfaces
{
    public interface IOfflineEntity
    {
        public int Id { get; set; }
        public bool IsSyncing { get; set; }
        public bool IsSynced { get; set; }
    }
}
