using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class OpinionsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Opinions";
            r.MapRoute("NewOpinion", "rate/{id}", new { controller = c, action = "New", id = "{id}" });
            r.MapRoute("OpinionDetail", "opinion/{id}", new { controller = c, action = "Detail", id = "{id}" });
            r.MapRoute("OpinionsOnObject", "opinions-on-{id}", new { controller = c, action = "ListObject", id = "{id}" });
            r.MapRoute("OpinionsByUser", "opinions-by-{id}", new { controller = c, action = "ListUser", id = "{id}" });

            r.MapRoute("TopAreas", "top-rated-climbing-areas-in-the-world", new { controller = c, action = "TopAreas" });
            r.MapRoute("TopIndoor", "top-rated-indoor-climbing-in-the-world", new { controller = c, action = "TopIndoorLocations" });
            r.MapRoute("TopOutdoor", "top-rated-outdoor-climbing-in-the-world", new { controller = c, action = "TopOutdoorLocations" });
            r.MapRoute("TopClimbs", "top-rated-climbs-in-the-world", new { controller = c, action = "TopClimbs" });
        }   
    }
}