using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Identity;
using cf.Services;
using NetFrameworkExtensions.Web.Mvc;

namespace cf.Web.Mvc.ActionFilters
{
    public class ModProfileInflateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var modProfile = new GeoService().GetModProfile(CfIdentity.UserID);
                if (modProfile != null)
                {
                    CfPrincipal.AttachModProfile(modProfile);
                }
            }
        }
    }
}