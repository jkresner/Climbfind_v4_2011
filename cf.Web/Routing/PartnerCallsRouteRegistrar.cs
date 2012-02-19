using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
    public static class PartnerCallsRouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection r)
        {
            var c = "PartnerCalls";

            //r.MapRoute("PartnerCall", "my-climbing-feed", new { controller = c, action = "Detail" });
            r.MapRoute("PartnerCall", "climbing-partners", new { controller = c, action = "Index" });
            r.MapRoute("PartnerCallFeed", "partner-call/{id}", new { controller = c, action = "Detail" });
            r.MapRoute("PartnerCallNew", "new-partner-call/{id}", new { controller = c, action = "New" });
            r.MapRoute("PartnerCallPrivateReply", "partner-call-reply/{id}", new { controller = c, action = "PrivateReply" });
            r.MapRoute("PartnerCallSubscripion", "my-partner-call-subscriptions", new { controller = c, action = "Subscriptions" });
            r.MapRoute("PartnerCallSubscripionNew", "new-partner-call-subscription/{id}", new { controller = c, action = "SubscriptionNew" });
            r.MapRoute("UpdatePartnerCallFeedPlace", "update-partner-feed/{id}", new { controller = c, action = "UpdatePartnerCallFeedPlace" });
            
        }
    }
}