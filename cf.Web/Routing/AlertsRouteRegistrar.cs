using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class AlertsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Alerts";
            r.MapRoute("Alerts", "alerts", new { controller = c, action = "Index" });
            r.MapRoute("SiteSettings", "site-usage-alerts", new { controller = c, action = "SiteSettings" });
        }   
    }
}