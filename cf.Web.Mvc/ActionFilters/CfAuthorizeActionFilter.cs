using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Identity;
using cf.Services;
using NetFrameworkExtensions.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Protocols.WSFederation;

namespace cf.Web.Mvc.ActionFilters
{
    /// <summary>
    /// Mvc Action filter for usage with WIF (Windows Identity Foundation) style authentication & authorization
    /// </summary>
    /// <remarks>
    /// This was taken / modified from project at : http://claimsid.codeplex.com/ 
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CfAuthorize : FilterAttribute, IAuthorizationFilter
    {
        public static GeoService geoSvc = new GeoService();
        public string Msg { get; set; }
        public string Roles { get; set; }
        public bool PopulateModDetails { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AuthenticateUser(filterContext);
            }
            else
            {
                if (PopulateModDetails) { geoSvc.SetModDetailsOnPrincipal(); }
                this.AuthorizeUser(filterContext);
            }
        }

        /// <summary>
        /// Force RP/STS style authentication flow to begin
        /// </summary>
        /// <param name="context"></param>
        private static void AuthenticateUser(AuthorizationContext context)
        {
            var returnUrl = GetReturnUrl(context.RequestContext);

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // user is not authenticated and it's entering for the first time
                var fam = FederatedAuthentication.WSFederationAuthenticationModule;
                var signIn = new SignInRequestMessage(new Uri(fam.Issuer), fam.Realm)
                {
                    Reply = returnUrl.ToString()
                };

                context.Result = new RedirectResult(signIn.WriteQueryString());
            }
        }

        /// <summary>
        /// Check if user has required roles
        /// </summary>
        /// <param name="context"></param>
        private void AuthorizeUser(AuthorizationContext context)
        {
            //-- If the attribute includes specified roles, then we go on to check if the user has them
            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var authorizedRoles = this.Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                bool hasValidRole = false;
                foreach (var role in authorizedRoles)
                {
                    if (context.HttpContext.User.IsInRole(role.Trim()) )
                    {
                        hasValidRole = true;
                        break;
                    }

                    if (CfPrincipal.ModDetails != null && CfPrincipal.ModDetails.Role == role)
                    {
                        hasValidRole = true;
                        break;
                    }
                }

                if (!hasValidRole)
                {
                    HandleUnauthorizedRequest(context);
                }
            }
        }


        protected void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult() { ViewName = "Unauthorized" };
            if (!string.IsNullOrWhiteSpace(Msg))
            {
                (filterContext.Result as ViewResult).ViewBag.Msg = Msg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Uri GetReturnUrl(RequestContext context)
        {
            var request = context.HttpContext.Request;
            var reqUrl = request.Url;
            var wreply = new StringBuilder();

            wreply.Append(reqUrl.Scheme); // e.g. "http"
            wreply.Append("://");
            wreply.Append(request.Headers["Host"] ?? reqUrl.Authority);
            wreply.Append(request.RawUrl);

            if (!request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                wreply.Append("/");
            }

            return new Uri(wreply.ToString());
        }


        private string GetControllerName(AuthorizationContext filterContext)
        {
            object controllerName = string.Empty;
            var routeData = filterContext.Controller.ControllerContext.RouteData;
            if (routeData.Values.TryGetValue("controller", out controllerName))
            {
                return controllerName.ToString();
            }
            return string.Empty;
        }
    }
}