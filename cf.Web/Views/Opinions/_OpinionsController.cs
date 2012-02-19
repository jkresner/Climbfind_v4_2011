using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Entities.Enum;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using cf.Web.Views.Moderate;
using cf.Caching;
using cf.Services;
using cf.Web.Mvc.ActionFilters;
using cf.Identity;
using cf.Web.Views.Shared;
using cf.Web.Mvc.ViewData;


namespace cf.Web.Controllers
{
    public class OpinionsController : BaseCfController
    {
        ContentService ctnSvc { get { if (_ctnSvc == null) { _ctnSvc = new ContentService(); } return _ctnSvc; } } ContentService _ctnSvc;
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        
        public ActionResult TopAreas() 
        { 
            //var topAreas = new GeoService().GetAreasAll()
            
            return View(); 
        }
        public ActionResult TopIndoorLocations() { return View(); }
        public ActionResult TopOutdoorLocations() { return View(); }
        public ActionResult TopClimbs() { return View(); }

        [HttpGet]
        public ActionResult New(Guid id)
        {
            if (!CfIdentity.IsAuthenticated) { return PartialView("NewLogin"); }
            
            var model = new NewRatingViewModel() { RateObjectID = id };
            
            var existing = ctnSvc.GetOpinion(id, CfIdentity.UserID);

            if (existing != default(Opinion)) { 
                model.RateScore = existing.Rating; 
                model.RateComment = existing.Comment; }

            return PartialView(model);
        }

        [HttpPost, CfAuthorize]
        public JsonResult New(NewRatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var opinion = new Opinion() { Comment = model.RateComment, ObjectID = model.RateObjectID, Rating = model.RateScore };
                ctnSvc.CreateOpinion(opinion, model.RateObjectID);
                return Json(opinion);
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        public ActionResult Delete(Guid id)
        {
            var opinion = ctnSvc.GetOpinionByID(id);
            ctnSvc.DeleteOpinion(opinion);
            return RedirectToAction("ListUser", new { id = CfIdentity.UserID });
        }

        [CfAuthorize]
        public ActionResult ListUser(Guid id)
        {
            ViewBag.User = new UserService().GetProfileByID(id);
            ViewBag.Opinions = ctnSvc.GetUsersOpinions(id).OrderByDescending(o=>o.Utc).ToList();
            return View();
        }

        [CfAuthorize]
        public ActionResult ListObject(Guid id)
        {
            ViewBag.Place = AppLookups.GetCacheIndexEntry(id);
            ViewBag.Opinions = ctnSvc.GetOpinionsOnObject(id).OrderByDescending(o => o.Utc).ToList();
            return View();
        }


        [HttpGet, CfAuthorize]
        public ActionResult Detail(Guid id)
        {
            var opinion = ctnSvc.GetOpinionByID(id);
            ViewBag.Opinion = opinion;
            ViewBag.Post = new PostService().GetPostByID(opinion.ID);
            ViewBag.User = CfPerfCache.GetClimber(opinion.UserID);

            return View();
        }
    }
}

