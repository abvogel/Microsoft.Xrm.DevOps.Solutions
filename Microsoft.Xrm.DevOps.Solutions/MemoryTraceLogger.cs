using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions
{
    class MemoryTraceLogger : TraceListener
    {
        private int id;
        public List<String> traces = new List<String>();

        public MemoryTraceLogger(): base()
        {
            this.id = Trace.Listeners.Add(this);
        }

        public override void Write(string message)
        {
            this.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            traces.Add(message);
        }

        internal void Reset()
        {
            traces.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
