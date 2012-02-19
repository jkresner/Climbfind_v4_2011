using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Services;
using cf.Caching;
using cf.Entities;
using cf.Identity;
using cf.Web.Views.Shared;
using System.Globalization;
using cf.Web.Views.CheckIns;
using NetFrameworkExtensions;
using System.Text.RegularExpressions;

namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class VisitsController : BaseCfController
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        VisitsService visitsSvc { get { if (_visitsSvc == null) { _visitsSvc = new VisitsService(); } return _visitsSvc; } } VisitsService _visitsSvc;

        public ActionResult ListMy()
        {
            return ListUser(CfIdentity.UserID);
        }

        /// <summary>
        /// A.k.a climbing history
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ListUser(Guid id)
        {
            var user = new UserService().GetProfileByID(id);
            ViewBag.UserProfile = user;
            ViewBag.IsPublic = user.PrivacyShowHistory;
            
            if (user.PrivacyShowHistory || CfIdentity.UserID == id)
            {
                ViewBag.Visits = visitsSvc.GetUsersHistory(id);
            }
            
            //Add comparison data
            if (id != CfIdentity.UserID)
            {
                //-- TODO Here
                //ViewBag.Comparisions;
            }

            return View("ListUser");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ListLocation(Guid id)
        {
            ViewBag.Location = AppLookups.GetCacheIndexEntry(id);
            ViewBag.CheckIns = visitsSvc.GetLocationsLatestCheckIns(id, 30);
            return View();
        }

        [HttpPost]
        public ActionResult CreateHistorical(NewHistoricalCheckInViewModel model)
        {
            var location = geoSvc.GetLocationByID(model.CheckLocationID);
            if (location == default(Location)) { RedirectToAction("PlaceNotFound", "Places"); }

            var checkInDateTime = DateTimeExtensions.ParseDateAndTime(model.CheckInDate,model.CheckInTime);

            //-- Here we're saying if they've checked in today or 'tomorrow',
            //-- just take the current data/time so the feed doesn't look strange
            if (checkInDateTime.Date == DateTime.UtcNow.Date) { checkInDateTime = DateTime.UtcNow; }
            else if (checkInDateTime > DateTime.UtcNow) { checkInDateTime = DateTime.UtcNow; } 
            
            if (String.IsNullOrWhiteSpace(model.CheckInComment)) { model.CheckInComment = string.Format("I'm at {0}", location.Name); }

            var ci = visitsSvc.CreateCheckIn(new CheckIn()
            {
                UserID = CfIdentity.UserID,
                Comment = model.CheckInComment,
                LocationID = location.ID,
                Utc = checkInDateTime
            });

            return RedirectToAction("My", new { id = ci.ID });
        }

        [HttpGet]
        public ActionResult AddMedia(Guid id)
        {
            var checkIn = visitsSvc.GetCheckInById(id);
            ViewBag.CheckIn = checkIn;
            var visitSlug = "/visit/" + id.ToString();
            var model = new cf.Web.Views.Media.AddViewModel() { ObjectId = checkIn.LocationID, ObjectSlug = visitSlug, ObjectName = "My visit", Title = "Visit media " };
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult AddMedia(Guid id, cf.Web.Views.Media.AddViewModel m)
        {
            if (ModelState.IsValid)
            {
                var ci = visitsSvc.GetCheckInById(id);
                var media = new MediaController().SaveMediaWithTag(m);
                visitsSvc.AddMedia(ci, media);

                return Redirect("/Visits/My?id=" + id.ToString());
            }
            else
            {
                return View(m);
            }
        }

        [HttpPost]
        public JsonResult UpdateComment(UpdateCommentViewModel m)
        {
            try
            {
                var ci = visitsSvc.GetCheckInById(m.ID);
                ci.Comment = m.Comment;

                visitsSvc.UpdateCheckIn(ci);

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogClimb(Guid id, ClimbLogViewModel m)
        {
            var checkIn = visitsSvc.GetCheckInById(id);

            if (checkIn != null && ModelState.IsValid)
            {
                var logged = visitsSvc.LogClimb(checkIn, new LoggedClimb()
                {
                    ClimbID = m.ClimbID,
                    Comment = m.Comment,
                    Experince = (byte)m.Experience,
                    Outcome = (byte)m.Outcome,
                    GradeOpinion = (byte)m.Opinion,
                    Rating = m.Rating
                });

                return PartialView("Partials/LoggedClimbDetail", logged);
                //return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        /// <summary>
        /// Perhaps they click through here when their on the place page and then want to check in?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //public ActionResult CreateCurrent(Guid id)
        //{
        //    var location = geoSvc.GetLocationByID(id);
        //    if (location == default(Location)) { RedirectToAction("PlaceNotFound", "Places"); }

        //    ViewBag.Location = location;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateCurrent(Guid id, string comment)
        //{
        //    var location = geoSvc.GetLocationByID(id);
        //    if (location == default(Location)) { RedirectToAction("PlaceNotFound", "Places"); }

        //    checkInSvc.CreateCheckIn(new CheckIn()
        //    {
        //        UserID = CfIdentity.UserID,
        //        Comment = comment,
        //        LocationID = id
        //    });

        //    return RedirectToAction("ListLocation", new { id = id });
        //}

        public ActionResult Detail(Guid id)
        {
            var ci = visitsSvc.GetCheckInById(id);
            ViewBag.CheckIn = ci;
            ViewBag.Profile = new UserService().GetProfileByID(ci.UserID);
            ViewBag.Post = new PostService().GetPostByID(id);
            var closestCI = visitsSvc.GetLocationsClosestCheckIns(ci.LocationID, ci.Utc).Where(c=>c.ID!=ci.ID).ToList();
            ViewBag.ClosestCheckIns = closestCI;

            return View(); 
        }


        public ActionResult My(Guid id)
        {
            var ci = visitsSvc.GetCheckInById(id);
            ViewBag.CheckIn = ci;
            if (ci.UserID != CfIdentity.UserID) { return RedirectToAction("Detail", new { id = id }); }

            var recentCI = visitsSvc.GetLocationsLatestCheckIns(ci.LocationID, 10).ToList();
            var closestCI = visitsSvc.GetLocationsClosestCheckIns(ci.LocationID, ci.Utc).Where(c => c.ID != ci.ID).ToList();
            ViewBag.RecentCheckIns = recentCI;
            ViewBag.ClosestCheckIns = closestCI;

            ViewBag.Loggedclimbs = visitsSvc.GetCheckInsLoggedClimbs(ci.ID).ToList();

            ViewBag.NotGivenOpinion = (null == new ContentService().GetUserOpinionOnPlace(CfIdentity.UserID, ci.LocationID));

            return View();
        }

        public ActionResult DeleteVisitMedia(Guid id, Guid mediaID, string returnUrl)
        {
            var ci = visitsSvc.GetCheckInById(id);
            var media = new MediaService().GetMediaByID(mediaID);
            visitsSvc.RemoveMedia(ci, media);

            return Redirect(returnUrl); 
        }

        public ActionResult Delete(Guid id)
        {
            var checkIn = visitsSvc.GetCheckInById(id);
            visitsSvc.DeleteCheckIn(checkIn);
            return RedirectToAction("ListUser", new { id = CfIdentity.UserID });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetAvailableClimbsForLogging(Guid id, string term)
        {
            var checkIn = new VisitsService().GetCheckInById(id);

            term = SanitizeClimbTerm(term);

            //-- because we cache on the client - no need to do here
            //var climbsAvailable = PerfCache.GetClimbsForCheckIn(checkIn.LocationID, checkIn.Utc);
            var climbsAvailable = geoSvc.GetClimbsOfLocationForLogging(checkIn.LocationID, checkIn.Utc);

            var matchClimbs = (from c in climbsAvailable
                               where
                                   Regex.Match(c.GradeLocal ?? "Undefined", term, RegexOptions.IgnoreCase).Success ||
                                   Regex.Match(c.Name, term, RegexOptions.IgnoreCase).Success ||
                                   Regex.Match(SanitizeClimbTerm(c.Name), term, RegexOptions.IgnoreCase).Success
                               select new AvailableClimb { ID = c.ID, Name = c.Name, Thumb = c.AvatarRelativeUrl, Grade = c.GradeLocal }).ToList();

            if (matchClimbs.Count == 0)
            {
                matchClimbs.Add(new AvailableClimb() { ID = Guid.Empty, Name = "No climbs found - add one now!", Thumb = string.Empty, Grade = string.Empty });
            }

            return Json(matchClimbs);
        }

        private string SanitizeClimbTerm(string original)
        {
            string sanitized = original.Replace("<", "").Replace(">", "").Replace("'", "").Replace(@"\", "").Replace(@"/", "").Replace(":", "")
                .Replace(@"(", "").Replace(@")", "").Replace(@"[", "").Replace(@"]", "");

            //-- protect from injection attacks
            if (sanitized.Length > 30) { sanitized = sanitized.Substring(0, 30); }

            return sanitized;
        }

        class AvailableClimb
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Thumb { get; set; }
            public string Grade { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loggedClimbID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteLoggedClimb(Guid id, Guid loggedClimbID)
        {
            var checkIn = new VisitsService().GetCheckInById(id);

            if (checkIn != null && ModelState.IsValid)
            {
                new VisitsService().DeleteLoggedClimb(checkIn, loggedClimbID);

                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        public ActionResult UsersLoggedClimbs(Guid id)
        {
            var user = CfPerfCache.GetClimber(id);
            ViewBag.User = user;
            ViewBag.LoggedClimbs = visitsSvc.GetUsersLoggedClimbs(user.ID).ToList();

            return View();
        }

    }
}
