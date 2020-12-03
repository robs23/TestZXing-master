using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.Models
{
    public class FileKeeper : Keeper<File>
    {
        protected override string ObjectName => "File";

        protected override string PluralizedObjectName => "Files";
    }
}
