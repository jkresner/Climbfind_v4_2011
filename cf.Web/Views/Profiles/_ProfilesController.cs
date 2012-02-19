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
using NetFrameworkExtensions.Web.Mvc;
using System.IO;
using NetFrameworkExtensions.Net;
using cf.Content.Images;
using cf.Web.Views.Profiles;
using System.Web.Script.Serialization;
using cf.Dtos;
using Omu.ValueInjecter;


namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class ProfilesController : BaseCfController
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        UserService usrSvc { get { if (_usrSvc == null) { _usrSvc = new UserService(); } return _usrSvc; } } UserService _usrSvc;
        VisitsService chkInSvc { get { if (_chkInSvc == null) { _chkInSvc = new VisitsService(); } return _chkInSvc; } } VisitsService _chkInSvc;
        MediaService medSvc { get { if (_medSvc == null) { _medSvc = new MediaService(); } return _medSvc; } } MediaService _medSvc;

        public ActionResult ProfileNotFound()
        {
            return new HttpStatusCodeWithBodyResult("ProfileNotFound", 404);
        }
        
        public ActionResult Me() 
        {
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            if (user.HasAvatar)
            {
                PrepareProfilePageViewData(user);
                return View();
            }
            else
            {
                return RedirectToAction("ChooseAvatar");
            }
        }

        public ActionResult Edit()
        {
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            ViewBag.Current = user;
            var model = new EditProfileViewModel();
            model.InjectFrom(user);
            //if (user.PlaceHome.HasValue) { model.PlaceHomeName = CfCacheIndex.Get(user.PlaceHome.Value).Name; }
            //if (user.PlaceFavorite1.HasValue) { model.PlaceFavorite1Name = CfCacheIndex.Get(user.PlaceFavorite1.Value).Name; }
            //if (user.PlaceFavorite2.HasValue) { model.PlaceFavorite2Name = CfCacheIndex.Get(user.PlaceFavorite2.Value).Name; }
            //if (user.PlaceFavorite3.HasValue) { model.PlaceFavorite3Name = CfCacheIndex.Get(user.PlaceFavorite3.Value).Name; }
            //if (user.PlaceFavorite4.HasValue) { model.PlaceFavorite4Name = CfCacheIndex.Get(user.PlaceFavorite4.Value).Name; }
            //if (user.PlaceFavorite5.HasValue) { model.PlaceFavorite5Name = CfCacheIndex.Get(user.PlaceFavorite5.Value).Name; }

            if (user.CountryID == 83) { ViewBag.NationatliySelectList = cf.Web.Mvc.ViewData.SelectLists.CountryWithUKSelectList; }
            else { ViewBag.NationatliySelectList = cf.Web.Mvc.ViewData.SelectLists.CountrySelectList; }

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(EditProfileViewModel m)
        {
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            
            if (m.DisplayNameTypeID == 2 && string.IsNullOrWhiteSpace(m.NickName)) { 
                ModelState["NickName"].Errors.Add(new ModelError("Cannot choose nick name as display name and leave it blank"));
            }
            else if (m.DisplayNameTypeID == 1 && string.IsNullOrWhiteSpace(m.UserName)) {
                ModelState["UserName"].Errors.Add(new ModelError("Cannot choose username as display name and leave it blank"));
            }
            else if (m.UserName != user.UserName && !string.IsNullOrWhiteSpace(m.UserName)) //-- When making username blank stop multiple users coming back in SingleOrDefault();
            {
                var otherUserWithUserName = usrSvc.GetProfilesAll().Where(p => p.UserName == m.UserName).SingleOrDefault();
                if (otherUserWithUserName != null)
                {
                    ModelState["UserName"].Errors.Add(new ModelError("The username you have chosen is already taken"));
                }
            }
            else if (m.SlugUrlPart != user.SlugUrlPart) //-- kind of crappy there is no Unique index on this, because the value can be null...
            {
                var otherUserWithSameSlug = usrSvc.GetProfilesAll().Where(p => p.SlugUrlPart == m.SlugUrlPart).SingleOrDefault();
                if (otherUserWithSameSlug != null)
                {
                    ModelState["SlugUrlPart"].Errors.Add(new ModelError("The url you have chosen is already taken"));
                }
            }


            if (ModelState.IsValid)
            {
                user.InjectFrom(m);
                usrSvc.UpdateProfile(user);
                return RedirectToAction("Me");
            }
            else
            {
                return View("Edit", m);
            }
        }

        public ActionResult EditPlacePreferences()
        {
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            ViewBag.Current = user;
            var model = new EditPlacePreferencesViewModel();
            if (user.PlaceHome.HasValue) { model.PlaceHomeName = CfCacheIndex.Get(user.PlaceHome.Value).Name; model.PlaceHome = user.PlaceHome; }
            if (user.PlaceFavorite1.HasValue) { model.PlaceFavorite1Name = CfCacheIndex.Get(user.PlaceFavorite1.Value).Name; model.PlaceFavorite1 = user.PlaceFavorite1; }
            if (user.PlaceFavorite2.HasValue) { model.PlaceFavorite2Name = CfCacheIndex.Get(user.PlaceFavorite2.Value).Name; model.PlaceFavorite2 = user.PlaceFavorite2; }
            if (user.PlaceFavorite3.HasValue) { model.PlaceFavorite3Name = CfCacheIndex.Get(user.PlaceFavorite3.Value).Name; model.PlaceFavorite3 = user.PlaceFavorite3; }
            if (user.PlaceFavorite4.HasValue) { model.PlaceFavorite4Name = CfCacheIndex.Get(user.PlaceFavorite4.Value).Name; model.PlaceFavorite4 = user.PlaceFavorite4; }
            //if (user.PlaceFavorite5.HasValue) { model.PlaceFavorite5Name = CfCacheIndex.Get(user.PlaceFavorite5.Value).Name; model.PlaceFavorite5 = user.PlaceFavorite5; }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePlacePreferences(EditPlacePreferencesViewModel m)
        {
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            
            if (ModelState.IsValid)
            {
                user.InjectFrom(m);
                usrSvc.UpdateProfile(user);
                return Redirect("/");
            }
            else
            {
                return View("EditPlacePreferences", m);
            }
        }

        [HttpGet]
        public ActionResult Delete() {
            Microsoft.IdentityModel.Web.FederatedAuthentication.WSFederationAuthenticationModule.SignOut(false);
            string issuer = Microsoft.IdentityModel.Web.FederatedAuthentication.WSFederationAuthenticationModule.Issuer.Replace("/issue/wsfed", "/delete");
            return Redirect(issuer); 
        }


        public ActionResult Detail(string id) 
        {
            Guid userID = default(Guid);
            var user = default(Profile);
            if (Guid.TryParse(id, out userID)) { user = usrSvc.GetProfileByID(userID); }
            else { user = usrSvc.GetProfileBySlugUrlPart(id); }
            if (user == default(Profile)) { return ProfileNotFound(); }

            PrepareProfilePageViewData(user);

            if (user.ID == CfIdentity.UserID) { return View("Me"); }
            
            return View(); 
        }

        private void PrepareProfilePageViewData(Profile user)
        {
            ViewBag.Current = user;
            ViewBag.ModProfile = geoSvc.GetModProfile(user.ID);
            ViewBag.LatestCheckIns = chkInSvc.GetUsersVisits(user.ID).Take(15);
            ViewBag.LatestPartnerCalls = new PartnerCallService().GetUsersLatestPartnerCalls(user.ID, 5);
            ViewBag.PersonalityMedia = usrSvc.GetPersonalityMediaCollectionOfUser(user.ID);
            ViewBag.IndoorClimbedAt = new VisitsService().GetUsersIndoorPlaces(user.ID).ToList();
            ViewBag.LatestOpinions = new ContentService().GetUsersLatestOpinions(user.ID, 5).ToList();
            var media = medSvc.GetUsersMostRecentMedia(user.ID, 11).ToList();

            if (!user.PrivacyShowFeed) { ViewBag.Posts = new List<PostRendered>(); }
            else {
                var feedPosts = new PostService().GetPostForUser(user.ID, ClientAppType.CfWeb, 20).ToList();
                ViewBag.Posts = feedPosts;
            }

            ViewBag.MediaList = media;
        }


        public ActionResult DetailFromSlug(string namePartUrl) { return View(); }

        public ActionResult ChooseAvatar() {
            ViewBag.Current = usrSvc.GetProfileByID(CfIdentity.UserID);
            ViewBag.BoxMaxWidth = 720;
            ViewBag.BoxMaxHeight = 720;
            ViewBag.MinWidth = 240;
            ViewBag.MinHeight = 180;

            return View("ChooseAvatar"); 
        }

        [HttpPost]
        public ActionResult SavePic(string originalImgUrl, int x, int y, int w, int h)
        {
            CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(originalImgUrl,
                stream => usrSvc.SaveProfileAvatarPic(stream, new ImageCropOpts(x, y, w, h)));

            return RedirectToAction("Personality", new { id = CfIdentity.UserID });
        }

        public ActionResult ChooseHeadshot()
        {
            ViewBag.Current = usrSvc.GetProfileByID(CfIdentity.UserID);
            return View();
        }

        [HttpPost]
        public ActionResult SaveHeadshot(string originalImgUrl, int x, int y, int w, int h)
        {
            var mediaName = "Headshot of " + CfIdentity.DisplayName;
            CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(originalImgUrl,
                stream => usrSvc.SavePersonalityMediaImage(CfIdentity.UserID, stream, mediaName, PersonalityCategory.Headshot, new ImageCropOpts(x, y, w, h)));

            return Redirect("/climber/my-profile");
        }


        public ActionResult Personality(Guid id)
        {
            var personalityMedia = usrSvc.GetPersonalityMediaCollectionOfUser(id);

            ViewBag.Personality = personalityMedia;
            ViewBag.Current = usrSvc.GetProfileByID(id);
            return View(); 
        }

        public ActionResult ChoosePersonalityMedia(PersonalityCategory category)
        {
            var model = new PersonalityMediaNewViewModel() { Category = category, 
                Title = string.Format("{0} {1}", CfIdentity.DisplayName, category) };

            if (category == PersonalityCategory.Avatar) { return RedirectToAction("ChooseAvatar"); }
            else if (category == PersonalityCategory.Headshot) { return RedirectToAction("ChooseHeadshot"); }
            else if (category == PersonalityCategory.Daredevil) { return View(model); }
            else if (category == PersonalityCategory.Funny) { return View(model); }
            else if (category == PersonalityCategory.Ready2Rock) { return View(model); }
            else if (category == PersonalityCategory.Scenic) { return View(model); }
            else if (category == PersonalityCategory.BestShot) { return View(model); }
            else if (category == PersonalityCategory.PartnerShot) { return View(model); }
            else
            {
                throw new ArgumentNullException(category + " not a valid personality media category");
            }
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult SavePersonalityMedia(PersonalityMediaNewViewModel model)
        {
            var type = model.Type;

            if (type == MediaType.Image)
            {
                CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(model.Content,
                    stream => usrSvc.SavePersonalityMediaImage(CfIdentity.UserID, stream, model.Title, model.Category, null));
            }
            else if (type == MediaType.Youtube)
            {
                YouTubeApiResult data = new JavaScriptSerializer().Deserialize<YouTubeApiResult>(model.Content);
                usrSvc.SavePersonalityMediaYouTube(model.Title, model.Category, data);
            }
            else if (type == MediaType.Vimeo)
            {
                VimeoApiResult data = new JavaScriptSerializer().Deserialize<VimeoApiResult>(model.Content);
                usrSvc.SavePersonalityMediaVimeo(model.Title, model.Category, data);
            }
            else
            {
                throw new NotImplementedException(string.Format("{0} media type not supported yet", type));
            }

            return Redirect("/climber/personality/"+CfIdentity.UserID.ToString());
        }


        public ActionResult PersonalityMediaDetail(Guid id)
        {
            var media = usrSvc.GetPersonalityMediaByID(id);
            var personalityMedia = usrSvc.GetPersonalityMediaCollectionOfUser(media.UserID);
            var owner = usrSvc.GetProfileByID(personalityMedia.UserID);

            ViewBag.Media = media;
            ViewBag.Personality = personalityMedia;
            ViewBag.MediaOwner = owner;

            return View();
        }


        [HttpPost, CfAuthorize]
        public ActionResult OpinionNewAjax(Guid id, NewOpinionWithCommentsListViewModel m)
        {
            if (ModelState.IsValid)
            {
                var media = new MediaOpinion() { MediaID = id, Rating = m.Rating, Comment = m.Comment };

                var opinion = medSvc.CreateMediaOpinion(media);

                return PartialView("OpinionDetail", opinion);
            }
            else
            {
                return Json(new { Success = false });
            }
        }
    }
}
