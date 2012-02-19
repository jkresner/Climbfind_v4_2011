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
using Omu.ValueInjecter;
using cf.Entities.Interfaces;
using cf.Dtos;
using cf.Identity;

namespace cf.Web.Controllers
{
    public partial class PlacesController : Controller
    {
        [CountryInflate(RedirectOnFailActionName="PlaceNotFound")]
        public ActionResult CountryDetail(string countryUrlPart) 
        { 
            ViewBag.Current = country;
            ViewBag.Provinces = AppLookups.CountrysProvinces(country.ID);
            var areas = geoSvc.GetCitiesAndMajorClimbingAreasOfCountry(country.ID);
            
            ViewBag.Cities = areas.Where(a => a.Type == CfType.City).ToList().RandomSample(12);
            var climbingAreas = areas.Where(a => a.Type == CfType.ClimbingArea).ToList();
            ViewBag.ClimbingAreas = climbingAreas.RemoveAllChildAreas().RandomSample(12);
            
            var geoJsonUrl = Stgs.MapSvcRelativeUrl + "country/" + countryUrlPart;
            var mapModel = new Bing7GeoJsonMapViewModel("climbing-map-" + countryUrlPart, 730, 460, geoJsonUrl) { MapTypeId = "road" };
            mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mappingSvc.GetCustomCountryBingMapView(country));
            ViewBag.MapModel = mapModel;

            ViewBag.TopClimbs = geoSvc.GetTopClimbOfCountry(country.ID, 24);
            ViewBag.TopLocations = geoSvc.GetTopLocationsOfCountry(country.ID);

            return View("CountryDetail");         
        }

        [CountryInflate(RedirectOnFailActionName = "PlaceNotFound")]
        public ActionResult AreaDetail(string countryUrlPart, string areaNameUrlPart) 
        {
            var area = geoSvc.GetArea(country.ID, areaNameUrlPart);
            if (area == null) { return PlaceNotFound(); }
            ViewBag.Current = area;

            if (CfIdentity.IsAuthenticated) //-- Only make db call if displaying map (improve performance)
            {
                ViewBag.MapView = new Bing7MapViewOptionsViewModel(mappingSvc.GetBingViewByID(area.ID));
            }

            //-- Looks like we no longer use this on the pages...
            //var modPlace = geoSvc.GetObjectModMetaOrSystemCreate(area);
            //ViewBag.ModPlaceDetails = modPlace;

            ViewBag.PageQR = string.Format("http://a/{0}", area.ID.ToString("N"));
            ViewBag.LatestOpinions = new ContentService().GetLatestOpinionsOnObject(area.ID, 20).ToList();

            var locationsOfArea = geoSvc.GetLocationsOfArea(area.ID);
            var indoorLocations = locationsOfArea.Where(c => c.IsIndoorClimbing).OrderByDescending(l => l.Rating).Take(18).ToList();
            ViewBag.IndoorLocations = indoorLocations;

            var outdoorLocations = locationsOfArea.Where(c => c.IsOutdoorClimbing).OrderByDescending(l => l.Rating).Take(18).ToList();
            ViewBag.OutdoorLocations = outdoorLocations;

            if (area.Type == CfType.ClimbingArea) { return OtherAreaDetail(area, locationsOfArea); } 
            else if (area.Type == CfType.Province) { return ProvinceDetail(area, locationsOfArea); }
            else if (area.Type == CfType.City) { return CityDetail(area,locationsOfArea); }
            else { throw new NotImplementedException(string.Format("Area {0}[{1}] has wrong type {2}", area.Name, area.ID, area.Type)); }
        }

        /// <summary>
        /// In Main: Show cities, indoor walls (top 21), top areas (top 21)
        /// In Side: Provinces of Country
        /// </summary>
        [ChildActionOnly]
        public ActionResult ProvinceDetail(Area area, IEnumerable<Location> locationsOfArea)
        {
            var provinces = AppLookups.CountrysProvinces(area.CountryID);
            provinces.Remove(area);
            ViewBag.Provinces = provinces;

            var intersectingAreas = geoSvc.GetIntersectingAreasWithGeoInflate(area);
            ViewBag.Cities = intersectingAreas.Where(p => p.Type == CfType.City).ToList();
            
            var intersectingParentClimbingAreas = intersectingAreas.Where(p => p.Type == CfType.ClimbingArea && p.ShapeArea > area.ShapeArea).ToList();
            var intersectingChildClimbingAreas = intersectingAreas.Where(p => p.Type == CfType.ClimbingArea && p.ShapeArea < area.ShapeArea).ToList().RemoveAllChildAreas().Take(20).ToList();

            var displayClimbingAreas = new List<Area>(intersectingParentClimbingAreas);
            displayClimbingAreas.AddRange(intersectingChildClimbingAreas);

            ViewBag.ClimbingAreas = displayClimbingAreas.OrderByDescending(l => l.Rating).ToList();

            var geoJsonUrl = Stgs.MapSvcRelativeUrl + "province/" + area.ID.ToString();
            ViewBag.MapModel = new Bing7GeoJsonMapViewModel("rock-climbing-map-" + area.NameUrlPart, 730, 420, geoJsonUrl) { ViewOptions = ViewBag.MapView };
            
            return View("ProvinceDetail");
        }

        /// <summary>
        /// In Main: Indoor Walls (top 21)
        /// In Side: Parent Provinces, Nearby Climbing Areas, Top outdoor locations (top 12)
        /// </summary>
        [ChildActionOnly]
        public ActionResult CityDetail(Area area, IEnumerable<Location> locationsOfArea)
        {
            ViewBag.TopOutdoorLocations = locationsOfArea.Where(c => c.IsOutdoorClimbing).OrderByDescending(l => l.Rating).Take(12).ToList();

            var relatedAreas = geoSvc.GetRelatedAreas(area);
            ViewBag.Provinces = relatedAreas.Where(a => a.Type == CfType.Province).ToList();
            
            var relatedClimbingAreas = relatedAreas.Where(c => c.Type == CfType.ClimbingArea).ToList();
            var climbingAreas = area.GetIntersectingAreas(relatedClimbingAreas);
            ViewBag.ClimbingAreas = climbingAreas;
            
            var nearbyAreas = area.GetNonIntersectingAreas(relatedClimbingAreas).RemoveAllChildAreas();
            ViewBag.NearbyClimbingAreas = nearbyAreas;

            var geoJsonUrl = Stgs.MapSvcRelativeUrl + "city/" + area.ID.ToString();
            var mapModel = new Bing7GeoJsonMapViewModel("rock-climbing-map-" + area.NameUrlPart, 730, 420, geoJsonUrl);
            if (area.Type != CfType.Province) { mapModel.SetInvisiblePolygons(); }
            mapModel.ViewOptions = ViewBag.MapView;
            ViewBag.MapModel = mapModel;

            return View("CityDetail");
        }

        /// <summary>
        /// In Main: Media, Sub Areas, Outdoor Locations (Top 21)
        /// In Side: Parent Provinces/Cities/CAreas, Nearby Climbing Areas, Top outdoor locations (top 12)
        /// </summary>
        [ChildActionOnly]
        public ActionResult OtherAreaDetail(Area area, IEnumerable<Location> locationsOfArea)
        {
            var relatedAreas = geoSvc.GetRelatedAreas(area);
            ViewBag.RelatedAreas = relatedAreas.Where(a => a.Type == area.Type).ToList();

            ViewBag.Provinces = relatedAreas.Where(a => a.Type == CfType.Province).ToList();
            ViewBag.Cities = relatedAreas.Where(p => p.Type == CfType.City).ToList();
            
            var relatedClimbingAreas = relatedAreas.Where(p => p.Type == CfType.ClimbingArea).ToList();
            var intersectingClimbingAreas = area.GetIntersectingAreas(relatedClimbingAreas);
            var parentAreas = area.GetParentAreas(intersectingClimbingAreas);
            intersectingClimbingAreas.RemoveAll( a => parentAreas.Contains(a)) ;
            
            var nearbyAreas = relatedClimbingAreas.Where(a => !intersectingClimbingAreas.Contains(a) && !parentAreas.Contains(a));
            ViewBag.ClimbingAreas = intersectingClimbingAreas;
            ViewBag.ParentClimbingAreas = parentAreas;
            ViewBag.NearbyClimbingAreas = nearbyAreas;

            var geoJsonUrl = Stgs.MapSvcRelativeUrl + "area/" + area.ID.ToString();
            ViewBag.MapModel = new Bing7GeoJsonMapViewModel("rock-climbing-map-" + area.NameUrlPart, 730, 420, geoJsonUrl, true) { ViewOptions = ViewBag.MapView };
            
            ViewBag.TopClimbs = geoSvc.GetTopClimbsOfArea(area.ID, 10);
            ViewBag.MediaList = medSvc.GetObjectsTopMedia(area.ID, 10);

            return View("AreaDetail");
        }

        [CfAuthorize]
        public ActionResult AreaNewPrestep(string countryUrlPart, string type) 
        {
            var model = new AreaNewPrestepViewModel() { countryUrlPart = countryUrlPart, type = type };
            
            ViewBag.CountryDropDownList = AppLookups.Countries.ToSelectList(c => c.NameUrlPart, c => c.Name);
            return View(model);
        }

        [HttpGet, CfAuthorize, CountryInflate(RedirectOnFailActionName = "AreaNewPrestep")]
        public ActionResult AreaNew(string countryUrlPart, string type)
        {
            CfType pType = UrlTypeToPlaceTypeEnum(type);
            if (pType == CfType.Unknown) { return RedirectToAction("AreaNewPrestep"); }

            List<Area> existingAreas = geoSvc.GetAreasOfCountry(country.ID).Where(a => a.Type == pType).OrderBy(a=>a.Name).ToList();
            
            InitializeAreaNewViewData(country, pType, existingAreas);

            return View();
        }

        private void InitializeAreaNewViewData(IArea parentArea, CfType placeType, List<Area> existingAreasOfType)
        {
            ViewBag.ParentArea = parentArea;
            ViewBag.AreaType = (placeType == CfType.City) ? "City" : "Area";
            ViewBag.AreaTypePluralName = (placeType == CfType.City) ? "cities" : "areas";

            ViewBag.ExistingAreas = existingAreasOfType;
            ViewBag.PlaceTypeID = (byte)placeType;            
        }

        private CfType UrlTypeToPlaceTypeEnum(string type)
        {
            if (!string.IsNullOrWhiteSpace(type)) 
            {  
                if (type == "province") { return CfType.Province; }
                if (type == "city") { return CfType.City; }
                if (type == "climbing") { return CfType.ClimbingArea; }
            }

            return CfType.Unknown;
        }

        /// <summary>
        /// The main reason for this overload is to avoid data overload for the user. E.g. if they had to add
        /// an area to the USA, it would like a few hundred other areas that they need not replicate
        /// </summary>
        /// <param name="countryUrlPart"></param>
        /// <param name="provinceUrlPart"></param>
        /// <returns></returns>
        [CfAuthorize, CountryInflate(RedirectOnFailActionName = "AreaNewPrestep")]
        public ActionResult AreaNewSubAreaToArea(string countryUrlPart, string areaUrlPart)
        {
            var parentArea = geoSvc.GetArea(country.ID, areaUrlPart);
            if (parentArea == null) { return PlaceNotFound(); }

            List<Area> existingAreasOfType = geoSvc.GetIntersectingAreas(parentArea).Where(
                a => a.Type == CfType.ClimbingArea).OrderBy(a => a.Name).ToList();

            InitializeAreaNewViewData(parentArea, CfType.ClimbingArea, existingAreasOfType);

            var mapCollection = new MapItemCollection();
            mapCollection.AppendGeographyToGeoMapItemCollection(parentArea.Geo, "shp");
            ViewBag.ParentAreaCoordinates = mapCollection.Items[0].C;
            
            return View("AreaNew");
        }
        
        [HttpPost, CfAuthorize, CountryInflate(RedirectOnFailActionName = "AreaNewPrestep")]
        public ActionResult AreaNewCreate(string countryUrlPart, AreaNewViewModel m)
        {
            //-- this is a bit hacky to remove the validation required in step 1 and not have it cause out model to be invalid here.
            ModelState.Remove("locality");

            //-- Double check for duplicate
            var existingArea = geoSvc.GetArea(country.ID, m.Name.ToUrlFriendlyString());
            if (existingArea != default(Area))
            {
                ModelState.AddModelError("Name", 
                    string.Format("There is already an area with the name [{0}] in {1} listed in our database, if you are trying to add a city inside a province with the name name e.g. 'New York', change the name to 'New York, New York'",
                        m.Name, country.Name));
            }
            
            if (ModelState.IsValid)
            {
                var area = new Area();
                area.InjectFrom(m);
                area.Geo = SqlGeography.Parse(m.WKT);
                geoSvc.CreateArea(area);
                return Redirect(area.SlugUrl);
            }
            else
            {
                InitializeAreaNewViewData(country, (CfType)m.TypeID, new List<Area>());
                m.locality = "DuplicateRetry";
                return View("AreaNew", m);
            }
        }

        [HttpPost, CfAuthorize]
        public JsonResult SearchAreaForNewArea(string parentAreaID, string countryUrlPart, string locality)
        {
            var country = AppLookups.Country(countryUrlPart);

            Guid parentID;
            Area parentArea = default(Area);
            if (Guid.TryParse(parentAreaID, out parentID))
            {
                parentArea = geoSvc.GetAreaByID(parentID);
            }

            List<GeocodeResult> results = new List<GeocodeResult>();
            if (parentArea == default(Area)) { results = new GeocodeService().Geocode(country, locality); }
            else {
                //-- we need to blur the geography otherwise places that are coast to the coast fail...
                SqlGeography blurredGeo = parentArea.Geo.STBuffer(1500); //-- 1.5Km buffer zone
                results = new GeocodeService().Geocode(country, locality, blurredGeo); 
            }

            if (results.Count == 0) { return Json(new { Success = false }); }
            else { return Json(new { Success = true, Results = results }); }
        }
    }

}
