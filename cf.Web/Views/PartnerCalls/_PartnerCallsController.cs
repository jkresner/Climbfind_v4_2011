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

namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class PartnerCallsController : Controller
    {
        UserService usrSvc { get { if (_usrSvc == null) { _usrSvc = new UserService(); } return _usrSvc; } } UserService _usrSvc;
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        PartnerCallService pcSvc { get { if (_pcSvc == null) { _pcSvc = new PartnerCallService(); } return _pcSvc; } } PartnerCallService _pcSvc;
        MappingService mappingSvc { get { if (_mappingSvc == null) { _mappingSvc = new MappingService(); } return _mappingSvc; } } MappingService _mappingSvc;

        public ActionResult Index() 
        {
            var user =  usrSvc.GetProfileByID(CfIdentity.UserID);
            ViewBag.User = user;

            CfCacheIndexEntry place = null;
            if (user.PlaceFavorite5.HasValue) { place = CfCacheIndex.Get(user.PlaceFavorite5.Value); }
            ViewBag.Place = place;

            var deducPlaces = new List<CfCacheIndexEntry>();
            if (place != null)
            {
                ViewBag.PartnerCalls = pcSvc.GetPlacesGeoDeducLatestPartnerCalls(place.ID, 25).ToList();

                if (place.Type.ToPlaceCateogry() != PlaceCategory.Area)
                {
                    deducPlaces = CfPerfCache.GetGeoDeduciblePlaces(place);
                }
            }
            ViewBag.DeducPlaces = deducPlaces;


            return View(); 
        }

        public ActionResult Detail(Guid id)
        {
            var pc = pcSvc.GetPartnerCallById(id);
            if (pc != null)
            {
                ViewBag.PartnerCall = pc;

                var place = AppLookups.GetCacheIndexEntry(pc.PlaceID);
                var model = PrepareNewCallViewData(place);

                ViewBag.Post = new PostService().GetPostByID(id);
                ViewBag.PlaceEntry = place;

                return View(model);
            }
            else
            {
                var pcwi = pcSvc.GetPartnerCallWorkItemByPartnerCallId(id);
                if (pcwi == null) { return View("PageNotFound"); }
                else { return RedirectToRoute("UserProfile", new { id = pcwi.OnBehalfOfUserID }); }
            }
        }

        
        public ActionResult PrivateReply(Guid id)
        {
            var pc = pcSvc.GetPartnerCallById(id);

            return PartialView("Forms/MessageForm",  new NewMessageViewModel(pc.ID) { Controller = "PartnerCalls", Action ="ReplySend"});
        }

        [CfAuthorize]
        public ActionResult ReplySend(NewMessageViewModel m)
        {
            var pc = pcSvc.GetPartnerCallById(m.ForID);

            if (ModelState.IsValid && pc != null)
            {
                new ConversationService().PrivatePartnerCallReply(pc, m.Content);
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        public ActionResult ListUser(Guid id)
        {
            var usersPartnerCalls = pcSvc.GetUsersPartnerCalls(id).OrderByDescending(o => o.CreatedUtc).ToList();
            ViewBag.PartnerCalls = usersPartnerCalls;
            ViewBag.User = new UserService().GetProfileByID(id);

            var distinctPlaceIDs = (from c in usersPartnerCalls select c.PlaceID).Distinct(); ;
            ViewBag.Places = (from c in distinctPlaceIDs
                             where AppLookups.GetCacheIndexEntry(c) != null
                             select AppLookups.GetCacheIndexEntry(c)).Take(10).ToList();

            return View();
        }


        public ActionResult ListPlace(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);
            if (place == null) { return new RedirectToRouteResult("PlaceNotFound", null); }
            ViewBag.Place = place;
            
            var placesPartnerCalls = pcSvc.GetPlacesLatestPartnerCalls(id, 20).ToList();
            ViewBag.PartnerCalls = placesPartnerCalls;

            //var placesTodayPartnerCalls = pcSvc.GetPlacesGeoDeducTodayPartnerCalls(id).ToList();
            //ViewBag.TodayPartnerCalls = placesTodayPartnerCalls;
            
            return View();
        }


        public ActionResult New(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);
            
            //-- Return PlaceNotFound
            //if (place == null) { return  }
  
            var model = PrepareNewCallViewData(place);
            
            return View("New", model);
        }

        private NewPartnerCallViewModel PrepareNewCallViewData(CfCacheIndexEntry place)
        {
            var m = new NewPartnerCallViewModel() { ParnterCallPlaceID = place.ID, ForIndoor = true, ForOutdoor = true };

            if (place.Type == CfType.Country)
            {
                ViewBag.PlaceDisallowed = true;
                ViewBag.Place = place;
            }
            else if (place.Type == CfType.Province)
            {
                var area = geoSvc.GetAreaByID(place.ID);

                ViewBag.PlaceDisallowed = true;
                ViewBag.Place = place;

                ViewBag.InterectingAreas = geoSvc.GetIntersectingAreas(area).Where(a => a.Type != CfType.Province
                    && !a.DisallowPartnerCalls).ToList();
            }
            else if (place.Type.ToPlaceCateogry() == PlaceCategory.Area)
            {
                ViewBag.PlaceType = "Area";
                var area = geoSvc.GetAreaByID(place.ID);
                ViewBag.Place = area;

                if (area.DisallowPartnerCalls) { ViewBag.PlaceDisallowed = true; }
                else
                {
                    var geoJsonUrl = Stgs.MapSvcRelativeUrl + "area/" + place.ID.ToString();

                    var mapModel = new Bing7GeoJsonMapViewModel("climbing-map-" + place.ID, 680, 400, geoJsonUrl);
                    mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mappingSvc.GetBingViewByID(area.ID));
                    var mapItems = new MappingService().GetAreaEditMapItems(area);
                    ViewBag.MapItemsArea = mapItems.Items[0];
                    mapItems.Items.RemoveAt(0);
                    ViewBag.MapItemsLocations = mapItems.Items;
                    ViewBag.MapModel = mapModel;
                }

                ViewBag.ChildLocations = geoSvc.GetGeoDeduciblePlaces(place).Where(p=>p.Type.IsLocation());

                ViewBag.InterectingAreas = geoSvc.GetIntersectingAreas(area).Where(a => a.Type != CfType.Province
                    && !a.DisallowPartnerCalls).ToList();
            }
            else if (place.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing
                || place.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing)
            {
                ViewBag.PlaceType = "Location";
                var location = geoSvc.GetLocationByID(place.ID);
                ViewBag.Place = location;

                if (location.IsIndoorClimbing) { m.ForOutdoor = false; }
                if (location.IsOutdoorClimbing) { m.ForIndoor = false; }

                var mapModel = new Bing7MapWithLocationViewModel(location.NameUrlPart, 732, 340, location.Latitude,
                    location.Longitude, location.AvatarRelativeUrl);

                var mapViewSettings = mappingSvc.GetBingViewByID(location.ID);
                if (mapViewSettings == default(PlaceBingMapView)) { mapViewSettings = PlaceBingMapView.GetDefaultIndoorSettings(location); }
                mapModel.ViewOptions = new Bing7MapViewOptionsViewModel(mapViewSettings);
                ViewBag.LocationMapView = mapModel;

                ViewBag.InterectingAreas = geoSvc.GetIntersectingAreasOfPoint(location.Latitude, location.Longitude)
                    .Where(a => a.Type != CfType.Province && !a.DisallowPartnerCalls).ToList();
            }
            else
            {
                throw new ArgumentException("Place type [" + place.Type.ToString() + "] not supported for partner calls");
            }

            return m;
        }

        [HttpPost]
        public ActionResult Create(NewPartnerCallViewModel m)
        {
            if (!m.ForOutdoor && !m.ForIndoor) { ModelState.AddModelError("For", "Your partner call must be for indoor, outdoor or both."); }
            
            if (ModelState.IsValid)
            {
                DateTime endDateTime;
                
                var startDateTime = DateTimeExtensions.ParseDateAndTime(m.StartDate, m.StartTime);
                DateTimeExtensions.TryParseDateAndTime(m.EndDate, m.EndTime, out endDateTime);

                var pc = pcSvc.CreatePartnerCall( new PartnerCall() { Comment = m.Comment, StartDateTime = startDateTime,
                    EndDateTime = endDateTime, ForIndoor = m.ForIndoor, ForOutdoor = m.ForOutdoor, PlaceID = m.ParnterCallPlaceID, 
                    PreferredLevel = (byte)m.PreferredLevel }
                );

                if (!pcSvc.UserHasSubscriptionForRelatedGeo(m.ParnterCallPlaceID))
                {
                    var pcs = pcSvc.CreatePartnerCallSubscription(new PartnerCallSubscription()
                    {
                        ForIndoor = true,
                        ForOutdoor = true,
                        PlaceID = m.ParnterCallPlaceID,
                        EmailRealTime = true,
                        MobileRealTime = false,
                        ExactMatchOnly = false
                    });
                    //return RedirectToAction("SubscriptionSuggestion", new { id = m.ParnterCallPlaceID });
                }

                return RedirectToAction("ListPlace", new { id = m.ParnterCallPlaceID });
            }
            else
            {
                PrepareNewCallViewData(AppLookups.GetCacheIndexEntry(m.ParnterCallPlaceID));
                return View("New");
            }
        }

        public ActionResult Details(Guid id)
        {
            ViewBag.PartnerCalls = pcSvc.GetPartnerCallById(id);
            return View(); 
        }

        public ActionResult UpdatePartnerCallFeedPlace(Guid id)
        {
            var place = CfCacheIndex.Get(id);
            if (place == null) { throw new ArgumentNullException(string.Format("No place exists for ID {0}", id)); }
            else if (place.TypeID == 2) { throw new ArgumentNullException(string.Format("Cannot set partner feed for Province {0}", id)); }
            else if (place.TypeID > 60 || place.TypeID < 3) { throw new ArgumentNullException(string.Format("Cannot set partner feed for CFtype {0}[1]", id, place.TypeID)); }
            
            var user = usrSvc.GetProfileByID(CfIdentity.UserID);
            user.PlaceFavorite5 = place.ID;
            usrSvc.UpdateProfile(user);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var pc = pcSvc.GetPartnerCallById(id);
            pcSvc.DeletePartnerCall(pc);
            return RedirectToAction("ListUser", new { id = CfIdentity.UserID });
        }

        public ActionResult Subscriptions()
        {
            var subscriptions = pcSvc.GetUsersPartnerCallSubscriptions(CfIdentity.UserID).ToList();
            ViewBag.Subscriptions = subscriptions;
            return View();
        }

        [HttpGet]
        public ActionResult SubscriptionSuggestion(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);
            ViewBag.Place = place;
            return View();
        }     

        [HttpGet]
        public ActionResult SubscriptionNew(Guid id)
        {
            var place = AppLookups.GetCacheIndexEntry(id);

            PrepareNewCallViewData(place);

            var model = new NewPartnerCallSubscriptionViewModel()
            {
                ParnterCallPlaceID = place.ID,
                ForIndoor = true,
                ForOutdoor = true,
                EmailRealtime = true,
                MobileRealtime = true,
                ExactOnly = false
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SubscriptionNew(Guid id, NewPartnerCallSubscriptionViewModel m)
        {
            var place = AppLookups.GetCacheIndexEntry(id);

            if (ModelState.IsValid && place != null)
            {
                var pc = pcSvc.CreatePartnerCallSubscription(new PartnerCallSubscription()
                {
                    ForIndoor = m.ForIndoor,
                    ForOutdoor = m.ForOutdoor,
                    PlaceID = m.ParnterCallPlaceID,
                    EmailRealTime = m.EmailRealtime,
                    MobileRealTime = m.MobileRealtime,
                    ExactMatchOnly = m.ExactOnly
                });

                return RedirectToAction("Subscriptions");
            }
            else
            {
                PrepareNewCallViewData(AppLookups.GetCacheIndexEntry(m.ParnterCallPlaceID));
                return View("SubscriptionNew", m);
            }
        }


        public ActionResult SubscriptionDelete(Guid id)
        {
            var pcs = pcSvc.GetPartnerCallSubscriptionById(id);
            pcSvc.DeletePartnerCallSubscription(pcs);
            return RedirectToAction("Subscriptions");
        }
    }
}
