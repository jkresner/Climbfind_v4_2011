using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class VisitsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Visits";
            r.MapRoute("CheckInCurrent", "check-in-at/{id}", new { controller = c, action = "CreateCurrent" });
            r.MapRoute("My", "my-visit/{id}", new { controller = c, action = "My" });
            r.MapRoute("VisitsDetail", "visit/{id}", new { controller = c, action = "Detail" });
            r.MapRoute("ListUser", "history/{id}", new { controller = c, action = "ListUser" });
            r.MapRoute("ListMy", "history", new { controller = c, action = "ListMy" });
            r.MapRoute("GetAvailableClimbsForLogging", "get-available-climbs-for-logging/{id}", new { controller = c, action = "GetAvailableClimbsForLogging" });
            
        }
    }
}