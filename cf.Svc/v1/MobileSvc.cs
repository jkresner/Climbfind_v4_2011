using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using cf.Content.Search;
using cf.Identity;
using cf.Caching;
using System.Net;
using cf.Services;
using cf.Entities;
using cf.Entities.Enum;
using cf.Dtos;
using cf.Dtos.Mobile.V1;
using Message = System.ServiceModel.Channels.Message;
using Microsoft.IdentityModel.Claims;
using cf.Instrumentation;
using System.Globalization;
using NetFrameworkExtensions;

namespace cf.Svc.v1
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class MobileSvc : AbstractRestService
    {
        [WebGet(UriTemplate = "nearest-locations")]
        public Message GetNearestLocations()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            var places = new MobileService().GetNearestLocationsV1(ctx.Lat, ctx.Lon, 11);
            
            return ReturnAsJson(places);
        }

        [WebGet(UriTemplate = "geo-context")]
        public Message GetGeoContext()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            var places = new MobileService().GetNearestLocationsV1(ctx.Lat, ctx.Lon, 12);
            var areas = new GeoService().GetIntersectingAreasOfPoint(ctx.Lat, ctx.Lon);

            foreach (var a in areas)
            {
                places.Add(new LocationResultDto(a.ID, a.TypeID, a.CountryID, a.Name, a.NameShort, a.Avatar, a.Latitude, a.Longitude,
                    0, a.Rating, a.RatingCount));
            }

            return ReturnAsJson(places);
        }

        [WebGet(UriTemplate = "me")]
        public Message GetMe()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            var p = usrSvc.GetProfileByID(CfIdentity.UserID);

            return ReturnAsJson(new ProfileDto(p));
        }

        [WebGet(UriTemplate = "climber/{id}")]
        public Message GetClimber(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid userID = Guid.ParseExact(id, "N");

            var p = new UserService().GetProfileByID(userID);
            var dto = new ProfileDto(p);

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "climber-by-fid/{id}")]
        public Message GetClimberByFacebookID(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            var p = new UserService().GetProfileByFacebookID(long.Parse(id));

            return ReturnAsJson(new ProfileDto(p));
        }

        [WebGet(UriTemplate = "get-area/{id}")]
        public Message GetArea(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid areaID = Guid.ParseExact(id, "N");

            ////-- TODO, check cache
            var area = new GeoService().GetAreaByID(areaID);
            if (area == default(Area) || area.Type == CfType.Province) { return Failed("Service not invoked correctly - area not valid for operation"); }

            var dto = new AreaDetailDto(area);

            var places = CfPerfCache.GetGeoDeduciblePlaces(CfCacheIndex.Get(area.ID));

            var locations = geoSvc.GetLocationsOfArea(area.ID);
            foreach (var l in locations)
            {
                dto.Locations.Add(new LocationResultDto(l.ID, l.TypeID, l.CountryID, l.Name, l.NameShort, l.Avatar, l.Latitude, l.Longitude, 0, l.Rating, l.RatingCount));
            }

            var intersectionAreas = geoSvc.GetIntersectingAreas(area).Where(a=>a.Type == CfType.ClimbingArea);
            foreach (var l in intersectionAreas)
            {
                dto.Locations.Add(new LocationResultDto(l.ID, l.TypeID, l.CountryID, l.Name, l.NameShort, l.Avatar, l.Latitude, l.Longitude, 0, l.Rating, l.RatingCount));
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-area-lite/{id}")]
        public Message GetAreaLite(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid areaID = Guid.ParseExact(id, "N");
            var area = new GeoService().GetAreaByID(areaID);
            var dto = new AreaDetailDto(area);
            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "current-climbs/{id}")]
        public Message GetCurrentClimbsOfLocation(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
           
            try
            {
                Guid locID = Guid.ParseExact(id, "N");
                var loc = AppLookups.GetCacheIndexEntry(locID);

                var dto = GetClimbsOfLocation(loc, DateTime.UtcNow);
                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Could not get climbs for location: " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "checkin-available-climbs/{id}")]
        public Message GetClimbsOfLocationAtCheckIn(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid ciID = Guid.ParseExact(id, "N");

                var ci = new VisitsService().GetCheckInById(ciID);
                if (ci == null) { throw new Exception("Visit does not exist, it may have been deleted."); }

                ////-- TODO, check cache
                var loc = AppLookups.GetCacheIndexEntry(ci.LocationID);

                var dto = GetClimbsOfLocation(loc, ci.Utc);
                return ReturnAsJson(dto); 
            }
            catch (Exception ex)
            {
                return Failed("Could not get climbs for visit: " + ex.Message);
            }
        }

        private ClimbListDto GetClimbsOfLocation(CfCacheIndexEntry loc, DateTime dateTime)
        {
            if (loc == null) { throw new ArgumentNullException("GetClimbsOfLocation:loc"); }

            //-- GetCurrentClimbsOfLocation is smart enough under the hood to distinguish for indoor & outdoor            
            var placeCategory = loc.Type.ToPlaceCateogry();
            if (placeCategory == PlaceCategory.IndoorClimbing ||
                placeCategory == PlaceCategory.OutdoorClimbing)
            {
                if (dateTime > DateTime.UtcNow.AddHours(-24))
                {
                    return CfPerfCache.TryGetFromCache<ClimbListDto>("mobindoorclimblist-" + loc.ID.ToString("N"),
                        () => GetClimbListDto(loc, placeCategory, dateTime), CfPerfCache.SixtyMinCacheItemPolicy);
                }
                else
                {
                    return GetClimbListDto(loc, placeCategory, dateTime);
                }
            }
            else
            {
                throw new ArgumentNullException("Service not invoked correctly - loc not valid place for operation");
            }
        }

        private ClimbListDto GetClimbListDto(CfCacheIndexEntry loc, PlaceCategory placeCategory, DateTime dateTime)
        {
            var dto = new ClimbListDto();
            var climbsOfLoc = geoSvc.GetClimbsOfLocationForLogging(loc.ID, dateTime);
            
            if (placeCategory == PlaceCategory.IndoorClimbing)
            {
                dto.Sections = new List<ClimbSectionDto>();
                foreach (var s in geoSvc.GetLocationSections(loc.ID))
                {
                   var climbsInSectionOrdered = (from c in climbsOfLoc orderby c.GradeCfNormalize where c.SectionID == s.ID select new ClimbListItemDto(c)).ToList();
                    var range = "";
                    if (climbsInSectionOrdered.Count > 0) { range = string.Format("{0} - {1}", climbsInSectionOrdered.First().Grade, climbsInSectionOrdered.Last().Grade); }
                    
                    dto.Sections.Add(new ClimbSectionDto()
                    {
                        ID = s.ID.ToString("N"),
                        Name = s.Name,
                        Type = s.DefaultClimbTypeID.ToString(),
                        Range = range,
                        Avatar = s.Avatar ?? "4d62a66f-62f.jpg",
                        Climbs = climbsInSectionOrdered
                    });
                }
            }

            var uncategorized = from c in climbsOfLoc.Where(c => !c.SectionID.HasValue) select new ClimbListItemDto(c);
            if (uncategorized.Count() > 0)
            {
                dto.Sections.Add(new ClimbSectionDto()
                {
                    ID = Guid.Empty.ToString("N"),
                    Name = "Uncategorized",
                    Climbs = uncategorized.ToList()
                });
            }

            return dto;
        }

        [WebGet(UriTemplate = "climb/{id}")]
        public Message GetClimb(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid climbID = Guid.ParseExact(id, "N");

            ////-- TODO, check cache
            var climb = AppLookups.GetCacheIndexEntry(climbID);
            if (climb.Type == CfType.ClimbIndoor)
            {
                var c = geoSvc.GetIndoorClimbByID(climb.ID);    
                return ReturnAsJson(new ClimbIndoorDetailDto(c));
            } else if (climb.Type == CfType.ClimbOutdoor) {
                var c = geoSvc.GetOutdoorClimbByID(climb.ID);
                return ReturnAsJson(new ClimbOutdoorDetailDto(c));
            }
            else
            {
                return Failed("Service not invoked correctly - ID not valid for operation");
            }
        }

        [WebGet(UriTemplate = "location/{id}")]
        public Message GetLocation(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid locID = Guid.ParseExact(id, "N");

            //-- TODO, check cache
            var loc = AppLookups.GetCacheIndexEntry(locID);
            if (loc.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing)
            {
                var l = geoSvc.GetLocationIndoorByID(loc.ID);
                return ReturnAsJson(new LocationIndoorDetailDto(l));
            }
            else if (loc.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing)
            {
                var l = geoSvc.GetLocationOutdoorByID(loc.ID);
                return ReturnAsJson(new LocationOutdoorDetailDto(l));
            }
            else
            {
                return Failed("Service not invoked correctly - ID not valid for operation");
            }
        }

        [WebGet(UriTemplate = "get-subscriptions")]
        public Message GetPartnerCallSubscriptions()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            var usersSubscriptions = pcSvc.GetUsersPartnerCallSubscriptions(CfIdentity.UserID);
            var dto = new List<PartnerCallSubscriptionDto>();

            foreach (var s in usersSubscriptions)
            {
                //-- Here we use perf cache instead of Include()/join because it's likely the user will have revisited the same place
                var place = CfCacheIndex.Get(s.PlaceID);
                if (place != null)
                {
                    var subDto = new PartnerCallSubscriptionDto(place, s);
                    dto.Add(subDto);
                }
            }

            return ReturnAsJson(dto);
        }


        [WebGet(UriTemplate = "my-partner-calls")]
        public Message GetMyPartnerCalls()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; } 
            
            var calls = pcSvc.GetUsersLatestPartnerCalls(CfIdentity.UserID, 10);
            var dto = new List<PartnerCallDto>();

            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            foreach (var c in calls)
            {
                //-- Here we use perf cache instead of Include()/join because it's likely the user will have revisited the same place
                var place = CfCacheIndex.Get(c.PlaceID);
                if (place != null)
                {
                    var callDto = new PartnerCallDto(place, c, user);
                    dto.Add(callDto);
                }
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-partner-calls/{id}")]
        public Message GetNewestPartnerCallsForPlace(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; } 
            
            Guid pID = Guid.ParseExact(id, "N");
            var calls = pcSvc.GetPlacesGeoDeducLatestPartnerCalls(pID, 15);

            var dto = new List<PartnerCallDto>();

            foreach (var c in calls)
            {
                //-- Here we use perf cache instead of Include()/join because it's likely the user will have revisited the same place
                var place = CfCacheIndex.Get(c.PlaceID);
                if (place != null)
                {
                    var user = CfPerfCache.GetClimber(c.UserID);
                    var callDto = new PartnerCallDto(place, c, user);
                    dto.Add(callDto);
                }
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-today-partner-calls/{id}")]
        public Message GetTodayPartnerCallsForPlace(string id)
        {
            try
            {
                SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

                Guid pID = Guid.ParseExact(id, "N");
                var calls = pcSvc.GetPlacesGeoDeducTodayPartnerCalls(pID);

                var dto = new List<PartnerCallDto>();

                foreach (var c in calls)
                {
                    //-- Here we use perf cache instead of Include()/join because it's likely the user will have revisited the same place
                    var place = CfCacheIndex.Get(c.PlaceID);
                    if (place != null)
                    {
                        var user = CfPerfCache.GetClimber(c.UserID);
                        var callDto = new PartnerCallDto(place, c, user);
                        dto.Add(callDto);
                    }
                }

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                CfTrace.Error(ex);
                return Failed("Partner search failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "get-visit/{id}")]
        public Message GetVisit(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid ciID = Guid.ParseExact(id, "N");
                var ci = visitsSvc.GetCheckInById(ciID);
                var loc = CfPerfCache.GetLocation(ci.LocationID);
                var ciDto = new VisitDto(ci, loc, CfIdentity.DisplayName, "");

                return ReturnAsJson(ciDto);
            }
            catch { return Failed("Query failed"); }
        }

        [WebGet(UriTemplate = "history/{id}")]
        public Message History(string id)
        {
            try
            {
                SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

                Guid uID;
                if (id == "me") { uID = CfIdentity.UserID; }
                else { uID = Guid.ParseExact(id, "N"); }

                //-- TODO: rewrite to use $expand and do single database call
                var usersCheckIns = visitsSvc.GetUsersHistory(uID).OrderByDescending(c => c.Utc).Take(15);
                var dto = new List<VisitDto>();

                foreach (var ci in usersCheckIns)
                {
                    //-- Here we use perf cache instead of Include()/join because it's likely the user will
                    //-- have revisited the same place
                    var loc = CfPerfCache.GetLocation(ci.LocationID);
                    var ciDto = new VisitDto(ci, loc, "", "");
                    dto.Add(ciDto);
                }

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                CfTrace.Error(ex);
                return Failed("History query failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "recent-visits/{id}")]
        public Message GetLocationsRecentVisits(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            Guid locationID = Guid.ParseExact(id, "N");
            var dto = new List<VisitDto>();
            var location = CfPerfCache.GetLocation(locationID);

            foreach (var c in new VisitsService().GetLocationsCheckIns(locationID).OrderByDescending(c => c.Utc).Take(12))
            {
                var p = CfPerfCache.GetClimber(c.UserID);
                dto.Add(new VisitDto(c, location, p.DisplayName, p.Avatar));
            }

            return ReturnAsJson(dto);
        }
        
        [WebGet(UriTemplate = "check-in/{loc}/{isPrivate}/{getAlerts}?comment={comment}")]
        public Message CheckIn(string loc, string isPrivate, string getAlerts, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid locID = Guid.ParseExact(loc, "N");
                bool bIsPrivate = false; if (isPrivate == "y") { bIsPrivate = true; }
                bool bGetAlerts = true; if (isPrivate == "n") { bGetAlerts = false; }
                var location = CfPerfCache.GetLocation(locID);
                //-- TODO: Add GET ALERTS TO DB AND PASS IN HERE

                //var decodedComment = comment.ODataDecode();
                var ci = visitsSvc.CreateCurrentCheckIn(
                    new CheckIn() { LocationID = location.ID, Latitude = ctx.Lat, Longitude = ctx.Lon, Comment = comment, IsPrivate = bIsPrivate });
       
                var dto = new VisitDto(ci, location, CfIdentity.DisplayName, "");

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Check in failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "new-partner-call/{loc}/{start}/{end}/{preferredLevel}?comment={comment}")]
        public Message NewPartnerCall(string loc, string start, string end, string preferredLevel, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid locID = Guid.ParseExact(loc, "N");
                var place = CfCacheIndex.Get(locID);
                var startDateTime = DateTime.ParseExact(start, "dddMMMddyyyyhhmmtt", CultureInfo.InvariantCulture);
                var endDateTime = default(DateTime);
                if (end != "default") { endDateTime = DateTime.ParseExact(end, "dddMMMddyyyyhhmmtt", CultureInfo.InvariantCulture); } 

                var pc = pcSvc.CreatePartnerCall( new PartnerCall() { Comment = comment, StartDateTime = startDateTime, 
                    EndDateTime = endDateTime, ForIndoor = true, ForOutdoor = true, PlaceID = place.ID, 
                    PreferredLevel = byte.Parse(preferredLevel) }
                );

                var user = CfPerfCache.GetClimber(CfIdentity.UserID);

                var dto = new PartnerCallDto(place, pc, user);

                if (!pcSvc.UserHasSubscriptionForRelatedGeo(place.ID))
                {
                    pcSvc.CreatePartnerCallSubscription(new PartnerCallSubscription() {
                        ForIndoor = pc.ForIndoor,
                        ForOutdoor = pc.ForOutdoor,
                        PlaceID = pc.PlaceID,
                        EmailRealTime = true,
                        MobileRealTime = false,
                        ExactMatchOnly = false
                    });
                }

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("PartnerCall failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "update-climb-grade/{id}/{grade}")]
        public Message UpdateClimbGrade(string id, string grade)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid ciID = Guid.ParseExact(id, "N");
                var climb = geoSvc.GetClimbByID(ciID);
                if (climb.Type == CfType.ClimbIndoor && climb.GradeLocal != "Unestablished")
                {
                    throw new Exception("Indoor climbs that already have a grade cannot be updated from Climbfind Mobile.");
                }
                else
                {
                    geoSvc.UpdateClimbGrade(climb, grade);
                    if (climb.Type == CfType.ClimbIndoor)
                    {
                        var c = geoSvc.GetIndoorClimbByID(climb.ID);
                        return ReturnAsJson(new ClimbIndoorDetailDto(c));
                    }
                    else // if (climb.Type == CfType.ClimbOutdoor)
                    {
                        var c = geoSvc.GetOutdoorClimbByID(climb.ID);
                        return ReturnAsJson(new ClimbOutdoorDetailDto(c));
                    }
                }
            }
            catch (Exception ex)
            {
                return Failed("Update grade failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "update-user-device/{type}")]
        public Message UpdateDeviceTypeSiteSettings(string type)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            
            try
            {
                var alrSvc = new AlertsService();
                var userSettings = alrSvc.GetUserSiteSettings(CfIdentity.UserID);
                userSettings.DeviceTypeRegistered = type;
                alrSvc.UpdatedUserSiteSettings(userSettings);
                return ReturnAsJson(new { ID = userSettings.ID, DeviceTypeRegistered = userSettings.DeviceTypeRegistered });
            }
            catch (Exception ex)
            {
                return Failed("Update settings failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "update-check-in/{id}?comment={comment}")]
        public Message UpdateCheckInComment(string id, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            
            try
            {
                Guid ciID = Guid.ParseExact(id, "N");
                var ci = visitsSvc.GetCheckInById(ciID);
                ci.Comment = comment;

                visitsSvc.UpdateCheckIn(ci);

                return ReturnAsJson(new { Success = true });
            }
            catch (Exception ex)
            {
                return Failed("Update check in comment failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "check-out/{id}")]
        public Message CheckOut(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid ciID = Guid.ParseExact(id, "N");
                var ci = visitsSvc.GetCheckInById(ciID);

                //-- in case the visit has been deleted and we're trying to check out via the app.
                if (ci == null) { return ReturnAsJson(new VisitDto()); }
                else
                {
                    var co = visitsSvc.CheckOut(ciID);
                    var location = CfPerfCache.GetLocation(co.LocationID);
                    return ReturnAsJson(new VisitDto(co, location, CfIdentity.DisplayName, ""));
                }
            }
            catch (Exception ex)
            {
                return Failed("Check out failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "log-climb/{epochUtc}/{climbID}/{outcome}/{experience}/{gradeOpinion}/{rating}?comment={comment}")]
        public Message LogClimb(string epochUtc, string climbID, string outcome, string experience, string gradeOpinion, string rating, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            
            try
            {
                var milliseconds = long.Parse(epochUtc);
                var loggedUtc = new DateTime(1970, 01, 01).AddMilliseconds(milliseconds);
                
                var cID = Guid.ParseExact(climbID, "N");
                var climb = geoSvc.GetClimbByID(cID);

                var loc = CfCacheIndex.Get(climb.LocationID);

                var visit = new VisitsService().GetVisitByLocationAndUtc(loc, loggedUtc);
                
                var log = visitsSvc.LogClimb(visit, new LoggedClimb(){
                    ClimbID = climb.ID,
                    Comment = comment,
                    Experince = byte.Parse(experience),
                    Outcome = byte.Parse(outcome),
                    GradeOpinion = byte.Parse(gradeOpinion),
                    Rating = byte.Parse(rating),
                    Utc = loggedUtc
                });

                var dto = new LoggedClimbDetailDto(log, climb.GradeLocal, climb.Avatar, "");

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Log climb failed : " + ex.Message + " " + ex.Source);
            }
        }

        [WebGet(UriTemplate = "log-climb-update/{logID}/{outcome}/{experience}/{gradeOpinion}/{rating}?comment={comment}")]
        public Message LogClimbUpdate(string logID, string outcome, string experience, string gradeOpinion, string rating, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var lID = Guid.ParseExact(logID, "N");

                var log = visitsSvc.GetLoggedClimbById(lID);
                if (log == default(LoggedClimb)) { throw new ArgumentException("Logged climb does not exist as part of the specified check in!"); }

                log.Comment = comment;
                log.Experince = byte.Parse(experience);
                log.Outcome = byte.Parse(outcome);
                log.GradeOpinion = byte.Parse(gradeOpinion);
                log.Rating = byte.Parse(rating);

                visitsSvc.LogClimbUpdate(log);

                var dto = new LoggedClimbDetailDto(log, "", "", "");

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Log climb failed : " + ex.Message + " " + ex.Source);
            }
        }

        [WebGet(UriTemplate = "delete-visit/{id}")]
        public Message DeleteVisit(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var ciSvc = new VisitsService();
                
                var checkInID = Guid.ParseExact(id, "N");
                var ci = ciSvc.GetCheckInById(checkInID);
                ciSvc.DeleteCheckIn(ci);

                return ReturnAsJson(new {Success = true});
            }
            catch (Exception ex)
            {
                return Failed("Operation failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "delete-logged-climb/{id}")]
        public Message DeleteLoggedClimb(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var lcID = Guid.ParseExact(id, "N");
                var log = visitsSvc.GetLoggedClimbById(lcID);
                visitsSvc.DeleteLoggedClimb(log.CheckIn, log.ID);
                
                return ReturnAsJson(new { Success = true });
            }
            catch (Exception ex)
            {
                return Failed("Operation failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "get-logged-climb/{id}")]
        public Message GetLoggedClimb(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid loggedClimbID = Guid.ParseExact(id, "N");

            var log = visitsSvc.GetLoggedClimbById(loggedClimbID);
            var climb = geoSvc.GetClimbByID(log.ClimbID);
            return ReturnAsJson(new LoggedClimbDetailDto(log, climb.GradeLocal, climb.Avatar, ""));
        }

        [WebGet(UriTemplate = "get-my-logs-for-climb/{id}")]
        public Message GetMyLogsForClimb(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid climbID = Guid.ParseExact(id, "N");
            var climb = geoSvc.GetClimbByID(climbID); //CfPerfCache.GetClimb(climbID);

            var logs = visitsSvc.GetUsersLoggedClimbs(CfIdentity.UserID).Where(c => c.ClimbID == climb.ID);
            var dto = new List<LoggedClimbDetailDto>();
            foreach (var l in logs) { dto.Add(new LoggedClimbDetailDto(l, climb.GradeLocal, climb.Avatar, "")); }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-logs-for-user/{id}")]
        public Message GetLogsForUser(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            Guid uID = Guid.ParseExact(id, "N");
            var logs = visitsSvc.GetUsersMostRecentLoggedClimbs(uID, 50);
            var dto = new List<LoggedClimbDetailDto>();
            var user = CfPerfCache.GetClimber(uID);
           
            foreach (var l in logs) { 
                                                
                dto.Add(new LoggedClimbDetailDto(l, l.Climb.GradeLocal, l.Climb.Avatar, ""));
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-logs-for-climb/{id}")]
        public Message GetLogsForClimb(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid climbID = Guid.ParseExact(id, "N");
            var climb = geoSvc.GetClimbByID(climbID);

            var logs = visitsSvc.GetClimbsLogs(climbID, 25);
            var dto = new List<LoggedClimbDetailDto>();
            foreach (var l in logs) {
                var user = CfPerfCache.GetClimber(l.UserID);
                dto.Add(new LoggedClimbDetailDto(l, climb.GradeLocal, user.Avatar, user.DisplayName)); }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-latest-sends/{id}")]
        public Message GetLatestSends(string id)
        {
            Guid climbID = Guid.ParseExact(id, "N");
            var logs = visitsSvc.GetClimbsRecentSuccessfulLoggedClimbs(climbID, 25).ToList();
            var dto = new List<ClimbDetailSentClimbDto>();
            foreach (var l in logs.OrderByDescending(l => l.Utc))
            {
                var by = CfPerfCache.GetClimber(l.UserID);
                dto.Add(new ClimbDetailSentClimbDto(l, by));
            }
            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "get-latest-sends-by-user/{id}")]
        public Message GetLatestSendsByUser(string id)
        {
            Guid userID = Guid.ParseExact(id, "N");
            var logs = visitsSvc.GetUsersMostRecentLoggedClimbs(userID, 50).ToList();
            var dto = new List<ClimbDetailSentClimbDto>();
            var by = CfPerfCache.GetClimber(userID);
            foreach (var l in logs.OrderByDescending(l => l.Utc))
            {
                var send = new ClimbDetailSentClimbDto(l, by);
                send.By = l.ClimbName;
                dto.Add(send);
            }
            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "feed/geocontext")]
        public Message GetLatestGeoFeedPosts()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            return GetMobilePosts(() => postSvc.GetGeoFeed(ctx.Lat, ctx.Lon, PostType.Unknown, ClientAppType.CfiPhone));
        }

        [WebGet(UriTemplate = "feed/preference")]
        public Message GetLatestUserPreferencesFeedPosts()
        {
            return GetMobilePosts(() => postSvc.GetUsersFeed(CfIdentity.UserID, PostType.Unknown, ClientAppType.CfiPhone).Posts);
        }

        [WebGet(UriTemplate = "feed/everywhere")]
        public Message GetLatestEverywherFeedPosts()
        {
            return GetMobilePosts(() => postSvc.GetPostForEverywhere(PostType.Unknown, ClientAppType.CfiPhone));
        }

        [WebGet(UriTemplate = "feed/place/{id}")]
        public Message GetLatestPlaceFeedPosts(string id)
        {
            Guid pID = Guid.ParseExact(id, "N");
            var place = CfCacheIndex.Get(pID);
            if (place.Type.ToPlaceCateogry() == PlaceCategory.Area) { return GetMobilePosts(() => postSvc.GetPostForArea(pID, PostType.Unknown, ClientAppType.CfiPhone)); }
            else { return GetMobilePosts(() => postSvc.GetPostForLocation(pID, PostType.Unknown, ClientAppType.CfiPhone)); }
        }

        [WebGet(UriTemplate = "feed/user/{id}")]
        public Message GetLatestUserFeedPosts(string id)
        {
            Guid userID = Guid.ParseExact(id, "N");
            return GetMobilePosts(()=>postSvc.GetPostForUser(userID, ClientAppType.CfiPhone, 20));
        }

        private Message GetMobilePosts(Func<List<PostRendered>> getPostsDelegate)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            var dto = new List<PostDto>();
            var posts = getPostsDelegate();
            foreach (var p in posts)
            {                
                dto.Add(GetMobilePost(p));
            }

            return ReturnAsJson(dto);
        }

        private PostDto GetMobilePost(PostRendered p)
        {
            var post = new PostDto(p, CfCacheIndex.Get(p.PlaceID), p.UserDisplayName, p.UserAvatar);
            foreach (var c in p.PostComments)
            {
                var profile = CfPerfCache.GetClimber(c.UserID);
                post.Comments.Add(new PostCommentDto(c, profile));
            }
            return post;
        }

        [WebGet(UriTemplate = "feed/post/{id}")]
        private Message GetPost(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid postID = Guid.ParseExact(id, "N");

            return ReturnAsJson(GetMobilePost(postSvc.GetPostRenderedByID(postID, ClientAppType.CfiPhone)));
        }

        //[WebGet(UriTemplate = "talk/{id}?comment={comment}")]
        //public Message Talk(string id, string comment)
        //{
        //    SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

        //    try
        //    {
        //        var locID = Guid.ParseExact(id, "N");
        //        var talk = cf.Content.Feed.ContentRenderer.BindPostsContentForWeb( new List<Post> { postSvc.CreateTalkPost(locID, comment) })[0];
        //        var dto = new PostDto(talk, GetObjectName(talk.PlaceID), "", "");

        //        return ReturnAsJson(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Failed("Log climb failed : " + ex.Message);
        //    }
        //}
        
        [WebGet(UriTemplate = "comment/{id}?comment={comment}")]
        public Message CommentOnPost(string id, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
           
            try
            {
                var postID = Guid.ParseExact(id, "N");
                var post = postSvc.GetPostByID(postID);
                var com = postSvc.CreateComment(post, comment);

                var profile = CfPerfCache.GetClimber(CfIdentity.UserID);

                var dto = new PostCommentDto(com, profile);

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Comment failed : " + ex.Message);
            }      
        }

        [WebGet(UriTemplate = "leave-opinion/{id}/{rating}?comment={comment}")]
        public Message LeaveOpinion(string id, string rating, string comment)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var objID = Guid.ParseExact(id, "N");
                var obj = AppLookups.GetCacheIndexEntry(objID);

                var opinion = new ContentService().CreateOpinion(new Opinion() { Comment = comment,
                      ObjectID = obj.ID, Rating = byte.Parse(rating) }, obj.ID);

                var by = CfPerfCache.GetClimber(CfIdentity.UserID);
                var dto = new OpinionDto(opinion, by);

                return ReturnAsJson(dto);
            }
            catch (Exception ex)
            {
                return Failed("Opinion failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "message/{id}?content={content}")]
        public Message Message(string id, string content)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var userID = Guid.ParseExact(id, "N");

                var msg = new ConversationService().SendMessage(userID, content);

                return ReturnAsJson(new ConversationMessageDto(msg));
            }
            catch (Exception ex)
            {
                return Failed("Message failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "message-partner-call-reply/{id}?content={content}")]
        public Message MessagePartnerCallReply(string id, string content)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var pcID = Guid.ParseExact(id, "N");
                var pc = pcSvc.GetPartnerCallById(pcID);
                var msg = new ConversationService().PrivatePartnerCallReply(pc, content);
               
                return ReturnAsJson(new ConversationMessageDto(msg));
            }
            catch (Exception ex)
            {
                return Failed("Message failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "my-conversations")]
        public Message MyConversations()
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                Guid userID = CfIdentity.UserID;
                var conversations = new ConversationService()
                    .GetUsersConversations(userID).OrderByDescending(c => c.LastActivityUtc).ToList();


                var conversationsListDto = new ConversationListDto();
                foreach (var c in conversations)
                {
                    var otherPartyID = c.PartyAID;
                    var myView = c.ConversationViews.Where(cv => cv.PartyID == CfIdentity.UserID).Single();
                    if (c.PartyAID == CfIdentity.UserID) { otherPartyID = c.PartyBID; }
                    if (myView.ShouldShow)
                    {
                        var with = CfPerfCache.GetClimber(otherPartyID);
                        conversationsListDto.Conversations.Add(new ConversationItemDto(c, with));
                    }
                }

                return ReturnAsJson(conversationsListDto);
            }
            catch (Exception ex)
            {
                return Failed("Message failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "get-conversation/{id}")]
        public Message GetConversation(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }

            try
            {
                var userID = Guid.ParseExact(id, "N");
                var conversation = new ConversationService().GetConversationByPartyIDs(userID, CfIdentity.UserID); ;

                var me = CfPerfCache.GetClimber(CfIdentity.UserID);
                var with = CfPerfCache.GetClimber(userID);
                var conversationDto = new ConversationDetailDto(conversation, with, me);
                foreach (var m in conversation.Messages) { conversationDto.Messages.Add(new ConversationMessageDto(m)); } 

                return ReturnAsJson(conversationDto);
            }
            catch (Exception ex)
            {
                return Failed("Message failed : " + ex.Message);
            }
        }

        [WebGet(UriTemplate = "best-media/{id}")]
        public Message GetObjectsBestMedia(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid objectID = Guid.ParseExact(id, "N");
            
            var dto = new List<MediaDto>();

            foreach (var m in new MediaService().GetObjectsTopMedia(objectID, 15))
            {
                var profile = CfPerfCache.GetClimber(m.AddedByUserID);
                dto.Add(new MediaDto(m.ID, m.Title, m.TypeID, m.AddedUtc, profile.DisplayName, m.AddedByUserID,
                    profile.Avatar, m.Content, m.Rating, m.RatingCount));
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "latest-media/{id}")]
        public Message GetObjectsLatestMedia(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid objectID = Guid.ParseExact(id, "N");
            
            var dto = new List<MediaDto>();

            foreach (var m in new MediaService().GetObjectsMostRecentMedia(objectID, 15))
            {
                dto.Add(new MediaDto(m.ID, m.Title, m.TypeID, m.AddedUtc, m.Profile.DisplayName, m.AddedByUserID,
                    m.Profile.Avatar, m.Content, m.Rating, m.RatingCount));
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "latest-opinions/{id}")]
        public Message GetObjectsOpinions(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid objectID = Guid.ParseExact(id, "N");

            var dto = new List<OpinionDto>();

            var latestOpinions = new ContentService().GetLatestOpinionsOnObject(objectID, 20);
            foreach (var m in latestOpinions)
            {
                var profile = CfPerfCache.GetClimber(m.UserID);
                dto.Add(new OpinionDto(m.ID, m.Rating, m.Utc, profile.DisplayName, m.UserID,
                    profile.Avatar, m.Comment));
            }

            return ReturnAsJson(dto);
        }

        [WebGet(UriTemplate = "latest-opinions-by-user/{id}")]
        public Message GetUsersOpinions(string id)
        {
            SvcContext ctx = InflateContext(); if (ctx.Invalid) { return ctx.ContextMessage; }
            Guid userID = Guid.ParseExact(id, "N");

            var dto = new List<OpinionDto>();

            var latestOpinions = new ContentService().GetUsersLatestOpinions(userID, 30);
            var profile = CfPerfCache.GetClimber(userID);
            foreach (var m in latestOpinions)
            {
                var obj = AppLookups.GetCacheIndexEntry(m.ObjectID);
                if (obj != null)
                {
                    dto.Add(new OpinionDto(m.ID, m.Rating, m.Utc, obj.Name, m.UserID,
                        profile.Avatar, m.Comment));
                }
            }

            return ReturnAsJson(dto);
        }
            
        //[WebGet]
        //public string CreateOutdoorLocation(string name, string type, byte country, double lat, double lon)
        //{
        //    try
        //    {
        //        if (!Authenticated()) { return NotAllowed; }

        //        CfType eType; Enum.TryParse<CfType>(type, out eType);

        //        var location = geoSvc.CreateLocationOutdoor(new LocationOutdoor()
        //        {
        //            CountryID = country,
        //            Latitude = lat,
        //            Longitude = lon,
        //            Name = name,
        //            TypeID = (byte)eType
        //        });

        //        geoSvc.CreateLocationOutdoor(location);

        //        return location.ToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Response.StatusCode = 404;
        //        return ex.Message;
        //    }
        //}

        //[WebGet]
        //public string CreateClimb(Guid loc, string name)
        //{
        //    if (!Authenticated()) { return NotAllowed; }

        //    var location = geoSvc.GetLocationByID(loc);
        //    if (location == default(Location))
        //    {
        //        throw new ArgumentException("Cannot add climb: Unrecognized location.");
        //    }

        //    var categoriesList = new List<int>();
        //    var climb = geoSvc.CreateClimb(new Climb { LocationID = location.ID, CountryID = location.CountryID, Name = name }, categoriesList);

        //    return climb.ToJson();
        //}

        //[WebGet]
        //public string UpdateClimb(Guid clm, string grade, string description)
        //{
        //    if (!Authenticated()) { return NotAllowed; }

        //    var climb = geoSvc.GetClimbByID(clm);
        //    if (climb == default(Climb))
        //    {
        //        throw new ArgumentException("Cannot add climb: Unrecognized climb.");
        //    }

        //    var categoriesList = climb.ClimbCategories.Select(c => c.Category).ToList();
        //    var original = climb.GetSimpleTypeClone();
        //    climb.Grade = grade;
        //    climb.Description = description;
        //    geoSvc.UpdateClimb(original, climb, categoriesList);

        //    return climb.ToJson();
        //}


        //[WebGet]
        //public string FlagObject(Guid id, byte reason, string comment)
        //{
        //    throw new NotImplementedException("Going to be implemented really soon");
        //}

        //[WebGet]
        //public string RateMedia(Guid med, byte rating, string comment)
        //{
        //    if (!Authenticated()) { return NotAllowed; }

        //    var media = new MediaRating() { MediaID = med, Rating = rating, Comment = comment };

        //    var rated = new MediaService().CreateMediaRating(media);

        //    return rated.ID.ToString();
        //}
    }
}