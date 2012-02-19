using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using cf.Entities;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using NetFrameworkExtensions.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Caching;
using NetFrameworkExtensions.Net;
using cf.Services;
using cf.Instrumentation;
using cf.Identity;
using cf.Web.Views.Media;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using Omu.ValueInjecter;
using cf.Content;
using System.Web.Script.Serialization;
using cf.Dtos;

namespace cf.Web.Controllers
{
    public class MediaController : BaseCfController
    {
        Country country { get { return ViewBag.Country as Country; } }
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MediaService mediaSvc { get { if (_mediaSvc == null) { _mediaSvc = new MediaService(); } return _mediaSvc; } } MediaService _mediaSvc;
        UserService usrSvc { get { if (_usrSvc == null) { _usrSvc = new UserService(); } return _usrSvc; } } UserService _usrSvc;

        public ActionResult PlaceNotFound()
        {
            return new HttpStatusCodeWithBodyResult("PlaceNotFound", 404);
        }

        [OutputCache(VaryByParam = "none", Duration = 60)]
        public ActionResult LatestCommunityMediaStrip()
        {
            try
            {
                var model = mediaSvc.GetLatestMedia(12);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                CfTrace.Error(ex);
                return Content("<p>Failed to load recent media. Refresh the page</p>");
            }
        }

        public ActionResult LatestCommunityMedia()
        {
            var model = new ViewerViewModel(Guid.Empty, "latest", "Most recently submitted", "/", mediaSvc.GetLatestMedia(60), "Most recently submitted");
            model.ShowAddMedia = false;

            return View("Viewer", model);
        }

        public ActionResult UsersSubmittedMedia(string id)
        {
            Guid userID = default(Guid);
            var user = default(Profile);
            if (Guid.TryParse(id, out userID)) { user = usrSvc.GetProfileByID(userID); }
            else { user = usrSvc.GetProfileBySlugUrlPart(id); }
            if (user == default(Profile)) { return new ProfilesController().ProfileNotFound(); }

            var medium = mediaSvc.GetUsersMediaWithOpinions(user.ID).ToList();
            var pageHeading = string.Format("by <a href='{0}'>{1}</a>", user.SlugUrl, user.DisplayName);

            var model = new ViewerViewModel(user.ID, "user", user.DisplayName, user.SlugUrl, medium, pageHeading);
            model.ShowAddMedia = false;

            return View("Viewer", model);
        }


        [CountryInflate]
        public ActionResult LocationOutdoor(string countryUrlPart, string locationTypeUrlPart, string locationNameUrlPart)
        {
            var l = geoSvc.GetLocationOutdoor(country.ID, locationNameUrlPart);
            if (l == null) { return PlaceNotFound(); }
            ViewBag.Current = l;

            var pageHeading = string.Format("of <a href='{0}'>{1}</a>", l.SlugUrl, l.Name);
            var model = new ViewerViewModel(l.ID, "outdoor", l.Name, l.SlugUrl, mediaSvc.GetObjectsMedia(l.ID), pageHeading);

            return View("Viewer", model);
        }

        [CountryInflate]
        public ActionResult LocationIndoor(string countryUrlPart, string locationNameUrlPart)
        {
            var l = geoSvc.GetLocationIndoor(country.ID, locationNameUrlPart);
            if (l == null) { return PlaceNotFound(); }

            var pageHeading = string.Format("of <a href='{0}'>{1}</a>", l.SlugUrl, l.Name);
            var model = new ViewerViewModel(l.ID, "indoor", l.Name, l.SlugUrl, mediaSvc.GetObjectsMedia(l.ID), pageHeading);

            return View("Viewer", model);
        }

        [CountryInflate]
        public ActionResult Area(string countryUrlPart, string nameUrlPart)
        {
            var a = geoSvc.GetArea(country.ID, nameUrlPart);
            if (a == null) { return PlaceNotFound(); }

            var pageHeading = string.Format("of <a href='{0}'>{1}</a>", a.SlugUrl, a.Name);
            var model = new ViewerViewModel(a.ID, "area", a.Name, a.SlugUrl, mediaSvc.GetObjectsMedia(a.ID), pageHeading);

            return View("Viewer", model);
        }

        public ActionResult Climb(string nameUrlPart, Guid id)
        {
            var c = geoSvc.GetClimbByID(id);
            if (c == default(Climb)) { return PlaceNotFound(); }

            ViewBag.Country = AppLookups.Country(c.CountryID);
            var pageHeading = string.Format("of <a href='{0}'>{1}</a>", c.SlugUrl, c.Name);
            var model = new ViewerViewModel(c.ID, "climb", c.Name, c.SlugUrl, mediaSvc.GetObjectsMedia(c.ID), pageHeading);

            return View("Viewer", model);
        }


        [HttpGet, CfAuthorize]
        public ActionResult Add(Guid id)
        {
            var obj = AppLookups.GetCacheIndexEntry(id);
            if (obj == null) { PlaceNotFound(); }
                        
            var model = new AddViewModel() { ObjectId = id, ObjectSlug = obj.SlugUrl, ObjectName = obj.Name, Title = "Climbing media of " + obj.Name, ChooseFromExisting = true };

            ViewBag.MyMedia = mediaSvc.GetMediaByUserWithObjectRefereces(CfIdentity.UserID).ToList();

            return View(model);
        }

        [HttpPost, CfAuthorize, ValidateInput(false)]
        public ActionResult Add(Guid id, AddViewModel model)
        {
            if (ModelState.IsValid)
            {
                SaveMediaWithTag(model);
                                
                return Redirect("/" + CfUrlProvider.MediaUrlPrefix + model.ObjectSlug);
            }
            else
            {
                return View(model);
            }
        }

        public Media SaveMediaWithTag(AddViewModel model)
        {
            var id = model.ObjectId;
            var type = model.Type;
            var media = new Media() { FeedVisible = true };
            media.InjectFrom(model);

            if (type == MediaType.Image)
            {
                CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(model.Content,
                    stream => mediaSvc.CreateImageMedia(media, id, stream).Content);
            }
            else if (type == MediaType.Youtube)
            {
                YouTubeApiResult data = new JavaScriptSerializer().Deserialize<YouTubeApiResult>(model.Content);
                if (data.Description.Length > 1499) { data.Description = data.Description.Substring(0, 1499); }
                mediaSvc.CreateYouTubeMedia(media, id, data);
            }
            else if (type == MediaType.Vimeo)
            {
                VimeoApiResult data = new JavaScriptSerializer().Deserialize<VimeoApiResult>(model.Content);
                if (data.Description.Length > 1499) { data.Description = data.Description.Substring(0, 1499); }
                mediaSvc.CreateVimeoMedia(media, id, data);
            }
            else
            {
                throw new NotImplementedException("JSK Cmon you have implemented adding media type : " + type.ToString());
            }

            return media;
        }

        [CfAuthorize]
        public ActionResult Edit(Guid id)
        {
            var media = mediaSvc.GetMediaByID(id);
            ViewBag.Media = media;

            if (media.AddedByUserID != CfIdentity.UserID)
            {
                throw new AccessViolationException("You cannot edit media that does not belong to you");
            }

            var returnUrl = string.Empty;
            if (HttpContext.Request.UrlReferrer != null) { returnUrl = HttpContext.Request.UrlReferrer.ToString(); }

            var model = new EditViewModel() { ID = media.ID, ReturnUrl = returnUrl, Description = media.Description, Title = media.Title };

            return View(model);
        }

        [HttpPost, CfAuthorize]
        public ActionResult Update(EditViewModel m)
        {
            var media = mediaSvc.GetMediaByID(m.ID);

            if (ModelState.IsValid)
            {
                media.Title = m.Title;
                media.Description = m.Description;
                mediaSvc.UpdateMedia(media);

                if (string.IsNullOrWhiteSpace(m.ReturnUrl)) { return RedirectToAction("UsersSubmittedMedia", new { id = CfIdentity.UserID }); }
                else { return Redirect(m.ReturnUrl); }
            }
            else
            {
                return View("Edit",m);
            }
        }

        [HttpPost, CfAuthorize]
        public ActionResult AddMediaTag(Guid id, Guid onObjectID)
        {
            var media = mediaSvc.GetMediaByID(id);
            var obj = AppLookups.GetCacheIndexEntry(onObjectID);

            var alreadyTagged = media.ObjectMedias.Where( om=>om.OnOjectID == onObjectID).Count() > 0;

            if (media != null && obj != null && !alreadyTagged)
            {
                mediaSvc.AddMediaTag(media, onObjectID);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false });
            }
        }


        [HttpPost, CfAuthorize]
        public ActionResult RemoveMediaTag(Guid id, Guid onObjectID)
        {
            var media = mediaSvc.GetMediaByID(id);
            var alreadyTagged = media.ObjectMedias.Where( om=>om.OnOjectID == onObjectID ).Count() > 0;

            if (alreadyTagged)
            {
                mediaSvc.RemoveMediaTag(media, onObjectID);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false });
            }
        }
        


        [HttpPost, CfAuthorize]
        public ActionResult OpinionNew(MediaRatingNewViewModel m)
        {
            if (ModelState.IsValid)
            {
                var media = new MediaOpinion() { MediaID = m.MediaID, Rating = m.Rating, Comment = m.Comment };

                MediaOpinion newRating = mediaSvc.CreateMediaOpinion(media);

                return PartialView("MediaRatingComment", newRating);
            }
            else
            {
                return View(m);
            }
        }

        [CfAuthorize]
        public ActionResult OpinionDelete(Guid id)
        {
            var opinion = mediaSvc.GetMediaOpinion(id);
            mediaSvc.DeleteMediaOpinion(opinion);

            var returnUrl = HttpContext.Request.UrlReferrer.ToString();

            return Redirect(returnUrl);
        }

        [HttpGet, CfAuthorize]
        public ActionResult Delete(Guid id, string returnUrl)
        {
            var media = mediaSvc.GetMediaByID(id);
            mediaSvc.DeleteMedia(media);
            
            return Redirect(returnUrl);
        }
    }
}
