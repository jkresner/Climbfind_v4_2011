using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public class RouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //-- Remove when we move the upload application up to the cloud
            routes.MapRoute("UploadClimbPhotoMedia", "climb-photo/{id}/{mediaName}", new { controller = "Upload", action = "AddClimbPhotoMedia" });
            routes.MapRoute("UploadLocationPhotoMedia", "location-photo/{id}/{mediaName}", new { controller = "Upload", action = "AddLocationPhotoMedia" });
            routes.MapRoute("UploadCheckInPhotoMedia", "visit-photo/{id}/{mediaName}", new { controller = "Upload", action = "AddVisitPhotoMedia" });

            HomeRouteRegistrar.RegisterRoutes(routes);
            UsersRouteRegistrar.RegisterRoutes(routes);
            UtilityRouteRegistrar.RegisterRoutes(routes);
            PlacesRouteRegistrar.RegisterRoutes(routes);
            ClimbsRouteRegistrar.RegisterRoutes(routes);
            MediaRouteRegistrar.RegisterRoutes(routes);
            ModerateRouteRegistrar.RegisterRoutes(routes);
            HelpRouteRegistrar.RegisterRoutes(routes);
            VisitsRouteRegistrar.RegisterRoutes(routes);
            OpinionsRouteRegistrar.RegisterRoutes(routes);
            PostsRouteRegistrar.RegisterRoutes(routes);
            UltimateRouteRegistrar.RegisterRoutes(routes);
            AlertsRouteRegistrar.RegisterRoutes(routes);
            MessagesRouteRegistrar.RegisterRoutes(routes);
            PartnerCallsRouteRegistrar.RegisterRoutes(routes);

            routes.MapRoute("Default", "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } );
        } 
    }
}