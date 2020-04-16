using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class Area : Entity<Area>
    {
        public int AreaId { get; set; }
        public override int Id
        {
            set => value = AreaId;
            get => AreaId;
        }

        public string Name { get; set; }
    }
}
