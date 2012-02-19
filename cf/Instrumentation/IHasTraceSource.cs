using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation
{
    /// <summary>
    /// Interface declaring that we have access to an ITraceSource object
    /// </summary>
    /// <remarks>
    /// This interface is useful for gaining access to our TraceSource from anywhere in an application.
    /// </remarks>
    /// <example>
    /// For Webapps: declare that the globalasax class implements IHasTraceSource
    /// Then access by: (HttpContext.Current.ApplicationInstance as IHasTraceSource).CfTrace
    /// 
    /// For Desktop Apps: because the main method is static and it is in a static class Program (& static classes cannot 
    /// inherit interfaces), we need to create an instance of some sort of 'Container' that implements IHasTraceSource
    /// </example>
    public interface IHasTraceSource
    {
        CfTraceSource CfTracer { get; }
    }
}
