using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TestZXing.Interfaces;

namespace TestZXing.Models
{
    public class UserLog : Entity<UserLog>, IOfflineEntity
    {
        public override int Id
        {
            set => value = UserLogId;
            get => UserLogId;
        }
        
        public int UserLogId { get; set; }
        public string Platform { get; set; }
        public string Device { get; set; }
        public string LogName { get; set; }
        public Nullable<bool> HasTheAppCrashed { get; set; }
        public Nullable<bool> OnRequest { get; set; }
        public string Comment { get; set; }
        public string OfflineDescription { get => LogName; }

        private string _SyncStatus { get; set; }
        public string SyncStatus
        {
            get
            {
                return _SyncStatus;
            }
            set
            {
                if (value != _SyncStatus)
                {
                    _SyncStatus = value;
                }
            }
        }
    }
}
