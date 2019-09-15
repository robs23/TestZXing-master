using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class PartKeeper : Keeper<Part>
    {
        protected override string ObjectName => "Part";

        protected override string PluralizedObjectName => "Parts";
    }
}
