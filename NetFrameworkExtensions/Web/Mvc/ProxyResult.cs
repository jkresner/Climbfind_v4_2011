using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Web;
using NetFrameworkExtensions.Web;

namespace NetFrameworkExtensions.Web.Mvc
{
    /// <summary>
    /// Since javascript cannot execute cross domain requests sometimes it's necessary to provide a proxy for the javascript call.
    /// This result allows us to call any external services from the client
    /// </summary>
    /// <remarks>
    /// This is used in the Climbfind search bar up the top
    /// </remarks>
    /// <see cref="http://www.thefreakparade.com/2009/02/simple-aspnet-mvc-ajax-proxy/" />
    public class ProxyResult : ViewResult
    {
        public Uri TargetUri { get; set; }
        public HttpCookieCollection CookiesToPassOn { get; set; }

        /// <summary>
        /// Basic result from a target endpoint
        /// </summary>
        /// <param name="targetUri"></param>
        public ProxyResult(Uri targetUri) 
        {
            TargetUri = targetUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetUri"></param>
        /// <param name="cookiesToPassOn"></param>
        public ProxyResult(Uri targetUri, HttpCookieCollection cookiesToPassOn) : this (targetUri)
        {
            CookiesToPassOn = cookiesToPassOn;
        }

        /// <summary>
        /// Execute a web request against foreign resource and copy the target response to the local outgoing response steam
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            WebRequest proxy = WebRequest.Create(TargetUri);

            if (CookiesToPassOn != null) { proxy.Headers.Add("Cookie", CookiesToPassOn.ToCookieCollectionKeyValueString()); }

            using (WebResponse proxyResponse = proxy.GetResponse())
            {
                context.HttpContext.Response.ContentType = proxyResponse.ContentType;
                using (Stream proxyResponseStream = proxyResponse.GetResponseStream())
                {
                    StreamExtensions.CopyStream(proxyResponseStream, context.HttpContext.Response.OutputStream);
                }
            }
        }
    }
}
