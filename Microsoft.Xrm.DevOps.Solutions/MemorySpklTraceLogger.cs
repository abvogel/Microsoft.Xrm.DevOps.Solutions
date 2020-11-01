using SparkleXrm.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions
{
    class MemorySpklTraceLogger : ITrace
    {
        public void Write(string format, params object[] args)
        {
            Trace.Write(DateTime.Now.ToLongTimeString() + "\t" + format, String.Join(", ", args));
        }

        public void WriteLine(string format, params object[] args)
        {
            Trace.WriteLine(DateTime.Now.ToLongTimeString() + "\t" + format, String.Join(", ", args));
        }
    }
}
