using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using NetFrameworkExtensions.Diagnostics;

namespace cf.Instrumentation
{
    /// <summary>
    /// Sets a correlation ID on all trace statements that execute inside a single Request to the HttpApplicationIntance. This facilitates
    /// querying instrumentation events and retrieving all events that fall within a single request (if they're stored in a database).
    /// </summary>
    /// <remarks>By passing the correlation ID to the ESB we should also be able to find associated events that are executing outside of the .net stack</remarks>
    /// <example>To use this functionality, add this module to the WebApps Web.config HttpModules section</example>
    public class InstrumentationRequestHttpModule : System.Web.IHttpModule
    {
        /// <summary>
        /// Access the current HttpApplicationInstance configured to use correlation tracing (by implementing IHasTraceSource)
        /// </summary>
        /// <remarks>If the TraceSource hasn't been initialized in global.asax our WebApp is going to crash on the first request (a good thing)</remarks>
        public IHasTraceSource WebApplication { get { return HttpContext.Current.ApplicationInstance as IHasTraceSource; } }

        /// <summary>
        /// Access the static / global TraceSource for the current running application so we can trace in this module
        /// </summary>
        public CfTraceSource GlobalApplicationTraceSource { get { return WebApplication.CfTracer; } }


        public HttpRequest CurrentRequest { get { return HttpContext.Current.Request; } }

        /// <summary>
        /// Wire up our event handlers to hook into BeginRequest, EndRequest, and PostAuthenticateRequest for ever Request made to the application
        /// </summary>
        /// <param name="context"></param>
        public void Init(System.Web.HttpApplication context)
        {
            context.BeginRequest += new EventHandler(SetRequestionInstrumentationCorrelation);
            context.EndRequest += new EventHandler(RemoveRequestionInstrumentationCorrelation);
            context.PostAuthenticateRequest += new EventHandler(PostAuthenticateRequest);
        }

        /// <summary>
        /// Set the correction Id at the beginning of the request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SetRequestionInstrumentationCorrelation(object sender, EventArgs e)
        {
            Trace.CorrelationManager.StartLogicalOperation(GenerateCorrelationReferenceString());

            GlobalApplicationTraceSource.Information("Begin Request " + CurrentRequest.RawUrl);
        }

        /// <summary>
        /// Remove the correction Id at the end of the request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Worth noting that this method fires even when an exception occurs in the page (pretty cool!)</remarks>
        protected void RemoveRequestionInstrumentationCorrelation(object sender, EventArgs e)
        {
            GlobalApplicationTraceSource.Information("End Request" + CurrentRequest.RawUrl);

            //-- Note if StopLogicalOperation fails, there's something wrong with some code in between StartLogicalOperation
            //-- and here and we want it to throw an exception so we become aware and can debug the problem. Also never use Pop()
            //-- to get a correlationID because that removes the logical operation (causing an exception on this line).
            //-- Instead use Peak()
            try
            {
                Trace.CorrelationManager.StopLogicalOperation();
            }
            catch { }
        }

        /// <summary>
        /// Posts the authenticate request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Records the web request based on the config section</remarks>
        protected void PostAuthenticateRequest(object sender, System.EventArgs args)
        {
            System.Web.HttpApplication application = sender as System.Web.HttpApplication;

            if (application != null)
            {
                
            }
        }

        public void Dispose() { }

        /// <summary>
        /// This could be any string e.g. "{0}{1}{2}{4}, AppName, UserName, WebRequest, DateTime" but for simplicity
        /// and also to be able to pass it to the ESB, we're just going to use a GUID
        /// </summary>
        private static string GenerateCorrelationReferenceString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
