using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TestZXing.Interfaces
{
    interface ILogService
    {
        void Initialize(Assembly assembly, string assemblyName);
    }
}
