using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class ProcessAction : Entity<ProcessAction>
    {
        public int ProcessActionId { get; set; }
        public override int Id
        {
            set => value = ProcessActionId;
            get => ProcessActionId;
        }

        public Nullable<int> ProcessId { get; set; }
        public Nullable<DateTime> PlannedStart { get; set; }
        public Nullable<DateTime> PlannedFinish { get; set; }
        public string PlaceName { get; set; }
        public Nullable<int> ActionId { get; set; }
        public string ActionName { get; set; }
        public int? GivenTime { get; set; }
        public int? HandlingTime { get; set; }
        public string Type { get; set; }
    }
}
