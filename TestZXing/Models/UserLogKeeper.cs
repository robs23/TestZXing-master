using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TestZXing.Static;
using SQLite;
using TestZXing.Interfaces;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text.RegularExpressions;

namespace TestZXing.Models
{
    public class UserLogKeeper : Keeper<UserLog>, IOfflineKeeper, IOfflineKeeper<UserLog>
    {
        protected override string ObjectName => "UserLog";

        protected override string PluralizedObjectName => "UserLogs";



    }
}
