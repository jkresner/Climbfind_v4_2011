using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class ClimbsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Climbs";
                        
            r.MapRoute("ClimbNew", "rock-climbing-database/add-climb/{id}",
                new { controller = c, action = "ClimbNew" });
            //-- End Adding routes
                        
            r.MapRoute("ClimbDetail", UrlProvider.ClimbUrlPrefix + "/{nameUrlPart}/{id}",
                new { controller = c, action = "ClimbDetail" });
            //-- End Detail routes

            r.MapRoute("ClimbSectionDetail", "section/{sectionNameUrlPart}/{id}",
                new { controller = c, action = "ClimbSectionDetail" });

            r.MapRoute("ClimbIndoorNew", "managing-indoor-climbs/{id}",
                new { controller = c, action = "ClimbIndoorNew" });
                        //-- End Adding routes
        }
    }
}