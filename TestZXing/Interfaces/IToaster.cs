using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Interfaces
{
    public interface IToaster
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
