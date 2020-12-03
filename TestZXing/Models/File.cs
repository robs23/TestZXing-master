using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class File : Entity<File>
    {
        [Unique]
        public int FileId { get; set; }
        public override int Id
        {
            set => value = FileId;
            get => FileId;
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Link { get; set; }
        public int? PartId { get; set; }
        public int? PlaceId { get; set; }
        public int? ProcessId { get; set; }
    }
}
