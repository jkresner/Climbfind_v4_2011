using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using cf.Instrumentation;
using System.Net;
using System.Text;

namespace cf.Svc
{
    public class AbstractRestService
    {
        protected CfTraceSource CfTracer { get { return (HttpContext.Current.ApplicationInstance as IHasTraceSource).CfTracer; } }
        
        /// <summary>
        /// Converts the .net enumerable list into a json string and sets the Response content type to "application/json"
        /// </summary>
        /// <param name="results">The enumerable list of objects we want to serialize</param>
        /// <returns>A WCF Message containing our json text</returns>
        protected Message ReturnResultAsJson<T>(IEnumerable<T> results)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var output = serializer.Serialize(results);

            return WebOperationContext.Current.CreateTextResponse(output, "application/json", new UTF8Encoding(false));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Message ReturnAsJson(object obj)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            //WebOperationContext.Current.OutgoingResponse.e
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var output = serializer.Serialize(obj);

            return WebOperationContext.Current.CreateTextResponse(output, "application/json", new UTF8Encoding(false));
        }


        protected Message Unauthorized(string message)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
            return WebOperationContext.Current.CreateTextResponse(message);
        }

        protected Message Forbidden(string message)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Forbidden;
            return WebOperationContext.Current.CreateTextResponse(message);
        }

        protected Message Failed(string message)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
            return WebOperationContext.Current.CreateTextResponse(message);
        }

        protected Message Gone(string message)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Gone;
            return WebOperationContext.Current.CreateTextResponse(message);
        }
    }
}