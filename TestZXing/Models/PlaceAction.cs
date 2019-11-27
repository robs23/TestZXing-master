using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;

namespace TestZXing.Models
{
    class PlaceAction : Entity<PlaceAction>, IActionKeeper
    {
        public int PlaceActionId { get; set; }
        public override int Id
        {
            set => value = PlaceActionId;
            get => PlaceActionId;
        }

        public Nullable<int> PlaceId { get; set; }
        public string PlaceName { get; set; }
        public Nullable<int> ActionId { get; set; }
        public string ActionName { get; set; }
        public int? GivenTime { get; set; }
        public string Type { get; set; }
        public bool IsRequired {
            get
            {
                return false;
            }
        }
    }
}
