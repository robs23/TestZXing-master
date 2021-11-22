using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class UserLogKeeper : Keeper<UserLog>
    {
        protected override string ObjectName => "UserLog";

        protected override string PluralizedObjectName => "UserLogs";
    }
}
