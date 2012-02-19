using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cf.Instrumentation.TraceListeners
{
    /// <summary>
    /// Not to be used in QA or Production!!, just to play with tracing during development
    /// </summary>
    /// <remarks>
    /// To use this TraceListener either add it to web.config under <system.diagnostics> or add to the TraceListenerCollection of
    /// the desired TraceSource object
    /// </remarks>
    public class DevelopmentTextFileTraceListener : TextWriterTraceListener
    {
        /// <summary>
        /// Construct our listener to point to the desired file on disk
        /// </summary>
        /// <param name="fileName"></param>
        public DevelopmentTextFileTraceListener(string fileName)
            : base(AppDomain.CurrentDomain.BaseDirectory + fileName) //-- use underlying TextWriterTraceListener scoped to the current app directory
        {

        }

        /// <summary>
        /// Override on TraceData to show how to deal gracefully with Exceptions
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is Exception)
            {
                var ex = data as Exception;

                var message = ex.Message;

                WriteLine(message);
            }
            else
            {
                WriteLine(data.ToString());
            }
        }



        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (eventCache.LogicalOperationStack.Count != 0)
            {
                Write(eventCache.LogicalOperationStack.Peek().ToString() + "\t");
            }

            WriteLine(message);

        }


        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            base.TraceEvent(eventCache, source, eventType, id);
        }

        public override void Write(string message)
        {
            base.Write(message);
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(message);
        }
    }
}
