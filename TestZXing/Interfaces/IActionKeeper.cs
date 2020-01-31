using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Interfaces
{
    public interface IActionKeeper
    {
        //Will be implemented by all classes handling actions e.g. processAction, placeAction..
        int Id { get; set; }
        int? ActionId { get; set; }
        string ActionName { get; set; }
        Nullable<int> PlaceId { get; set; }
        string PlaceName { get; set; }
        int? GivenTime { get; set; }
        string Type { get; set; }
        bool IsRequired { get; }
        bool? IsChecked { get; set; }
        bool? IsMutable { get; set; } //can it be changed? Not, if it was saved already..
        List<DateTime?> LastChecks { get; set; }
        DateTime? LastCheck { get;  }
    }
}
