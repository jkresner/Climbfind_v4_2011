using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class MediaRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Media";

            r.MapRoute("ClimbMedia", UrlProvider.MediaUrlPrefix + "/" + UrlProvider.ClimbUrlPrefix + "/{nameUrlPart}/{id}",
                new { controller = c, action = "Climb" });

            r.MapRoute("LocationIndoorMedia", UrlProvider.MediaUrlPrefix + "/" + UrlProvider.IndoorUrlPrefix + "-{countryUrlPart}/{locationNameUrlPart}",
                new { controller = c, action = "LocationIndoor" });

            r.MapRoute("LocationOutdoorMedia", UrlProvider.MediaUrlPrefix + "/" + UrlProvider.OutdoorUrlPrefix + "-{countryUrlPart}/{locationTypeUrlPart}/{locationNameUrlPart}",
                new { controller = c, action = "LocationOutdoor" });

            r.MapRoute("AreaMedia", UrlProvider.MediaUrlPrefix + "/" + UrlProvider.AreaUrlPrefix + "-{countryUrlPart}/{nameUrlPart}",
                new { controller = c, action = "Area" });

            r.MapRoute("UserMedia", UrlProvider.MediaUrlPrefix + "/" + UrlProvider.ClimberUrlPrefix + "/{id}",
                new { controller = c, action = "UsersSubmittedMedia" });
            
            r.MapRoute("LatestCommunityMedia", "new-rock-climbing-media", new { controller = c, action = "LatestCommunityMedia" });
        }
    }
}