using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class ModerateRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Moderate";
            r.MapRoute("ModerateDash", "moderator-dashboard", new { controller = c, action = "Index" });
            r.MapRoute("ModerateAreas", "moderate-my-areas", new { controller = c, action = "Areas" });
            r.MapRoute("ModerateLocationsIndoor", "moderate-my-indoor-locations", new { controller = c, action = "LocationsIndoor" });
            r.MapRoute("ModerateLocationsOutdoor", "moderate-my-outdoor-locations", new { controller = c, action = "LocationsOutdoor" });
            r.MapRoute("ModerateClimbs", "moderator-my-climbs", new { controller = c, action = "Climbs" });
            r.MapRoute("ModerateEdit", "rock-climbing-database/edit/{id}", new { controller = c, action = "Edit" });
            r.MapRoute("ModerateClaimPage", "rock-climbing-database/claim/{id}", new { controller = c, action = "ClaimPage" });
            r.MapRoute("ModerateClimbEdit", "rock-climbing-database/edit-climb/{id}", new { controller = c, action = "ClimbEdit" });
            r.MapRoute("ModerateClaimClimb", "rock-climbing-database/claim-climb/{id}", new { controller = c, action = "ClaimClimb" });
        }
    }
}