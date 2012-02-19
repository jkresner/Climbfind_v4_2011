using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlProvider = cf.Content.CfUrlProvider;

namespace cf.Web.Mvc.Routing
{
	public static class HomeRouteRegistrar
	{
		public static void RegisterRoutes(RouteCollection r)
		{
			var c = "Home";
			r.MapRoute("About", "rock-climbing-social-network", new { controller = c, action = "About" });
			r.MapRoute("AboutOpinions", "opinions", new { controller = c, action = "AboutOpinions" });
			r.MapRoute("AboutSearch", "rock-climbing-search-engine", new { controller = c, action = "AboutSearch" });
			r.MapRoute("AboutTheUpgrade", "upgrade-from-cf3-to-cf4", new { controller = c, action = "AboutTheUpgrade" });
			r.MapRoute("Vision", "vision", new { controller = c, action = "Vision" });
			r.MapRoute("CF5", "cf5", new { controller = c, action = "CF5" });
			r.MapRoute("Glossary", "rock-climbing-glossary", new { controller = c, action = "Glossary" });
			r.MapRoute("Partners", "partners", new { controller = c, action = "Partners" });
			r.MapRoute("Press", "press", new { controller = c, action = "Press" });
			r.MapRoute("Mobile", "mobile", new { controller = c, action = "Mobile" });
			r.MapRoute("AboutQR", "mobile/about-scan-codes", new { controller = c, action = "AboutQR" });
			r.MapRoute("Team", "team", new { controller = c, action = "Team" });
			r.MapRoute("Bugs", "bugs", new { controller = c, action = "Bugs" });
			r.MapRoute("Growth", "growth", new { controller = c, action = "Growth" });
			r.MapRoute("Credits", "credits", new { controller = c, action = "Credits" });
			r.MapRoute("ClimbingPartners", "search-for-rock-climbing-partners", new { controller = c, action = "ClimbingPartners" });
			r.MapRoute("ClimbingTravel", "rock-climbing-travel", new { controller = c, action = "ClimbingTravel" });
			r.MapRoute("ClimbingIndoor", "indoor-climbing-gym-search", new { controller = c, action = "ClimbingIndoor" });

			r.MapRoute("ResetPassword", "reset-password", new { controller = c, action = "ResetPassword" });

			r.MapRoute("TOS", "legal/terms-of-service", new { controller = c, action = "TermsOfService" });
			r.MapRoute("PP", "legal/privacy-policy", new { controller = c, action = "PrivacyPolicy" });

			r.MapRoute("ModeratorReputation", "moderator-reputation", new { controller = c, action = "ModeratorReputation" });
			r.MapRoute("ModeratorsProject", "moderators-project", new { controller = c, action = "ModeratorsProject" });
			r.MapRoute("WorldRockClimbingDatabase", "world-rock-climbing-database", new { controller = c, action = "WorldRockClimbingDatabase" });
		}
	}
}