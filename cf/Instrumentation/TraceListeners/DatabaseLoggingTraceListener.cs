using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH = cf.Instrumentation.TraceListenerConfigurationHelper;
using System.Diagnostics;
using System.Web;
using cf.Mail;

namespace cf.Instrumentation.TraceListeners
{
    /// <summary>
    /// A trace listener that logs and also sends email notifications
    /// </summary>
    public class DatabaseLoggingTraceListener : TraceListener
    {
        /// <summary>
        /// Constructor that takes configuration settings through the System.Diagnostics trace listener 'initializeData' attribute from .config
        /// </summary>
        /// <param name="initializeData"></param>
        public DatabaseLoggingTraceListener(string initializeData)
        {
            //string toEmails = CH.GetInitializeDataValue(initializeData, "ToEmail");

            //if (string.IsNullOrEmpty(toEmails))
            //{
            //    throw new ArgumentNullException("initializeData:ToEmail", "ExceptionEmailAndLoggingTraceListener initializeData:ToEmail not properly specified in .config");
            //}

            //ExceptionSubscribers = toEmails.Split(',');
        }

        /// <summary>
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (id == (int)TraceCode.PartnerCallNofiticatonWorkItemProcess) { 
                //new cf.DataAccess.Repositories.LogEventRepository().EfficientCreate(id, Stgs.SystemID, data.ToString()); 
            }
        }

        /// <summary>
        /// DO NOTHING!
        /// </summary>
        /// <remarks>Virtual so we can test this behavior</remarks>
        public virtual void DoNothing() { }

        /// <summary>
        /// override to do nothing because we're not interesting  in any trace other than Performance on ESBRequest
        /// </summary>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message) { DoNothing(); }

        /// <summary>
        /// override to do nothing because we're not interesting  in any trace other than Performance on ESBRequest
        /// </summary>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args) { DoNothing(); }

        /// <summary>
        /// override to do nothing because we're not interesting  in any trace other than Performance on ESBRequest
        /// </summary>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data) { DoNothing(); }

        /// <summary>
        /// override to do nothing because we're not interesting  in any trace other than Performance on ESBRequest
        /// </summary>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id) { DoNothing(); }

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }
    }

}
