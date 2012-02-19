using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Linq.Expressions;

namespace NetFrameworkExtensions.Web.Mvc
{
    /// <summary>
    /// Extra convenience Html Helpers used for building links with cleaner syntax in Mvc views
    /// </summary>
    public static class LinkExtensions
    {
        public static MvcHtmlString ActionCssLink<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string linkText, string cssClasses) 
            where TController : Controller
        {
            return helper.ActionLink<TController>(action, linkText, new { @class = cssClasses });
        }
    }
}
