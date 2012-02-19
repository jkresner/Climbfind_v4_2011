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
using cf.Web.Mvc.ViewData;
using cf.Entities.Interfaces;
using GeoExtensions = NetFrameworkExtensions.SqlServer.Types.SqlGeographyExtensions;
using cf.Dtos;
using cf.Identity;

namespace cf.Web.Controllers
{
    public partial class PlacesController : Controller
    {
        [CfAuthorize]
        public ActionResult LocationNewPrestep(string countryUrlPart, string type) 
        {
            if (countryUrlPart == "unknown")
            {
                // allow the user to select the country?
            }

            if (type == "unknown")
            {
                // allow the user to select the country?
            }

            return View(); 
        }

        [HttpGet, CfAuthorize, CountryInflate(RedirectOnFailActionName = "LocationNewPrestep")]
        public ActionResult LocationOutdoorNew(string countryUrlPart, string areaNameUrlPart)
        {
            var area = PrepareViewDataForLocationOutdoorNew(countryUrlPart, areaNameUrlPart);
            if (area == default(Area)) { return RedirectToAction("LocationNewPrestep"); }
            
            return View();
        }

        private Area PrepareViewDataForLocationOutdoorNew(string countryUrlPart, string areaNameUrlPart)
        {
            var area = geoSvc.GetArea(country.ID, areaNameUrlPart);
            if (area == default(Area)) { return default(Area); }
            ViewBag.Area = area;

            List<Location> locationsOfTypeAlreadyInArea = geoSvc.GetLocationsOfArea(area.ID).Where(
              a => a.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing).ToList();

            ViewBag.ExistingLocations = locationsOfTypeAlreadyInArea;

            var mapItems = new MappingService().GetAreaEditMapItems(area);
            ViewBag.MapItemsArea = mapItems.Items[0];
            mapItems.Items.RemoveAt(0);
            ViewBag.MapItemsLocations = mapItems.Items;

            return area;
        }

        [HttpPost, CfAuthorize, CountryInflate(RedirectOnFailActionName = "LocationNewPrestep")]
        public ActionResult LocationOutdoorNew(string countryUrlPart, string areaNameUrlPart, LocationOutdoorNewViewModel m)
        {
            PerformLocationAddValidation(areaNameUrlPart, m.Latitude, m.Longitude, m.Name, AppLookups.Country(countryUrlPart), false);
                       
            if (ModelState.IsValid)
            {
                var location = geoSvc.CreateLocationOutdoor(new LocationOutdoor()
                {
                    CountryID = m.CountryID,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Name = m.Name,
                    TypeID = (byte)m.Type
                });

                var view = mappingSvc.CreateBingView(new PlaceBingMapView()
                {
                    ID = location.ID,
                    Bounds = m.ViewOptions.Bounds,
                    CenterOffset = m.ViewOptions.CenterOffset,
                    Heading = m.ViewOptions.Heading,
                    MapCenterLatitude = m.ViewOptions.MapCenterLatitude,
                    MapCenterLongitude = m.ViewOptions.MapCenterLongitude,
                    MapTypeId = m.ViewOptions.MapTypeId,
                    Zoom = m.ViewOptions.Zoom
                });

                return Redirect(location.SlugUrl);
            }
            else
            {
                PrepareViewDataForLocationOutdoorNew(countryUrlPart, areaNameUrlPart);
                return View("LocationOutdoorNew");
            }
        }

        private void PerformLocationAddValidation(string areaNameUrlPart, double lat, double lon, string locationName, Country country, bool forIndoor)
        {
            //-- Check lat/lon is valid
            var parentArea = geoSvc.GetArea(country.ID, areaNameUrlPart);

            //-- we need to blur the geography otherwise places that are coast to the coast fail...
            SqlGeography blurredGeo = parentArea.Geo.STBuffer(1500); //-- 1.5Km buffer zone

            if (!blurredGeo.STIntersects(GeoExtensions.GetGeoPoint(lat, lon)))
            {
                ModelState.AddModelError("Latitude",
                    string.Format("The location you've plotted does not fall in the area you chose to add it to. If the location is correct, either choose a different area, or if the area is correct, first edit the areas boundaries and then try again.",
                        locationName, country.Name));
            }

            //-- Double check for duplicate
            Guid existing = Guid.Empty;
            if (forIndoor) { var result = geoSvc.GetLocationIndoor(country.ID, locationName.ToUrlFriendlyString()); if (result != null) { existing = result.ID; } }
            else { var result = geoSvc.GetLocationOutdoor(country.ID, locationName.ToUrlFriendlyString()); if (result != null) { existing = result.ID; } }

            if (existing != Guid.Empty)
            {
                var objModMeta = geoSvc.GetObjectModeMeta(existing);
                if (objModMeta.CreatedByUserID == CfIdentity.UserID)
                {
                    ModelState.AddModelError("Name",
                        string.Format("You've already added location with the name [{0}] in {1} to our database at {2} GMT. If this is was a few seconds ago you're browser may have tried to save your work more than once.",
                            locationName, country.Name, objModMeta.CreatedUtc));
                }
                else
                {
                    ModelState.AddModelError("Name",
                        string.Format("There is already a location with the name [{0}] in {1} listed in our database added by another user on {2} GMT, please double check you are not adding a duplicate",
                            locationName, country.Name, objModMeta.CreatedUtc));
                }
            }
        }


        [HttpGet, CfAuthorize, CountryInflate]
        public ActionResult LocationIndoorNew(string countryUrlPart, string areaNameUrlPart)
        {
            var area = geoSvc.GetArea(country.ID, areaNameUrlPart);
            if (area == default(Area)) { return RedirectToAction("LocationNewPrestep"); }
            ViewBag.Area = area;

            List<Location> locationsOfTypeAlreadyInArea = geoSvc.GetLocationsOfArea(area.ID).Where(
                a => a.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();

            ViewBag.ExistingLocations = locationsOfTypeAlreadyInArea;
            
            return View();
        }

        [HttpPost, CfAuthorize]
        public ActionResult LocationIndoorNew(string countryUrlPart, string areaNameUrlPart, LocationIndoorNewViewModel m)
        {
            PerformLocationAddValidation(areaNameUrlPart, m.Latitude, m.Longitude, m.Name, AppLookups.Country(countryUrlPart), true);
            
            if (ModelState.IsValid)
            {
                var location = geoSvc.CreateLocationIndoor(new LocationIndoor()
                    {
                        Address = m.Address,
                        MapAddress = m.Address,
                        CountryID = m.CountryID,
                        Latitude = m.Latitude,
                        Longitude = m.Longitude,
                        Name = m.Name,
                        Website = m.Website,
                        TypeID = m.TypeID
                    });

                return Redirect(location.SlugUrl);
            }
            else
            {
                var country = AppLookups.Country(countryUrlPart);
                ViewBag.Country = country;
                var area = new GeoService().GetArea(country.ID, areaNameUrlPart);
                ViewBag.Area = area;
                ViewBag.ExistingLocations = new GeoService().GetLocationsOfArea(area.ID).Where(
                    a => a.Type == CfType.CommercialIndoorClimbing || a.Type == CfType.PrivateIndoorClimbing).ToList();

                return View(m);
            }
        }


        [HttpPost, CfAuthorize]
        public JsonResult SearchAreaForNewLocationIndoor(string countryUrlPart, string locality)
        {
            //-- Step 1) Make a GeoCoding call to Bing
            var country = AppLookups.Country(countryUrlPart);

            List<GeocodeResult> results = new GeocodeService().Geocode(country, locality);

            if (results.Count == 0) { return Json(new { Success = false }); }

            return Json(new { Success = true, Results = results });
        }


        [CountryInflate]
        public ActionResult LocationIndoorDetail(string countryUrlPart, string locationNameUrlPart)
        {
            var location = geoSvc.GetLocationIndoor(country.ID, locationNameUrlPart);
            if (location == null) { return PlaceNotFound(); }

            ViewBag.Current = location;

            //var modPlace = geoSvc.GetObjectModMetaOrSystemCreate(location);
            //ViewBag.ModPlaceDetails = modPlace;

            ViewBag.Posts = new PostService().GetPostForLocation(location.ID, PostType.Unknown, ClientAppType.CfWeb);

            var intersectingAreas = geoSvc.GetIntersectingAreas(location);
            ViewBag.Provinces = intersectingAreas.Where(c => c.Type == CfType.Province).ToList();
            ViewBag.Cities = intersectingAreas.Where(c => c.Type == CfType.City).ToList();

            var closestLocations = geoSvc.GetClosestLocationsOfLocation(location.ID);
            ViewBag.ClosestLocations = closestLocations;

            var mapViewSettings = mappingSvc.GetBingViewByID(location.ID);
            if (mapViewSettings == default(PlaceBingMapView)) { mapViewSettings = PlaceBingMapView.GetDefaultIndoorSettings(location); }  
            
            var mapModel = new Bing7MapWithLocationViewModel(location.NameUrlPart, 486, 240, location.Latitude,
                location.Longitude, location.AvatarRelativeUrl);
            mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mapViewSettings);
            ViewBag.MapView = mapModel;

            ViewBag.Climbs = geoSvc.GetCurrentClimbsOfLocation(location.ID).ToList();
            ViewBag.MediaList = medSvc.GetObjectsTopMedia(location.ID, 11);

            ViewBag.PageQR = string.Format("http://i/{0}", location.ID.ToString("N"));

            ViewBag.LatestOpinions = new ContentService().GetLatestOpinionsOnObject(location.ID, 20).ToList();

            return View();
        }


        [CountryInflate]
        public ActionResult LocationOutdoorDetail(string countryUrlPart, string locationTypeUrlPart, string locationNameUrlPart)
        {
            var location = geoSvc.GetLocationOutdoor(country.ID, locationNameUrlPart);
            if (location == null) { return PlaceNotFound(); }
            ViewBag.Current = location;

            //var modPlace = geoSvc.GetObjectModMetaOrSystemCreate(location);
            //ViewBag.ModPlaceDetails = modPlace;
            ViewBag.Posts = new PostService().GetPostForLocation(location.ID, PostType.Unknown, ClientAppType.CfWeb);
                       
            SetPlaceContextToViewData(location);

            var mapViewSettings = mappingSvc.GetBingViewByID(location.ID);
            var mapModel = new Bing7MapWithLocationViewModel(location.NameUrlPart, 732, 340, location.Latitude,
                location.Longitude, location.AvatarRelativeUrl);
            mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mapViewSettings);
            ViewBag.MapView = mapModel;

            ViewBag.MediaList = medSvc.GetObjectsTopMedia(location.ID, 10);

            ViewBag.PageQR = string.Format("http://o/{0}", location.ID.ToString("N"));

            ViewBag.LatestOpinions = new ContentService().GetLatestOpinionsOnObject(location.ID, 20).ToList();

            return View();
        }


        public void SetPlaceContextToViewData(ILocation location)
        {
            var relatedAreas = geoSvc.GetRelatedAreas(location);
            ViewBag.Provinces = relatedAreas.Where(c => c.Type == CfType.Province).ToList();
            ViewBag.Cities = relatedAreas.Where(c => c.Type == CfType.City).ToList();
            var climbingAreas = relatedAreas.Where(c => c.Type == CfType.ClimbingArea).ToList();
            ViewBag.ClimbingAreas = climbingAreas;

            var parentAreas = location.GetParentAreas(climbingAreas);
            ViewBag.ParentClimbingAreas = parentAreas;

            var closestLocations = geoSvc.GetClosestLocationsOfLocation(location.ID);
            ViewBag.ClosestLocations = closestLocations;

            ViewBag.Climbs = geoSvc.GetClimbsOfLocation(location.ID).ToList();
        }
    }
}
