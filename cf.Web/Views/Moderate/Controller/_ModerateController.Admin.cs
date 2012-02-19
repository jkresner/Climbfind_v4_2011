using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using cf.Entities;
using cf.Entities.Enum;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using cf.Web.Views.Moderate;
using cf.Caching;
using cf.Services;
using cf.Web.Mvc.ActionFilters;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Instrumentation;
using cf.Web.Mvc.ViewData;
using NetFrameworkExtensions.Web;
using NetFrameworkExtensions.Web.Mvc;
using Omu.ValueInjecter;

namespace cf.Web.Controllers
{
    public partial class ModerateController
    {
        [CfAuthorize(Roles = "ModAdmin", Msg = "The admin menu is only available for Admin Moderators", PopulateModDetails=true)]
        public ActionResult Admin()
        {
            return View();
        }


        [HttpGet]
        [CfAuthorize(Roles = "ModAdmin", Msg = "Refreshing the search index is only available for Admin Moderators", PopulateModDetails = true)]
        public ActionResult RefreshSearchIndex()
        {
            return View();
        }

        [HttpPost]
        [CfAuthorize(Roles = "ModAdmin", Msg = "Refreshing the search index is only available for Admin Moderators", PopulateModDetails = true)]
        public ActionResult RebuildSearchIndex()
        {
            try
            {
                //-- Refresh our local cache
                AppLookups.RefreshCacheIndex();

                //-- Refresh our remote cache & search index
                //var url = Stgs.RestSvcsUrl + "v0/search/refresh";

                //var requestCookies = HttpContext.Request.Cookies;
                //var authCookies = new HttpCookieCollection();
                //if (requestCookies.AllKeys.Contains("FedAuth")) { authCookies.Add(requestCookies.Get("FedAuth")); }
                //if (requestCookies.AllKeys.Contains("FedAuth1")) { authCookies.Add(requestCookies.Get("FedAuth1")); }

                return Json(new { Success = true, Msg = "Cache refresh notification sent to server" });
            }
            catch (Exception ex)
            {
                CfTrace.Error(ex);
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        [HttpGet]
        [CfAuthorize(Roles = "ModAdmin", Msg = "The map performance tuning feature is only available for Admin Moderators", PopulateModDetails = true)]
        public ActionResult PerformanceTuneMap()
        {
            return View();
        }

        [HttpGet]
        [CfAuthorize(Roles = "ModAdmin", Msg = "Country level editing is only available for Admin Moderators", PopulateModDetails = true)]
        public ActionResult CountryEdit(string id)
        {
            if (CfPrincipal.IsGod())
            {
                //-- TODO Put error check
                var cachedCountry = AppLookups.Countries.Where(c => c.NameUrlPart == id).SingleOrDefault();
                var country = geoSvc.GetCountryByID(cachedCountry.ID);
                ViewBag.Country = country;

                var geoJsonUrl = Stgs.MapSvcRelativeUrl + "country/" + id;

                var mapModel = new Bing7GeoJsonMapViewModel("climbing-map-" + id, 720, 480, geoJsonUrl);
                //mapModel.Buttons.Add(new Bing7MapButtonModel() { ButtonText = "Track LatLong", ButtonEventInitializer = "toggleTrackLatLong()" });
                ViewBag.MapModel = mapModel;

                return View(new CountryEditViewModel()
                {
                    WKT = new string(country.Geo.STAsText().Value),
                    GeoReduceThreshold = country.GeoReduceThreshold
                });
            }
            else
            {
                throw new AccessViolationException("You must be a GOD level Climbfind user to moderate country data! Moderate province or city level data instead.");
            }
        }

        [HttpPost]
        [CfAuthorize(Roles = "ModAdmin", Msg = "Country level editing is only available for Admin Moderators", PopulateModDetails = true)]
        public ActionResult CountryEdit(string id, CountryEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //-- Need to invalidate the cache... what about other app domains like the rss one? SHIT!
                var country = AppLookups.Countries.Where(c => c.NameUrlPart == id).SingleOrDefault();
                country.GeoReduceThreshold = model.GeoReduceThreshold;
                country.Geo = SqlGeography.Parse(new SqlString(model.WKT));

                geoSvc.UpdateCountry(country);
                return RedirectToAction("CountryEdit");
            }
            else
            {
                return RedirectToAction("CountryEdit", model);
            }
        }
    }
}
