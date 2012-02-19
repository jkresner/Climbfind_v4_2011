using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace cf.Web.Mvc.ActionFilters
{
    public class WWWRedirectAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public static string WebsiteUrl { get; set; }
        public static string WebsiteUrlWithoutWWW = GetUrlWithoutWWW();
        public static int WebsiteUrlWithoutWWWLength { get; set; }
        private static string GetUrlWithoutWWW() {
            WebsiteUrl = Stgs.WebRt;
            var withoutWWW = WebsiteUrl.Replace("www.", "");
            WebsiteUrlWithoutWWWLength = withoutWWW.Length;
            return withoutWWW;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url.ToString();

            if (url.Substring(0, WebsiteUrlWithoutWWWLength) == WebsiteUrlWithoutWWW)
            {
                filterContext.HttpContext.Response.Status = "301 Moved Permanently";
                filterContext.HttpContext.Response.AddHeader("Location", url.Replace(WebsiteUrlWithoutWWW, WebsiteUrl));
                filterContext.Result = new ContentResult() { Content = "Please use " + WebsiteUrl + " instead of " + WebsiteUrlWithoutWWW };
            }
        }
    }
}