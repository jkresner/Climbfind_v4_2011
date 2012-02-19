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
using Omu.ValueInjecter;
using System.IO;
using cf.Content.Images;
using NetFrameworkExtensions.Web;
using NetFrameworkExtensions.Web.Mvc;
using NetFrameworkExtensions.Net;
using NetFrameworkExtensions.SqlServer.Types;

namespace cf.Web.Controllers
{
    public partial class ModerateController
    {
        public ActionResult Areas()
        {
            var modPlaces = GetModsPlaces(PlaceCategory.Area);

            ViewBag.ModeratorPlaces = modPlaces;

            ViewBag.ModeratorActions = geoSvc.GetModeratorActionsOnObjects(modPlaces.Select(mp => mp.ID).ToList());

            return View("Areas");
        }

        [HttpGet]
        public ActionResult AreaEdit(Guid id)
        {
            //-- TODO Put error check
            var area = geoSvc.GetAreaByID(id);
            if (area == null) { return new PlacesController().PlaceNotFound(); }
            
            ViewBag.Area = area;

            var geoJsonUrl = Stgs.MapSvcRelativeUrl + "area/" + id.ToString();
            var mapModel = new Bing7GeoJsonMapViewModel("climbing-map-" + id, 680, 400, geoJsonUrl);
            mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mappingSvc.GetBingViewByID(area.ID));
 
            ViewBag.PlaceTypeDropDownList = cf.Web.Mvc.ViewData.SelectLists.GetAreaTypeSelectList(false);

            ViewBag.MapImageToDisplayUrl = (area.AvatarRelativeUrl != string.Empty)
                ? Stgs.ImgsRt + area.AvatarRelativeUrl
                : Stgs.DefaultMapInfoImage;

            var mapItems = new MappingService().GetAreaEditMapItems(area);
            ViewBag.MapItemsArea = mapItems.Items[0];
            mapItems.Items.RemoveAt(0);
            ViewBag.MapItemsLocations = mapItems.Items;

            return View(new AreaEditViewModel()
            {
                ID = area.ID,
                WKT = area.Geo.GetWkt(),
                GeoReduceThreshold = area.GeoReduceThreshold,
                SearchSupportString = area.SearchSupportString,
                PlaceType = area.Type.ToString(),
                Name = area.Name,
                NameShort = area.NameShort,
                NameUrlPart = area.NameUrlPart,
                Description = area.Description,
                NoIndoorConfirmed = area.NoIndoorConfirmed,
                MapModel = mapModel
            });
        }
                
        [HttpPost]
        public ActionResult AreaEdit(Guid id, AreaEditViewModel m)
        {
            var area = geoSvc.GetAreaByID(id);
            var original = area.GetCloneWithGeo();

            if (ModelState.IsValid)
            {
                area.TypeID = (byte)Enum.Parse(typeof(CfType), m.PlaceType);
                area.SearchSupportString = m.SearchSupportString ?? "";
                area.GeoReduceThreshold = m.GeoReduceThreshold;
                area.Geo = SqlGeography.Parse(new SqlString(m.WKT));
                area.Name = m.Name;
                area.NameUrlPart = m.NameUrlPart;
                area.NameShort = m.NameShort;
                area.NoIndoorConfirmed = m.NoIndoorConfirmed;
                area.Description = m.Description;

                SaveBing7MapViewFromModel(m.MapModel.ViewOptions, area.ID);

                geoSvc.UpdateArea(original, area);
                return Redirect(area.SlugUrl);
            }
            else
            {
                ViewBag.Area = area;

                var geoJsonUrl = Stgs.MapSvcRelativeUrl + "area/" + id.ToString();
                var mapModel = new Bing7GeoJsonMapViewModel("climbing-map-" + id, 680, 400, geoJsonUrl);
                mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mappingSvc.GetBingViewByID(area.ID));
                ViewBag.MapModel = mapModel;

                ViewBag.PlaceTypeDropDownList = SelectLists.GetAreaTypeSelectList(false);

                return View(m);
            }
        }

        private void SaveBing7MapViewFromModel(Bing7MapViewOptionsViewModel model, Guid placeID)
        {
            var placeBingMapView = new PlaceBingMapView();
            placeBingMapView.InjectFrom(model);
            if (!string.IsNullOrWhiteSpace(model.ID)) { placeBingMapView.ID = new Guid(model.ID); }
            if (placeBingMapView.ID == Guid.Empty)
            {
                placeBingMapView.ID = placeID;
                mappingSvc.CreateBingView(placeBingMapView);
            }
            else
            {
                mappingSvc.UpdateBingView(placeBingMapView);
            }
        }

        [HttpGet]
        public ActionResult AreaVerify(Guid id)
        {
            var area = geoSvc.GetAreaByID(id);
            ViewBag.Area = area;
            return View();
        }

        [HttpPost]
        public JsonResult SaveAreaMapImageFromWebUrl(Guid imgObjID, string originalImgUrl, int x, int y, int w, int h)
        {
            return CropAndSaveImageFromUrlAndDeleteOriginalAtUrl( originalImgUrl, 
                stream => geoSvc.SaveAreaAvatar(geoSvc.GetAreaByID(imgObjID), stream, new ImageCropOpts(x, y, w, h)));
        }

        [HttpGet]
        public ActionResult AreaDelete(Guid id)
        {
            var area = geoSvc.GetAreaByID(id);
            geoSvc.DeleteArea(area);
            return RedirectToAction("Areas");
        }
    }
}
