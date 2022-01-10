using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Classes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string TableName)
        {
            ForeignKey = false;
            Table = TableName;
        }

        public ForeignKeyAttribute(bool value)
        {
            ForeignKey = value;
        }

        public bool ForeignKey { get; set; } = false;
        public string Table { get; set; }
    }
}
