using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation
{
    /// <summary>
    /// A static container for our application trace source (useful for web app/ worker role etc.)
    /// </summary>
    public static class CfTrace
    {
        public static CfTraceSource _current;
        public static CfTraceSource Current { get { return _current; }  }
        public static void InitializeTraceSource(CfTraceSource source) { _current = source; }
        public static void Error(Exception ex) { _current.Error(ex); }
        public static void Information(TraceCode code, string msg) { _current.Information(code, msg); }
        public static void Information(TraceCode code, string msgformat, params object[] args) { _current.Information(code, msgformat, args); }  
    }
}
