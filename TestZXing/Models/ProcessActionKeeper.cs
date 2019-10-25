using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class ProcessActionKeeper : Keeper<ProcessAction>
    {
        protected override string ObjectName => "ProcessAction";

        protected override string PluralizedObjectName => "ProcessActions";
    }
}
