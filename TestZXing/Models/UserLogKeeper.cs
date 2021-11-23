using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TestZXing.Static;
using SQLite;
using System.Collections.ObjectModel;
using TestZXing.Interfaces;

namespace TestZXing.Models
{
    public class UserLogKeeper : Keeper<UserLog>
    {
        protected override string ObjectName => "UserLog";

        protected override string PluralizedObjectName => "UserLogs";
        public bool IsWorking { get; set; } = false;
    }
}
