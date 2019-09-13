﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.Static
{
    public static class RuntimeSettings
    {
        public static int TenantId { get; set; }
        public static int UserId { get; set; }
        public static User CurrentUser { get; set; }
        public static string LocalDbPath { get; set; }
    }
}
