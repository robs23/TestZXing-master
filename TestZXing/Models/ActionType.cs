using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class ActionType
    {
        public int ActionTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public bool? ShowInPlanning { get; set; }
        public bool? MesSync { get; set; }
        public bool? RequireInitialDiagnosis { get; set; }
        public bool? AllowDuplicates { get; set; }

        public override string ToString()
        {
            string str = "";
            if (this == null)
            {
                str = "ActionType=null";
            }
            else
            {
                str = "ActionTypeId={0}, Name={1}, Description={2}, CreatedOn={3}, CreatedByName={4}";
                str = string.Format(str, ActionTypeId, Name, Description, CreatedOn, CreatedByName);
            }
            
            return str;
        }
    }
}
