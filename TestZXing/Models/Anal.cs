using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class Anal
    {
        public Anal(string title, Dictionary<string,string> props = null)
        {
            string UserName = string.Empty;
            if (RuntimeSettings.CurrentUser != null)
            {
                UserName = RuntimeSettings.CurrentUser.FullName;
            }
            var properties = new Dictionary<string, string>
                {
                    {"User", UserName}
                };
            if(props != null)
            {
                foreach(KeyValuePair<string,string> prop in props)
                {
                    properties.Add(prop.Key, prop.Value);
                }
            }
            Analytics.TrackEvent(title, properties);
        }
    }
}
