using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetFrameworkExtensions.Diagnostics;
using System.Diagnostics;

namespace NetFrameworkExtensions.Threading
{
    /// <summary>
    ///  Taken from subtext http://subtextproject.com/
    /// </summary>
    public class ThreadHelper
    {
        private ITraceService Trace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trace"></param>
        public ThreadHelper(ITraceService trace) { Trace = trace; }
        
        public bool FireAndForget(WaitCallback callback, string failureLogMessage)
        {
            return ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    callback(o);
                }
                catch (Exception e)
                {
                    //-- 2 stands for error in CF TraceCode enumeration
                    Trace.TraceData(TraceEventType.Error, 999999, e); 
                }
            });
        }
    }
}
