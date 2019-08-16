using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.CustomExceptions
{
    public class ServerUnreachableException : Exception
    {
        public ServerUnreachableException()
        {
        }

        public ServerUnreachableException(string message) : base(message)
        {
        }

        public ServerUnreachableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
