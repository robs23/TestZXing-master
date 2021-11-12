using System;
using System.Collections.Generic;
using System.Text;

namespace TestZXing.CustomExceptions
{
    public class WifiTurnedOffException : Exception
    {
        public WifiTurnedOffException()
        {
        }

        public WifiTurnedOffException(string message) : base(message)
        {
        }

        public WifiTurnedOffException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
