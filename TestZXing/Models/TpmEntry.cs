using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class TpmEntry
    {
        public string Number { get; set; }
        public string Manager { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FinishedBy { get; set; }
        public string InitialDiagnosis { get; set; }
        public string RepairActions { get; set; }
        public string Status { get; set; }
        public string IsAdjustment { get; set; }
    }
}
