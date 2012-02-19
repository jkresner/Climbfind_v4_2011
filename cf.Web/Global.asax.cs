using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cf.Web.Mvc.Routing;
using cf.Caching;
using cf.Caching.WazMemcached;
using cf.Instrumentation;
using cf.Web.Mvc.ActionFilters;
using cf.Identity;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.IdentityModel.Web;

namespace cf.Web
{
    public class CfWebApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (Stgs.WebRt.Contains("www")) { filters.Add(new WWWRedirectAttribute()); }
            filters.Add(new HandleCfErrorAttribute());
        }

        protected void Application_Start()
        {
            CfTrace.InitializeTraceSource(new Instrumentation.CfTraceSource("Cf.Web"));

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);

            //-- Setup caching depending on if we're running in the cloud or not
            if (!RoleEnvironment.IsAvailable) { CfCacheIndex.Initialize(); CfPerfCache.Initialize(); }
            else { CfCacheIndex.Initialize(new Level2MemcachedCacheIndex()); CfPerfCache.Initialize(new Level2MemcachedPerfCache()); }
            
            Microsoft.IdentityModel.Web.FederatedAuthentication.ServiceConfigurationCreated += new EventHandler
                <Microsoft.IdentityModel.Web.Configuration.ServiceConfigurationCreatedEventArgs>(RsaServerfarmSessionCookieTransform.OnServiceConfigurationCreated);
        }

        //void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        //{
        //    var token = e.SessionToken;
        //}

        //void SessionAuthenticationModule_SessionSessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        //{
        //    var token = e.SessionToken;
        //}


        //void WSFederationAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        //{
        //    e.WriteSessionCookie = true;

        //    e.SessionToken = FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(
        //        e.SessionToken.ClaimsPrincipal,
        //        e.SessionToken.Context,
        //        DateTime.UtcNow,
        //        DateTime.UtcNow.AddDays(300),
        //        true);

        //    var id = e.SessionToken.Id;
        //}

        void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                string relativeUrl = HttpContext.Current.Request.RawUrl.ToLower();
                Exception exception = Server.GetLastError().GetBaseException();

                if (exception.ShouldRecord(relativeUrl) && !HttpContext.Current.Request.Url.OriginalString.StartsWith("http://cf4"))
                {
                    CfTrace.Error(exception);
                }

                Server.ClearError();

                if (SpecialUrls.Cf3Redirect(relativeUrl))
                {
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", "http://cf3.climbfind.com"+relativeUrl);
                    Response.Flush();
                    Response.End();
                }
                else if (SpecialUrls.Instance.PermanentlyMoved.ContainsKey(relativeUrl))
                {
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", SpecialUrls.Instance.PermanentlyMoved[relativeUrl]);
                    Response.Flush();
                    Response.End();
                }
                else if (SpecialUrls.HasBeenRemoved(relativeUrl))
                {
                    Response.Status = "410 Gone";
                    Response.ContentType = "text/html";
                    Response.WriteFile(Server.MapPath("~/page-not-found.htm"));
                }
                else if (exception.Message.Contains("The controller for path") || exception.Message.Contains("A public action method"))
                {
                    Response.Status = "404 Not Found";
                    Response.ContentType = "text/html";
                    Response.WriteFile(Server.MapPath("~/page-not-found.htm"));
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span class='note'>GLOBAL ENDPOINT: PLEASE EMAIL jkresner@yahoo.com.au IMMEDIATELY IF YOU SEE THIS SCREEN!!!!!!!!!!!!</span>" + ex.ToString());
            }
        }
    }
}