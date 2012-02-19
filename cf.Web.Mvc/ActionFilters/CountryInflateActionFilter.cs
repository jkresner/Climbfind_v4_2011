using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Caching;
using System.Web.Routing;

namespace cf.Web.Mvc.ActionFilters
{
    /// <summary>
    /// Populates ViewBag.Country and redirects to 404 if the Country.NameUrlPart is not valid
    /// </summary>
    public class CountryInflateAttribute : ActionFilterAttribute
    {
        public string RedirectOnFailActionName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Country country = null;
            string countryUrlPart = null;

            bool countryUrlPartInvalid = false;

            //-- First check route data
            if (filterContext.RouteData.Values["countryUrlPart"] != null)
            {
                countryUrlPart = filterContext.RouteData.Values["countryUrlPart"].ToString();
            }

            //-- If route data fails, double check http query string and form parameters
            if (string.IsNullOrWhiteSpace(countryUrlPart))
            {
                if (filterContext.RequestContext.HttpContext.Request["countryUrlPart"] != null)
                {
                    countryUrlPart = filterContext.RequestContext.HttpContext.Request["countryUrlPart"].ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(countryUrlPart)) { countryUrlPartInvalid = true; }
            else
            {
                country = AppLookups.Country(countryUrlPart);
                if (country == default(Country)) { countryUrlPartInvalid = true; }
            }
            if (countryUrlPartInvalid) { filterContext.Result = new RedirectToRouteResult("PlaceNotFound", null); }
            else
            {
                filterContext.Controller.ViewBag.Country = country;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}