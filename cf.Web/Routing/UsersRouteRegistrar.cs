using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class UsersRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Profiles";
            r.MapRoute("Me", UrlProvider.ClimberUrlPrefix + "/my-profile", new { controller = c, action = "Me" });
            r.MapRoute("Edit", UrlProvider.ClimberUrlPrefix + "/edit", new { controller = c, action = "Edit" });
            r.MapRoute("EditPlacePreferences", UrlProvider.ClimberUrlPrefix + "/edit-place-preferences", new { controller = c, action = "EditPlacePreferences" });
            r.MapRoute("ChooseAvatar", UrlProvider.ClimberUrlPrefix + "/choose-my-pic", new { controller = c, action = "ChooseAvatar" });
            r.MapRoute("UserProfile", UrlProvider.ClimberUrlPrefix + "/{id}", new { controller = c, action = "Detail" });
            r.MapRoute("Personality", UrlProvider.ClimberUrlPrefix + "/personality/{id}", new { controller = c, action = "Personality" });
            
            r.MapRoute("PersonalityMedia", "personality-media/{id}", new { controller = c, action = "PersonalityMediaDetail" });
        }
    }
}