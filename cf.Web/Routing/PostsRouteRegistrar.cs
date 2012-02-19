using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class PostsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "Posts";

            r.MapRoute("MyFeed", "my-climbing-feed", new { controller = c, action = "MyFeed" });
            r.MapRoute("Post", "post/{id}", new { controller = c, action = "Detail" });
        }
    }
}