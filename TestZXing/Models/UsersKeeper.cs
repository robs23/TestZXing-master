using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;
using System.Collections.ObjectModel;

namespace TestZXing.Models
{
    public class UsersKeeper
    {
        public List<User> Users { get; set; }

        public UsersKeeper()
        {
            Users = new List<User>();
        }

    }
}
