using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using System.Linq;

namespace TestZXing.ViewModels
{
    public class ProcessesPageViewModel : BaseViewModel
    {
        public ActiveProcessesViewModel UserProcesses { get; set; }
        public ActiveProcessesViewModel AllProcesses { get; set; }
        public ProcessesFilterViewModel Filter { get; set; }
        public ProcessesPageViewModel()
        {
            Filter = new ProcessesFilterViewModel();
        }

        public async Task Initialize()
        {
            base.Initialize();
            Task.Run(() => SetUpFilter());
        }

        public async Task SetUpFilter()
        {
            ProcessKeeper pk = new ProcessKeeper();
            await pk.Reload("IsCompleted = false and IsSuccessfull = false");
            List<Place> places = new List<Place>();
            List<ActionType> actionTypes = new List<ActionType>();
            List<Area> areas = new List<Area>();

            foreach(Process p in pk.Items)
            {
                if (!Filter.ActionTypes.Any(i => i.ActionTypeId == p.ActionTypeId))
                {
                    Filter.ActionTypes.Add(new ActionType() { ActionTypeId = p.ActionTypeId, Name = p.ActionTypeName });
                }
                if (!Filter.Places.Any(i => i.PlaceId == p.PlaceId))
                {
                    Filter.Places.Add(new Place() { PlaceId = p.PlaceId, Name = p.PlaceName });
                }
                if (!Filter.Areas.Any(i => i.AreaId == p.AreaId))
                {
                    Filter.Areas.Add(new Area() { AreaId = (int)p.AreaId, Name = p.AreaName });
                }
            }
        }

        public string Icon
        {
            get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
        }

        public string FilterIcon
        {
            get
            {
                if (Filter.IsSet)
                {
                    return "filter_full.png";
                }
                else
                {
                    return "filter_empty.png";
                }
            }
        }
    }
}
