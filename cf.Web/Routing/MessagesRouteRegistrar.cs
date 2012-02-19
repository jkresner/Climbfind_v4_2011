using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class MessagesRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Messages";
            r.MapRoute("Messages", "messages", new { controller = c, action = "Index" });
            r.MapRoute("SendMessages", "message/{id}", new { controller = c, action = "New" });
        }   
    }
}