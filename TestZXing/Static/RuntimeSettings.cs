using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Static
{
    public static class RuntimeSettings
    {
        public static int TenantId { get; set; }
        public static string TenantToken
        {
            get
            {
                return "zyjp7h38DE205BpjzLqWw";
            }
        }
        public static string ApiAddress
        {
            get
            {
                return "http://jde_api.robs23.webserwer.pl/";
            }
        }

        public static int UserId { get; set; }

    }
}
