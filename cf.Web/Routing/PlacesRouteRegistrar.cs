using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class PlacesRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Places";

            //-- Special place routes
            r.MapRoute("PlaceIdRedirect", "climbing-database/{id}",
                new { controller = c, action = "PlaceIdRedirect" });
            
            r.MapRoute("PlaceNotFound", "rock-climbing-database/place-not-found",
                new { controller = c, action = "PlaceNotFound" });

            r.MapRoute("ClimbingByCountry", "world-rock-climbing",
                new { controller = c, action = "Countries" });
            //-- End Special routes

            //-----------------------------------------------------------------
            //-- Adding routes (must come before detail routes to work)
            //r.MapRoute("PlaceNewAdvanced", "rock-climbing-database/add-climbing-place-to/{countryUrlPart}/{areaUrlPart}",
            //    new { controller = c, action = "PlaceNewAdvanced" });

            //-- Used from check-in form (in feed)
            r.MapRoute("LocationNewPrestep", "rock-climbing-database/add-climbing-location",
                new { controller = c, action = "LocationNewPrestep", countryUrlPart = "unknown", type = "unknown" });

            //-- Used from LocationNewPreset (area radio button)
            r.MapRoute("AreaNewPrestep", "rock-climbing-database/choose-area-type/{countryUrlPart}/{type}",
                new { controller = c, action = "AreaNewPrestep", countryUrlPart = "unknown", type = "unknown" });

            //-- Used from menu down right of place pages
            r.MapRoute("PlaceNew", "rock-climbing-database/add-climbing-to/{countryUrlPart}/{areaUrlPart}",
                new { controller = c, action = "PlaceNew" });

            r.MapRoute("AreaNew", "rock-climbing-database/add-climbing-area/{countryUrlPart}/{type}",
                new { controller = c, action = "AreaNew" });

            r.MapRoute("AreaNewPost", "rock-climbing-database/save-climbing-area",
                new { controller = c, action = "AreaNewCreate" });

            r.MapRoute("AreaNewToArea", "rock-climbing-database/add-sub-area-to-area/{countryUrlPart}/{areaUrlPart}",
                new { controller = c, action = "AreaNewSubAreaToArea" });

            r.MapRoute("AreaNewToAreaStatic", "Places/AreaNewSubAreaToArea",
                new { controller = c, action = "AreaNewSubAreaToArea" });

            //r.MapRoute("SubAreaNew", "rock-climbing-database/add-sub-climbing-area/{countryUrlPart}/{parentAreaNameUrlType}",
            //    new { controller = c, action = "AreaNewSubClimbingAreaToArea" });

            r.MapRoute("LocationIndoorNew", "rock-climbing-database/add-indoor-location/{countryUrlPart}/{areaNameUrlPart}",
                new { controller = c, action = "LocationIndoorNew" });

            r.MapRoute("LocationOutdoorNew", "rock-climbing-database/add-outdoor-location/{countryUrlPart}/{areaNameUrlPart}",
                new { controller = c, action = "LocationOutdoorNew" });

            //-- End Adding routes
            
            //-- Detail routes
            r.MapRoute("CountryDetail", UrlProvider.CountryUrlPrefix + "-{countryUrlPart}",
                new { controller = c, action = "CountryDetail" });

            r.MapRoute("AreaDetail", "rock-climbing-{countryUrlPart}/{areaNameUrlPart}",
                new { controller = c, action = "AreaDetail" });

            r.MapRoute("LocationIndoorDetail", UrlProvider.IndoorUrlPrefix + "-{countryUrlPart}/{locationNameUrlPart}",
                new { controller = c, action = "LocationIndoorDetail" });

            r.MapRoute("LocationOutdoorDetail", UrlProvider.OutdoorUrlPrefix + "-{countryUrlPart}/{locationTypeUrlPart}/{locationNameUrlPart}",
                new { controller = c, action = "LocationOutdoorDetail" });

            //-- End Detail routes
        }
    }
}