using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NetFrameworkExtensions.Diagnostics
{
    /// <summary>
    /// Wrapper around the .net Trace Source class in case we want to do some testing
    /// </summary>
    public class TraceService : ITraceService
    {
        private readonly TraceSource _trace;

        public TraceService(string name) { _trace = new TraceSource(name); }
        public TraceService(TraceSource traceSource) { _trace = traceSource; }

        public string Name { get { return _trace.Name; } }

        public void TraceEvent(TraceEventType eventType, int id) { _trace.TraceEvent(eventType, id); }
        public void TraceEvent(TraceEventType eventType, int id, string message) { _trace.TraceEvent(eventType, id, message); }
        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args) { _trace.TraceEvent(eventType, id, format, args); }
        public void TraceData(TraceEventType eventType, int id, object data) { _trace.TraceData(eventType, id, data); }
        public void TraceData(TraceEventType eventType, int id, params object[] data) { _trace.TraceData(eventType, id, data); }
        public void TraceInformation(string message) { _trace.TraceInformation(message); }
        public void TraceInformation(string format, params object[] args) { _trace.TraceInformation(format, args); }
        public void TraceTransfer(int id, string message, Guid relatedActivityId) { _trace.TraceTransfer(id, message, relatedActivityId); }
    }
}
