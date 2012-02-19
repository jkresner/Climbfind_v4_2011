using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class UtilityRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Utility";

            r.MapRoute("Login", "login", new { controller = c, action = "LogOn" });
            r.MapRoute("Logout", "logout", new { controller = c, action = "LogOff" });
            r.MapRoute("Search", "search", new { controller = c, action = "Search" });
            r.MapRoute("SearchPlaces", "search-places", new { controller = c, action = "SearchPlaces" });
            r.MapRoute("SearchLocations", "search-locations", new { controller = c, action = "SearchLocations" });
            r.MapRoute("SearchProvinces", "search-provinces", new { controller = c, action = "SearchProvinces" });
            r.MapRoute("SearchClimbingAreas", "search-climbing-areas", new { controller = c, action = "SearchClimbingAreas" });
            r.MapRoute("Unauthorized", "unauthorized", new { controller = c, action = "Unauthorized" });
            r.MapRoute("PageNotFound", "page-not-found", new { controller = c, action = "PageNotFound" });   
        }
    }
}