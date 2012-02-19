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
    public class CfGodAuthorize : FilterAttribute, IAuthorizationFilter
    {
        public string Msg { get; set; }
        
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!CfPrincipal.IsGod())
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var routeData = filterContext.Controller.ControllerContext.RouteData;
            object controllerName = string.Empty;
            if (routeData.Values.TryGetValue("controller", out controllerName) && controllerName.ToString() == "Moderate")
            {
                filterContext.Result = new ViewResult() { ViewName = "Unauthorized" };
                if (!string.IsNullOrWhiteSpace(Msg))
                {
                    (filterContext.Result as ViewResult).ViewBag.Msg = Msg;
                }
                else
                {
                    (filterContext.Result as ViewResult).ViewBag.Msg = "Before you can moderate things on Climbfind, you must add at least one place to our database.";
                }
            }
            else
            {
                //base.HandleUnauthorizedRequest(filterContext);
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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