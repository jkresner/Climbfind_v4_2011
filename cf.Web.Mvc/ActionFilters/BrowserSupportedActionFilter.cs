using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Identity;
using cf.Services;

namespace cf.Web.Mvc.ActionFilters
{
    public class BrowserSupportedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            System.Web.HttpBrowserCapabilitiesBase browser = context.HttpContext.Request.Browser;

            if (browser.Browser == "IE")
            {
                if (browser.MajorVersion < 9)
                {
                    context.Result = new ViewResult() { ViewName = "BrowserNotSupported" };
                    (context.Result as ViewResult).ViewBag.Msg = "Climbfind does not support internet explorer below version 9. Please upgrade to the latest IE or change to another browser.";
                }
            }
        }
    }
}