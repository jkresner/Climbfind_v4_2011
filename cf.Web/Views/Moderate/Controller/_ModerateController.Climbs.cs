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
using System.IO;
using NetFrameworkExtensions.Net;
using cf.Content.Images;
using NetFrameworkExtensions;
using System.Globalization;

namespace cf.Web.Controllers
{
    public partial class ModerateController
    {
        [HttpPost]
        public JsonResult SaveClimbClimbingImageFromWebUrl(Guid imgObjID, string originalImgUrl, int x, int y, int w, int h)
        {
            return CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(originalImgUrl, 
                stream => 
                    string.Format(@"{0}{1}{2}", Stgs.ImgsRt, ImageManager.ClimbPath, 
                        geoSvc.SaveClimbAvatar((Climb)geoSvc.GetClimbByID(imgObjID), 
                            stream, new ImageCropOpts(x, y, w, h)).Content));
        }

        public ActionResult Climbs()
        {
            var modObjects = GetModsPlaces(PlaceCategory.Climb);
            ViewBag.ModeratorClimbs = modObjects;
            ViewBag.ModeratorActions = geoSvc.GetModeratorActionsOnObjects(modObjects.Select(mp => mp.ID).ToList());
            return View();
        }

        [HttpGet]
        public ActionResult ClimbEdit(Guid id)
        {
            var cachedClimb = AppLookups.GetCacheIndexEntry(id);
            if (cachedClimb.Type == CfType.ClimbIndoor) { return ClimbIndoorEdit(id); }
            else if (cachedClimb.Type == CfType.ClimbOutdoor) { return ClimbOutdoorEdit(id); }
            else { throw new Exception("ClimbEdit not a valid climb type"); }
        }

        [HttpGet]
        public ActionResult ClimbIndoorEdit(Guid id)
        {
            var climb = geoSvc.GetIndoorClimbByID(id);
            ViewBag.Climb = climb;

            var loc = geoSvc.GetLocationIndoorByID(climb.LocationID);
            ViewBag.Location = loc;

            return RedirectToAction("ClimbIndoorNew", "Climbs", new { id = loc.ID });
        }

        [HttpGet]
        public ActionResult ClimbOutdoorEdit(Guid id)
        {
            var climb = geoSvc.GetOutdoorClimbByID(id);
            ViewBag.Climb = climb;

            ViewBag.Location = geoSvc.GetLocationByID(climb.LocationID);

            var climbImageToDisplay = (climb.AvatarRelativeUrl != string.Empty)
                ? Stgs.ImgsRt + climb.AvatarRelativeUrl
                : Stgs.DefaultMapInfoImage;

            ViewBag.ClimbingImageToDisplayUrl = climbImageToDisplay;

            var model = new ClimbOutdoorEditViewModel();
            model.InjectFrom(climb);
            model.Categories = climb.ClimbTags.Select(c => c.Category).ToList();

            return View("ClimbOutdoorEdit", model);
        }

        [HttpPost]
        public ActionResult ClimbOutdoorEdit(Guid id, ClimbOutdoorEditViewModel m)
        {
            var climb = geoSvc.GetOutdoorClimbByID(id);
            var original = climb.GetSimpleTypeClimbClone();

            if (ModelState.IsValid)
            {
                climb.InjectFrom(m);

                geoSvc.UpdateClimbOutdoor(original, climb, m.Categories);

                return Redirect(climb.SlugUrl);
            }
            else
            {
                return View(m);
            }
        }

        [HttpGet]
        public ActionResult ClimbDelete(Guid id)
        {
            var c = CfCacheIndex.Get(id);

            if (c == null) { throw new ArgumentException("Delete climb failed - no climb in cahce with id="+id); }
            else if (c.Type == CfType.ClimbIndoor) { geoSvc.DeleteClimbIndoor(geoSvc.GetIndoorClimbByID(id)); }
            else if (c.Type == CfType.ClimbOutdoor) { geoSvc.DeleteClimbOutdoor(geoSvc.GetOutdoorClimbByID(id)); }
            else { throw new Exception("Delete climb failed - unrecognized climb type"); }

            return RedirectToAction("Climbs");
        }
    }
}
