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
        public ActionResult ActionContribs()
        {
            return View();
        }
        
        public ActionResult ActionDetail(Guid id)
        {
            if (id == new Guid("00000000-0000-0000-0000-000000000001")) { return View(); }
            else {
                var modAction = geoSvc.GetModAction(id);
                ViewBag.ModAction = modAction;
                ViewBag.ModProfile = geoSvc.GetModProfile(modAction.UserID);
                ViewBag.ModPlace = geoSvc.GetObjectModeMeta(modAction.OnObjectID);
                return View();
            }
        }

        public ActionResult ActionPlaceList(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);
            var modPlace = geoSvc.GetObjectModeMeta(id);

            ViewBag.ModActions = geoSvc.GetModeratorActionsOnObject(id);
            ViewBag.ModPlace = modPlace;
            ViewBag.Place = place;
            return View();
        }

        public ActionResult ActionUserList(Guid id)
        {
            var modActions = geoSvc.GetModeratorsActions(id).Reverse();
            ViewBag.ModActions = modActions;
            ViewBag.ModProfile = geoSvc.GetModProfile(id);

            var monthInt = DateTime.Now.Month;
            var yearInt = DateTime.Now.Year;
            ViewBag.PointThisMonth = modActions.Where(a => a.Utc.Month == monthInt && a.Utc.Year == yearInt).Sum(a => a.Points);

            return View();
        }    
    }
}
