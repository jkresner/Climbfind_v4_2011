using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using cf.Caching;
using System.Net;
using System.IO;
using System.Xml.Linq;
using cf.Web.Views.Places;
using cf.Entities.Enum;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using cf.Services;
using NetFrameworkExtensions;
using NetFrameworkExtensions.Web.Mvc;
using cf.Web.Mvc.ActionFilters;

namespace cf.Web.Controllers
{
    public partial class PlacesController : Controller
    {
        protected Country country { get { return ViewBag.Country as Country; } }
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MappingService mappingSvc { get { if (_mappingSvc == null) { _mappingSvc = new MappingService(); } return _mappingSvc; } } MappingService _mappingSvc;
        MediaService medSvc { get { if (_medSvc == null) { _medSvc = new MediaService(); } return _medSvc; } } MediaService _medSvc;

        public ActionResult PlaceNotFound()
        {
            return new HttpStatusCodeWithBodyResult("PlaceNotFound", 404);
        }

        public ActionResult PlaceIdRedirect(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);
            if (place == null) { return PlaceNotFound(); }
            else
            {
                return RedirectPermanent(place.SlugUrl);
            }
        }

        public ActionResult Countries() 
        {
            ViewBag.CountrySummaries = new GeoService().GetGeoSummary();
            return View(); 
        }


        [ModProfileInflate, CfAuthorize, CountryInflate(RedirectOnFailActionName = "PlaceNotFound")]
        public ActionResult PlaceNew(string countryUrlPart, string areaUrlPart)
        {
            var area = geoSvc.GetArea(country.ID, areaUrlPart);
            if (area == default(Area)) { return RedirectToAction("LocationNewPrestep"); }
            ViewBag.Current = area;
            ViewBag.Area = area;
            var areas = geoSvc.GetIntersectingAreas(area);
            areas.Add(area);
            //-- Adding the countryID check is important as when adding to Norwegian states that touch sweedish states they get mixed up.
            ViewBag.Provinces = areas.Where(a => a.Type == CfType.Province && a.CountryID == area.CountryID).ToList();
            ViewBag.Cities = areas.Where(a => a.Type == CfType.City && a.CountryID == area.CountryID).ToList();
            ViewBag.ClimbingAreas = areas.Where(a => a.Type == CfType.ClimbingArea).ToList();

            return View();
        }

        //[ModProfileInflate, CfAuthorize, CountryInflate(RedirectOnFailActionName = "PlaceNotFound")]
        //public ActionResult PlaceNewAdvanced(string countryUrlPart, string areaUrlPart)
        //{
        //    var area = geoSvc.GetArea(country.ID, areaUrlPart);
        //    if (area == default(Area)) { return RedirectToAction("LocationNewPrestep"); }
        //    ViewBag.Area = area;
        //    ViewBag.Current = area;
        //    var areas = geoSvc.GetIntersectingAreas(area);
        //    //-- Adding the countryID check is important as when adding to Norwegian states that touch sweedish states they get mixed up.
        //    ViewBag.Provinces = areas.Where(a => a.Type == CfType.Province && a.CountryID == area.CountryID).ToList();
        //    ViewBag.Cities = areas.Where(a => a.Type == CfType.City && a.CountryID == area.CountryID).ToList();
        //    ViewBag.ClimbingAreas = areas.Where(a => a.Type == CfType.ClimbingArea).ToList();

        //    return View();
        //}
    }
}
