using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace cf.Web.Mvc.Helpers
{
    public static class CfPageTitleMaster
    {
        public static void Set(string format, params object[] args)
        {
            var viewBag = ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).ViewBag;
            viewBag.Title = string.Format(format + " - Climbfind", args);
        }
    }
}