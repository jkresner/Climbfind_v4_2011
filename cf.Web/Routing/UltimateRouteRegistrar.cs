using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class UltimateRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Ultimate";
            r.MapRoute("PersonalityMediaDetail", "ultimate-2011-media/{id}", new { controller = c, action = "PersonalityMediaDetail" });
            r.MapRoute("UltimateCompRules", "ultimate-2011-rules", new { controller = c, action = "Rules" });
        }
    }
}