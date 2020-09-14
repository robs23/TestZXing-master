using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class Component : Entity<Component>
    {
        public int ComponentId { get; set; }
        public override int Id
        {
            set => value = ComponentId;
            get => ComponentId;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int? PlaceId { get; set; }
        public string PlaceName { get; set; }

    }
}
