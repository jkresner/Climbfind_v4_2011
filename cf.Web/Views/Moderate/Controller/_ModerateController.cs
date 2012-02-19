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
using cf.Dtos;

namespace cf.Web.Controllers
{
    [CfAuthorize(Roles = "ModCommunity,ModSenior,ModAdmin", PopulateModDetails=true)]
    public partial class ModerateController : BaseCfController
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MappingService mappingSvc { get { if (_mappingSvc == null) { _mappingSvc = new MappingService(); } return _mappingSvc; } } MappingService _mappingSvc;

        public ActionResult Index() 
        {
            ViewBag.ModeratorProfile = CfPrincipal.ModDetails;
            ViewBag.ModeratorPlaces = GetModsPlaces(PlaceCategory.Unknown).OrderByDescending(c=>c.LastChangedUtc).ToList();

            var latestActions = geoSvc.GetLastHundredActions();
            ViewBag.LastHundredActions = latestActions;

            var modActions = geoSvc.GetModeratorsActions(CfIdentity.UserID);
            ViewBag.ModeratorActions = modActions;

            var monthInt = DateTime.Now.Month;
            var yearInt = DateTime.Now.Year;

            ViewBag.PointThisMonth = modActions.Where( a=>a.Utc.Month == monthInt && a.Utc.Year == yearInt).Sum(a=>a.Points);
            
            return View(); 
        }

        public ActionResult Edit(Guid id)
        {
            CfCacheIndexEntry place = AppLookups.GetCacheIndexEntry(id);

            if (place == null) { return new PlacesController().PlaceNotFound(); }
            else if (place.Type == CfType.ClimbIndoor) { return RedirectToAction("ClimbIndoorEdit", new { id = id }); }
            else if (place.Type == CfType.ClimbOutdoor) { return RedirectToAction("ClimbOutdoorEdit", new { id = id }); }
            else
            {
                var category = place.Type.ToPlaceCateogry();
                if (category == PlaceCategory.Area) { return RedirectToAction("AreaEdit", new { id = id }); }
                else if (category == PlaceCategory.IndoorClimbing) { return RedirectToAction("LocationIndoorEdit", new { id = id }); }
                else if (category == PlaceCategory.OutdoorClimbing) { return RedirectToAction("LocationOutdoorEdit", new { id = id }); }
                throw new Exception(string.Format("Category [{0}] from place [{1}][2] not supported by moderate edit", category, place.Name, place.IDstring));
            }
        }

        private List<PlaceWithModDetails> GetModsPlaces(PlaceCategory category)
        {
            //-- If the last action ID == noCreateActionID, it means it was either system added (taken from CF3) or is indoor climb which we
            //-- don't want to track in the moderator system

            var noCreateActionID = new Guid("00000000-0000-0000-0000-000000000001");
            var modPlaces = geoSvc.GetModeratorsClaimedObjects(CfIdentity.UserID);
            var modPlacesWDetails = 
                (from c in modPlaces select new PlaceWithModDetails(AppLookups.GetCacheIndexEntry(c.ID), c)).Where( pwd=>!pwd.PlaceDeleted);//&& pwd.LastChangedActionID != noCreateActionID 

            if (category == PlaceCategory.Unknown) { return modPlacesWDetails.ToList(); }
            else { return modPlacesWDetails.Where(a => a.Type.ToPlaceCateogry() == category).ToList(); }
        }

        public ActionResult ClaimPage(Guid id)
        {
            var p = AppLookups.GetCacheIndexEntry(id);
            
            geoSvc.ClaimObject(p);

            if (p.Type == CfType.ClimbIndoor || p.Type == CfType.ClimbOutdoor) { return RedirectToAction("Climbs"); }
            else if (p.Type.ToPlaceCateogry() == PlaceCategory.Area) { return RedirectToAction("Areas"); }
            else if (p.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing) { return RedirectToAction("LocationsIndoor"); }
            else if (p.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { return RedirectToAction("LocationsOutdoor"); }
            return RedirectToAction("Index");
        }

        public ActionResult UnclaimPage(Guid id)
        {
            var p = AppLookups.GetCacheIndexEntry(id);
            geoSvc.UnclaimObject(p);
            return RedirectToAction("Index");
        }
    }
}
