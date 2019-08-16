using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Models
{
    public class NoPreferredConnectionException : Exception
    {
        public NoPreferredConnectionException()
        {
        }

        public NoPreferredConnectionException(string message) : base(message)
        {
        }

        public NoPreferredConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
