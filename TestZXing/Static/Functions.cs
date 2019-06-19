using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.Static
{
    public static class Functions
    {
        public static DateTime NextShift(DateTime now)
        {
            //DateTime now = DateTime.Now;
            
            var x = now.ToString("H:mm");
            string[] hs = Regex.Split(x, ":");
            string h = hs[0];
            int H;
            bool parsive = int.TryParse(h, out H);
            if (parsive)
            {
                if(H<14 && H >= 6)
                {
                    //1st shift
                    return new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
                }
                else if(H<22 && H >=14)
                {
                    //2nd shift
                    return new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
                }
                else
                {
                    return DateTime.Today.AddHours(30);
                }
            }
            else
            {
                return now.AddHours(1);
            }
        }
    }
}
