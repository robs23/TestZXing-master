using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class AbandonReason : Entity<AbandonReason>
    {
        public int AbandonReasonId { get; set; }
        public override int Id
        {
            set => value = AbandonReasonId;
            get => AbandonReasonId;
        }

        public string Name { get; set; }
    }
}
