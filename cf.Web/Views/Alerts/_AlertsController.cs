using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Services;
using cf.Caching;
using cf.Entities;
using cf.Entities.Enum;
using cf.Identity;
using cf.Web.Views.Shared;
using System.Globalization;
using NetFrameworkExtensions;
using cf.Web.Models;
using cf.Dtos;
using Omu.ValueInjecter;

namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class AlertsController : Controller
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        PartnerCallService pcSvc { get { if (_pcSvc == null) { _pcSvc = new PartnerCallService(); } return _pcSvc; } } PartnerCallService _pcSvc;
        AlertsService alertsSvc { get { if (_alertsSvc == null) { _alertsSvc = new AlertsService(); } return _alertsSvc; } } AlertsService _alertsSvc;

        public ActionResult Index() 
        {
            Guid userID = CfIdentity.UserID;
            ViewBag.User = new UserService().GetProfileByID(userID);
            
            ViewBag.Alerts = alertsSvc.GetUsersLatestAlerts(CfIdentity.UserID, 30).ToList();
            return View(); 
        }

        public ActionResult SiteSettings()
        {
            ViewBag.AlertSettings = alertsSvc.GetUserSiteSettings(CfIdentity.UserID);
            return View();
        }

        [HttpGet]
        public ActionResult SiteSettingsEdit()
        {
            var settings = alertsSvc.GetUserSiteSettings(CfIdentity.UserID);
            var model = new UpdateSiteSettingsViewModel();
            model.InjectFrom(settings);

            return View(model);
        }


        [HttpPost]
        public ActionResult SiteSettingsEdit(UpdateSiteSettingsViewModel m)
        {
            var settings = alertsSvc.GetUserSiteSettings(CfIdentity.UserID);

            settings.InjectFrom(m);

            alertsSvc.UpdatedUserSiteSettings(settings);

            return RedirectToAction("SiteSettings");
        }

        [HttpPost]
        public ActionResult Create(NewPartnerCallViewModel m)
        {
            //ViewBag.PartnerCalls = pcSvc.GetPlacesPartnerCalls(id);
            return View();
        }

        public ActionResult ListLocation(Guid id)
        {
            ViewBag.Location = AppLookups.GetCacheIndexEntry(id);
            //ViewBag.PartnerCalls = pcSvc.GetPlacesPartnerCalls(id);
            return View();
        }

    }
}
