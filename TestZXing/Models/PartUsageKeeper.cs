using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class PartUsageKeeper : Keeper<PartUsage>
    {
        protected override string ObjectName => "PartUsage";

        protected override string PluralizedObjectName => "PartUsages";
    }
}
