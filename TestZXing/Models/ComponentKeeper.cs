using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class ComponentKeeper : Keeper<Component>
    {
        protected override string ObjectName => "Component";

        protected override string PluralizedObjectName => "Components";
    }
}
