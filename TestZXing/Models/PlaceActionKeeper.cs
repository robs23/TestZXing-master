using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class PlaceActionKeeper : Keeper<PlaceAction>
    {
        protected override string ObjectName => "PlaceAction";

        protected override string PluralizedObjectName => "PlaceActions";
    }
}
