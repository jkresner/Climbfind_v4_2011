using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Linq.Expressions;
using System.Text;
using cf.Entities.Interfaces;


namespace cf.Web.Mvc.Helpers
{
    public static class CfLinkExtensions
    {
        public static MvcHtmlString DeleteLink<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string linkText)
            where TController : Controller
        {
            return helper.ActionLink<TController>(action, linkText, new { @class = "delete" });
        }
    }
}