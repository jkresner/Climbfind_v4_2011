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
using cf.Dtos;
using Omu.ValueInjecter;
using cf.Entities.Interfaces;
using System.Globalization;
using cf.Identity;

namespace cf.Web.Controllers
{
    [CfAuthorize]
    public partial class ClimbsController : BaseCfController
    {
        protected Country country { get { return ViewBag.Country as Country; } }
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MediaService medSvc { get { if (_medSvc == null) { _medSvc = new MediaService(); } return _medSvc; } } MediaService _medSvc;

        public ActionResult PlaceNotFound() { return new HttpStatusCodeWithBodyResult("PlaceNotFound", 404); }
                
        public ActionResult ClimbNewPrestep()
        {
            return View();
        }

        public ActionResult ClimbNew(Guid id)
        {
            var cacheLoc = AppLookups.GetCacheIndexEntry(id);
            if (cacheLoc.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing) { return RedirectToAction("ClimbIndoorNew" , new { id = id }); }
            else if (cacheLoc.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { return ClimbOutdoorNew(id); }
            else { throw new Exception("ClimbNew not a valid climb type"); }
        }

        [HttpGet]
        public ActionResult ClimbIndoorNew(Guid id)
        {
            ViewBag.Location = CfCacheIndex.Get(id);
            return View("ClimbIndoorNew");
        }

        [CfAuthorize]
        public ActionResult ClimbPhotosEdit(Guid id)
        {
            var climb = geoSvc.GetClimbByID(id);
            if (climb == default(Climb)) { return PlaceNotFound(); }
            ViewBag.Climb = climb;

            var climbImageToDisplay = (climb.AvatarRelativeUrl != string.Empty)
                ? Stgs.ImgsRt + climb.AvatarRelativeUrl
                : Stgs.DefaultMapInfoImage;

            ViewBag.ClimbingImageToDisplayUrl = climbImageToDisplay;

            ViewBag.ClimbPhotos = new MediaService().GetObjectsMedia(climb.ID).Where(m => m.TypeID == (byte)MediaType.Image).ToList();

            return View();
        }

        [CfAuthorize, HttpPost]
        public ActionResult SaveMediaAsClimbAvatar(Guid id, Guid mediaID)
        {
            var climb = geoSvc.GetClimbByID(id);
            if (climb == default(Climb)) { return PlaceNotFound(); }

            var media = medSvc.GetMediaByID(mediaID);

            geoSvc.SaveMediaAsClimbAvatar(climb, media, null);

            return Json(new { Success = "true" });
        }
  
        [HttpGet, CfAuthorize]
        public ActionResult ClimbOutdoorNew(Guid id)
        {
            var location = geoSvc.GetLocationOutdoorByID(id);
            if (location == default(LocationOutdoor)) { return RedirectToAction("ClimbNewPrestep"); }

            var model = new ClimbOutdoorNewViewModel() { LocationID = location.ID, ClimbTerrainID = 1, NumberOfPitches = 1 };

            ViewBag.Location = location;

            return View("ClimbOutdoorNew", model);
        }

        [HttpPost, CfAuthorize]
        public ActionResult ClimbOutdoorNew(Guid id, ClimbOutdoorNewViewModel m)
        {
            var location = geoSvc.GetLocationOutdoorByID(id);
            if (location == default(LocationOutdoor)) { return RedirectToAction("ClimbNewPrestep"); }

            if (ModelState.IsValid)
            {
                var climb = new ClimbOutdoor();
                climb.InjectFrom(m);
                climb.CountryID = location.CountryID;
                
                geoSvc.CreateClimbOutdoor(climb, m.Categories);

                return Redirect(climb.SlugUrl);
            }
            else
            {
                return View("ClimbOutdoorNew", m);
            }
        }


        public ActionResult ClimbDetail(Guid id, string nameUrlPart)
        {
            var cacheClimb = AppLookups.GetCacheIndexEntry(id);
            ViewBag.Country = AppLookups.Country(cacheClimb.CountryID);

            if (cacheClimb.Type == CfType.ClimbIndoor) { return ClimbIndoorDetail(id, nameUrlPart); }
            else if (cacheClimb.Type == CfType.ClimbOutdoor) { return ClimbOutdoorDetail(id, nameUrlPart); }
            else
            {
                throw new Exception("ClimbDetail not a valid climb type");
            }
        }

        public ActionResult ClimbSectionDetail(Guid id, string sectionNameUrlPart)
        {
            var location = geoSvc.GetLocationIndoorByID(id);
            ViewBag.Current = location;

            var sections = location.LocationsSections.ToList();
            ViewBag.Sections = sections;
            var sec = sections.Where(s => s.NameUrlPart == sectionNameUrlPart).Single();
            ViewBag.Section = sec;

            ViewBag.Climbs = geoSvc.GetClimbsOfLocation(location.ID).Where(c=>c.SectionID == sec.ID).ToList();

            return View();
        }

        private void SetClimbPageViewData(Climb climb)
        {
            var location = geoSvc.GetLocationByID(climb.LocationID);
            ViewBag.Current = location;

            SetPlaceContextToViewData(location);

            ViewBag.RecentSends = new VisitsService().GetClimbsRecentSuccessfulLoggedClimbs(climb.ID, 10).ToList();

            var allClimbsAtLocation = geoSvc.GetClimbsOfLocation(climb.LocationID).ToList();
            ViewBag.OtherClimbs = allClimbsAtLocation.Where(c => c.ID != climb.ID).ToList();
            ViewBag.OtherClimbsInSection = allClimbsAtLocation.Where(c=>c.SectionID == climb.SectionID).ToList();
            
            //-- ClimbTypeID == ClimbTypeID
            ViewBag.OtherClimbsSimilar = allClimbsAtLocation.Where(c => 
                c.GradeCfNormalize > (climb.GradeCfNormalize - 15) &&
                c.GradeCfNormalize < (climb.GradeCfNormalize + 15) ).ToList();

            var meta = geoSvc.GetObjectModMetaOrSystemCreate(location);
            ViewBag.ModMeta = meta;
            ViewBag.Posts = new List<PostRendered>(); //Todo fix

            ViewBag.MediaList = medSvc.GetObjectsTopMedia(climb.ID, 10);

            ViewBag.LatestOpinions = new ContentService().GetLatestOpinionsOnObject(climb.ID, 20).ToList();

            ViewBag.PageQR = string.Format("http://c/{0}", climb.ID.ToString("N"));
        }


        /// <summary>
        /// WHAT A MESS... this is here because you can't set viewbag from another controller.
        /// It's probably worth moving the whole climbs controller back into the places controller...
        /// </summary>
        /// <param name="location"></param>
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


        [ChildActionOnly]
        public ActionResult ClimbIndoorDetail(Guid id, string nameUrlPart)
        {
            var climb = geoSvc.GetIndoorClimbByID(id);
            if (climb == default(ClimbIndoor)) { return PlaceNotFound(); }
            ViewBag.Climb = climb;
            ViewBag.LocationSections = geoSvc.GetLocationSections(climb.LocationID).ToList();

            SetClimbPageViewData(climb);

            return View("ClimbIndoorDetail");
        }

        [ChildActionOnly]
        public ActionResult ClimbOutdoorDetail(Guid id, string nameUrlPart)
        {
            var climb = geoSvc.GetOutdoorClimbByID(id);
            if (climb == default(ClimbOutdoor)) { return PlaceNotFound(); }
            ViewBag.Climb = climb;

            SetClimbPageViewData(climb);

            return View("ClimbOutdoorDetail");
        }
    }
}
