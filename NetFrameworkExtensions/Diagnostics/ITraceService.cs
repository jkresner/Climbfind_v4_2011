using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NetFrameworkExtensions.Diagnostics
{
    // The .net TraceSource is a concrete type and we want to test TraceSource with a dependency that is an interface (so we
    // can mock out the dependency.
    // The interface (ITraceService) and class (TraceService) demonstrate how to create an abstract wrapper around the TraceSouce type 
    // in order to make the TraceSource code unit testable.
    public interface ITraceService
    {
        string Name { get; }

        void TraceEvent(TraceEventType eventType, int id);
        void TraceEvent(TraceEventType eventType, int id, string message);
        void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);
        void TraceData(TraceEventType eventType, int id, object data);
        void TraceData(TraceEventType eventType, int id, params object[] data);
        void TraceInformation(string message);
        void TraceInformation(string format, params object[] args);
        void TraceTransfer(int id, string message, Guid relatedActivityId);
    }
}
