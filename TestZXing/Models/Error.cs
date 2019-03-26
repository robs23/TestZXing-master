using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Classes;
using TestZXing.Static;

namespace TestZXing.Models
{
    public class Error
    {
        public Error(Exception ex, string text, string methodName, string className)
        {
            string UserName = string.Empty;
            if(RuntimeSettings.CurrentUser != null)
            {
                UserName = RuntimeSettings.CurrentUser.FullName;
            }
            var properties = new Dictionary<string, string>
                {
                    {"Type", text},
                    {"Method", methodName},
                    {"Class", className},
                    {"User", UserName}
                };
            Crashes.TrackError(ex, properties);
        }
    }
}
