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
using NetFrameworkExtensions.Net;
using cf.Content.Images;
using System.IO;

namespace cf.Web.Controllers
{
    public partial class ModerateController
    {
        [HttpPost]
        public JsonResult SaveLocationIndoorLogoImageFromWebUrl(Guid imgObjID, string originalImgUrl, int x, int y, int w, int h)
        {
            return CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(originalImgUrl,
                stream => geoSvc.SaveLocationIndoorLogo(geoSvc.GetLocationIndoorByID(imgObjID), stream, new ImageCropOpts(x, y, w, h)));
        }

        [HttpPost]
        public JsonResult SaveLocationAvatarFromWebUrl(Guid imgObjID, string originalImgUrl, int x, int y, int w, int h)
        {
            var locTypePath = ImageManager.ClimbingIndoorPath;
            if (CfCacheIndex.Get(imgObjID).Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing)
            {
                locTypePath = ImageManager.ClimbingOutdoorPath;
            } 
            
            return CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(originalImgUrl,
                stream =>
                    string.Format(@"{0}{1}{2}", Stgs.ImgsRt, locTypePath, 
                        geoSvc.SaveLocationAvatar(geoSvc.GetLocationByID(imgObjID), stream, new ImageCropOpts(x, y, w, h)).Content
                    ));
        }

        public ActionResult LocationsIndoor()
        {
            var modPlaces = GetModsPlaces(PlaceCategory.IndoorClimbing);
            ViewBag.ModeratorPlaces = modPlaces;
            ViewBag.ModeratorActions = geoSvc.GetModeratorActionsOnObjects(modPlaces.Select(mp => mp.ID).ToList());
            return View();
        }

        public ActionResult LocationsOutdoor()
        {
            var modPlaces = GetModsPlaces(PlaceCategory.OutdoorClimbing);
            ViewBag.ModeratorPlaces = modPlaces;
            ViewBag.ModeratorActions = geoSvc.GetModeratorActionsOnObjects(modPlaces.Select(mp => mp.ID).ToList());
            return View();
        }

        [HttpGet]
        public ActionResult LocationIndoorEdit(Guid id)
        {
            var location = geoSvc.GetLocationIndoorByID(id);
            if (location == null) { return new PlacesController().PlaceNotFound(); }
            
            var model = new LocationIndoorEditViewModel();
            model.InjectFrom(location);

            SetLocationIndoorEditViewData(location);

            var climbImageToDisplay = (location.HasAvatar) ? Stgs.ImgsRt + location.AvatarRelativeUrl : Stgs.DefaultMapInfoImage;

            var mapViewSettings = mappingSvc.GetBingViewByID(location.ID);
            if (mapViewSettings == default(PlaceBingMapView)) { mapViewSettings = PlaceBingMapView.GetDefaultIndoorSettings(location); }           
            model.LatLongEditorModel = new Bing7MapWithLocationViewModel("myMap", 486, 240, location.Latitude, location.Longitude, climbImageToDisplay);
            model.LatLongEditorModel.ViewOptions = new Bing7MapViewOptionsViewModel(mapViewSettings);
                        
            return View(model);
        }

        private void SetLocationIndoorEditViewData(LocationIndoor location)
        {
            ViewBag.Location = location;
            ViewBag.LogoImageToDisplayUrl = (location.HasLogo) ? Stgs.ImgsRt + location.LogoRelativeUrl : Stgs.DefaultMapInfoImage;
            var climbImageToDisplay = (location.HasAvatar) ? Stgs.ImgsRt + location.AvatarRelativeUrl : Stgs.DefaultMapInfoImage;
            ViewBag.ClimbingImageToDisplayUrl = climbImageToDisplay;
        }

        [HttpPost]
        public ActionResult LocationIndoorEdit(Guid id, LocationIndoorEditViewModel m)
        {
            var location = geoSvc.GetLocationIndoorByID(id);
            var original = location.GetSimpleTypeClone();

            if (ModelState.IsValid)
            {
                location.InjectFrom(m);
                location.TypeID = (byte)m.Type;
                geoSvc.UpdateLocationIndoor(original, location);

                SaveBing7MapViewFromModel(m.LatLongEditorModel.ViewOptions, location.ID);

                return Redirect(location.SlugUrl);
            }
            else
            {
                SetLocationIndoorEditViewData(location);
                
                return View(m);
            }
        }

        [HttpGet]
        public ActionResult LocationIndoorDelete(Guid id)
        {
            var l = new GeoService().GetLocationIndoorByID(id);
            new GeoService().DeleteLocationIndoor(l);
            return RedirectToAction("LocationsIndoor");
        }

        [HttpGet]
        public ActionResult LocationOutdoorEdit(Guid id)
        {
            var location = new GeoService().GetLocationOutdoorByID(id);
            if (location == null) { return new PlacesController().PlaceNotFound(); }
            ViewBag.Location = location;

            var climbImageToDisplay = (location.AvatarRelativeUrl != string.Empty)
                ? Stgs.ImgsRt + location.AvatarRelativeUrl
                : Stgs.DefaultMapInfoImage;

            ViewBag.ClimbingImageToDisplayUrl = climbImageToDisplay;

            var model = new LocationOutdoorEditViewModel();
            model.InjectFrom(location);
            
            var mapView = mappingSvc.GetBingViewByID(location.ID);
            model.MapView = new Bing7MapWithLocationViewModel("myMap", 650, 340, location.Latitude, location.Longitude, climbImageToDisplay);
            model.MapView.ViewOptions = new Bing7MapViewOptionsViewModel(mapView); 

            return View(model);
        }

        [HttpPost]
        public ActionResult LocationOutdoorEdit(Guid id, LocationOutdoorEditViewModel m)
        {
            var location = geoSvc.GetLocationOutdoorByID(id);
            var original = location.GetSimpleTypeClone();

            if (ModelState.IsValid)
            {
                location.InjectFrom(m);
                location.TypeID = (byte)m.Type;
                geoSvc.UpdateLocationOutdoor(original, location);

                SaveBing7MapViewFromModel(m.MapView.ViewOptions, location.ID); ;

                return Redirect(location.SlugUrl);
            }
            else
            {
                return View(m);
            }
        }

        [HttpGet]
        public ActionResult LocationOutdoorDelete(Guid id)
        {
            var l = new GeoService().GetLocationOutdoorByID(id);
            geoSvc.DeleteLocationOutdoor(l);
            return RedirectToAction("LocationsOutdoor");
        }   
    }
}
