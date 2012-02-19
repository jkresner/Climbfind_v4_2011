using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace cf.Web.Mvc.Routing
{
    public static class HelpRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Help";
            
            r.MapRoute("HelpIndex", "help", new { controller = c, action = "Index" });
            r.MapRoute("HelpFaq", "help-faq", new { controller = c, action = "Faq" });
        }
    }
}