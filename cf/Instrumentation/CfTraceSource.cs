using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NetFrameworkExtensions.Diagnostics;

namespace cf.Instrumentation
{
    /// <summary>
    /// An extension of System.Diagnostics.TraceSource allowing us to write trace statements with easier / more direct
    /// methods / overloads in a consistent manner across the application using TraceSource overloads that our tracelisteners are expecting
    /// </summary>
    /// <example>
    /// traceSourceObj.Exception(Exception ex, TraceCode traceCode)
    /// instead of:
    /// traceSourceObj.EventData(TraceEventType eventType, int traceId, object eventData)
    /// </example>
    /// <see cref="http://www.grimes.demon.co.uk/workshops/InstrWSSix.htm">To gain an understanding of what a TraceSource is</see>
    public class CfTraceSource
    {
        /// <summary>
        /// ITraceService is just a wrapper around System.Diagnostics.TraceSource making this class more easily testable
        /// </summary>
        public readonly ITraceService trace;

        /// <summary>
        /// Construct our CfTraceSource by internally creating a TraceService (wrapping a TraceSource) with the given name
        /// </summary>
        /// <param name="name"></param>
        public CfTraceSource(string name) { trace = new TraceService(name); }

        /// <summary>
        /// Construct our TraceSource by injecting the ITraceService dependency
        /// </summary>
        public CfTraceSource(ITraceService traceService) { trace = traceService; }

        /// <summary>
        /// Logical Equivalent of Trace.TraceInformation in .net 1.1
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="message">The message to trace</param>
        public void Information(TraceCode traceCode, string message)
        {
            trace.TraceEvent(TraceEventType.Information, (int)traceCode, (string)message);
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceInformation in .net 1.1
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="format">String format for String.Format()</param>
        /// <param name="args">Arguments for String.Format()</param>
        public void Information(TraceCode traceCode, string format, params object[] args)
        {
            Information(traceCode, (string)string.Format(format, args));
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceInformation in .net 1.1
        /// </summary>       
        /// <param name="message">The message to trace</param>
        public void Information(string message)
        {
            Information((int)TraceCode.None, message);
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceWarning in .net 1.1 with trace code categorization option
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="message">The message to trace</param>
        public void Warning(TraceCode traceCode, string message)
        {
            trace.TraceEvent(TraceEventType.Warning, (int)traceCode, (string)message);
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceWarning in .net 1.1
        /// </summary>       
        /// <param name="message">The message to trace</param>
        public void Warning(string message)
        {
            Warning((int)TraceCode.None, message);
        }


        /// <summary>
        /// Logical Equivalent of Trace.TraceWarning in .net 1.1
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="format">String format for String.Format()</param>
        /// <param name="args">Arguments for String.Format()</param>
        public void Warning(TraceCode traceCode, string format, params object[] args)
        {
            Warning(traceCode, (string)string.Format(format, args));
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceError in .net 1.1
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="ex">The exception object to trace</param>
        public void Error(TraceCode traceCode, Exception ex)
        {
            trace.TraceData(TraceEventType.Error, (int)traceCode, ex);
        }

        /// <summary>
        /// Logical Equivalent of Trace.TraceError in .net 1.1
        /// </summary>
        /// <param name="ex">The exception object to trace</param>
        public void Error(Exception ex)
        {
            Error(TraceCode.Exception, ex);
        }

        /// <summary>
        /// Lets us make a TraceData call with a predefined Performance object that we can check for on the trace listener side without the
        /// developer using this method to know how it's implemented
        /// </summary>
        /// <param name="traceCode">custom categorization for the event</param>
        /// <param name="startTime">DateTime at the start of the event (eventName) that we are capturing</param>
        /// <param name="elapsedTime">Elapsed time (should be coming from a StopWatch.Start()/Stop()/.Elapsed)</param>
        /// <param name="eventName">The name of the event we are capturing (up to the developer to figure out)</param>
        /// <param name="eventParameters">List of parameters for the event</param>
        public void Performance(TraceCode traceCode, DateTime startTime, TimeSpan elapsedTime,
            string eventName, params object[] eventParameters)
        {
            //-- New up a an object that we can check for on the listener side using the 'as' keyword
            PerformanceCapture performanceCapture = new PerformanceCapture(traceCode,
                                                           startTime,
                                                           startTime.Add(elapsedTime),
                                                           elapsedTime,
                                                           CurrentRequestCorrelationID,
                                                           eventName,
                                                           eventParameters);

            trace.TraceData(TraceEventType.Information, (int)traceCode, performanceCapture);
        }


        /// <summary>
        /// The outer most LogicalOperationStackID or the Name of the TraceSource if there is no LogicalOperation available on the stack
        /// </summary>
        public string CurrentRequestCorrelationID { get { return GetCurrentApplicationScopedCorrelationRequestID(); } }

        /// <summary>
        /// When we start logical operation scopes using the Trace.CorrelationManager we can also nest scopes within other
        /// scopes. GetCurrentApplicationScopedCorrelationRequestID has two purposes, the first is to check that a scope is
        /// available (otherwise return the name of the TraceSource for the correlationID), the second is to return only the outer most scope
        /// as that is the one that is going to be our correlation ID set by the RequestInstrumentationCorrelationHttpModule
        /// </summary>
        /// <returns></returns>
        private string GetCurrentApplicationScopedCorrelationRequestID()
        {
            var logicalOperationStack = System.Diagnostics.Trace.CorrelationManager.LogicalOperationStack;
            string correlationID = null;

            if (logicalOperationStack.Count > 0)
            {
                correlationID = logicalOperationStack.Peek().ToString();
            }
            else
            {
                //-- If we don't have a logical operation set we just use the trace source (application) name
                correlationID = trace.Name;
            }

            return correlationID;
        }


        /// <summary>
        /// The name of the inner System.Diagnostics.TraceSource object
        /// </summary>
        public string Name { get { return trace.Name; } }

        public void TimeActionMethodCall(Action methodCall)
        {
            //-- Step 3 Start Trace Execution
            Information(TraceCode.MethodCall, "Start " + methodCall.Method.Name);

            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //-- note we check if null because one AppDomain contains multiple instances of our System.Web.HttpApplication
            methodCall();

            stopwatch.Stop();
            Performance(TraceCode.MethodCall, startTime, stopwatch.Elapsed, "execute method ", methodCall.Method.Name);

            //-- Step 5 End Trace Execution
            Information(TraceCode.MethodCall, "End method calls after {0}ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodCall"></param>
        public T TimeMethodCall<T>(Func<T> methodCall)
        {
            //-- Step 3 Start Trace Execution
            Information(TraceCode.MethodCall, string.Format("Start {0}|{1}", methodCall.Method.Name, methodCall.Target));

            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //-- note we check if null because one AppDomain contains multiple instances of our System.Web.HttpApplication
            T t = methodCall();

            stopwatch.Stop();
            Performance(TraceCode.MethodCall, startTime, stopwatch.Elapsed, "execute method ", methodCall.Method.Name);

            //-- Step 5 End Trace Execution
            Information(TraceCode.MethodCall, "End method calls after {0}ms", stopwatch.ElapsedMilliseconds);

            return t;
        }
    }
}
