using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class Part : Entity<Part>
    {
        [Unique]
        public int PartId { get; set; }
        public override int Id
        {
            set => value = PartId;
            get => PartId;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EAN { get; set; }
        public Nullable<int> ProducerId { get; set; }
        public string ProducerName { get; set; }
        public string ProducentsCode { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Symbol { get; set; }
        public string Destination { get; set; }
        public string Appliance { get; set; }
        public string UsedOn { get; set; }
        public string Token { get; set; }
        public string Identifier
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ProducentsCode))
                {
                    return this.ProducentsCode;
                }
                else if (!string.IsNullOrEmpty(this.Symbol))
                {
                    return this.Symbol;
                }
                else if (!string.IsNullOrEmpty(this.EAN))
                {
                    return this.EAN;
                }
                else
                {
                    return $"ID: {this.PartId}";
                }
            }
        }
    }
}
