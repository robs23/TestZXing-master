using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class AbandonReasonKeeper : Keeper<AbandonReason>
    {
        protected override string ObjectName => "AbandonReason";

        protected override string PluralizedObjectName => "AbandonReasons";
    }
}
