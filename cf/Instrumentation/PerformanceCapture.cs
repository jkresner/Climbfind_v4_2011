using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation
{
    /// <summary>
    /// Encapsulates performance data that we wish to trace and pass through to the trace listeners as a strongly typed object 
    /// which we can check for when they receive the data
    /// </summary>
    public class PerformanceCapture
    {
        public TraceCode TraceCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string RequestCorrelationID { get; set; }
        public string EventName { get; set; }
        public object[] Parameters { get; set; }

        /// <summary>
        /// Private constructor so that a PerformanceCapture object cannot be 'newed' up without the required parameters in the alternate constructor
        /// </summary>
        private PerformanceCapture() { }

        /// <summary>
        /// Constructor taking all required parameters for a Performance Capture
        /// </summary>
        /// <param name="traceCode">Enumerated trace code for the type of event we are perf tracing</param>
        /// <param name="startTime">Start DateTime of the performance capture</param>
        /// <param name="stopTime">Stop DateTime of the performance capture</param>
        /// <param name="elapsedTime">Timespan from start to finish</param>
        /// <param name="requestCorrelationID">Correlation ID of the request which this Performance Capture Belongs To</param>
        /// <param name="eventName">Name of the event we are capturing (usually decided by the developer writing the code that is making the trace)</param>
        /// <param name="parameters">A list of objects that are useful for encapsulating more information about the event, eg. an 'ID' of an object</param>
        public PerformanceCapture(TraceCode traceCode, DateTime startTime, DateTime stopTime, TimeSpan elapsedTime,
            string requestCorrelationID, string eventName, params object[] parameters)
        {
            TraceCode = traceCode;
            StartTime = startTime;
            StopTime = stopTime;
            ElapsedTime = elapsedTime;
            RequestCorrelationID = requestCorrelationID;
            EventName = eventName;
            Parameters = parameters;
        }

        /// <summary>
        /// Override of ToString() which returns a human readable string representing the values in the PerformanceCapture instance instead of
        /// GetType().Name
        /// </summary>
        /// <remarks>The default trace listeners use .ToString() in TraceData to serialize the object to something that can be written to file,
        /// so this is what is used</remarks>
        /// <returns>Pipe separated value representation of PerformanceCounter Properties</returns>
        public override string ToString()
        {
            return string.Format("Perf[{0}|{1}|{2}|{3}|{4}|{5}]", TraceCode, EventName, ParametersStringRepresentation, StartTime.Ticks, StopTime.Ticks, ElapsedTime);
        }

        /// <summary>
        /// Make sure we get each of the sub objects .ToString() instead of object[].GetType().Name
        /// </summary>
        public string ParametersStringRepresentation { get { return string.Format("{0}", Parameters); } }
    }
}
