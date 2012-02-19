using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using cf.Dtos;

namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VisitsService : AbstractCfService
    {
        CheckInRepository checkInRepo { get { if (_checkInRepo == null) { _checkInRepo = new CheckInRepository(); } return _checkInRepo; } } CheckInRepository _checkInRepo;
        LoggedClimbsRepository logCRepo { get { if (_logCRepo == null) { _logCRepo = new LoggedClimbsRepository(); } return _logCRepo; } } LoggedClimbsRepository _logCRepo;
        MediaRepository medRepo { get { if (_medRepo == null) { _medRepo = new MediaRepository(); } return _medRepo; } } MediaRepository _medRepo;
        ProfileRepository usrRepo { get { if (_usrRepo == null) { _usrRepo = new ProfileRepository(); } return _usrRepo; } } ProfileRepository _usrRepo;  
        
        public VisitsService() {}

        public CheckIn GetCheckInById(Guid id) { return checkInRepo.GetCheckInByID(id); }
        public IQueryable<CheckIn> GetMostRecentCheckIns() { return checkInRepo.GetAll().Take(30); }
        public IQueryable<CheckIn> GetUsersVisits(Guid userID) { return checkInRepo.GetAll().Where(c=>c.UserID == userID); }
        public IQueryable<CheckIn> GetUsersHistory(Guid userID) { return checkInRepo.GetUsersHistory(userID); }

        public CheckIn GetVisitByLocationAndUtc(CfCacheIndexEntry loc, DateTime loggedClimbUtc) {
            var startDate = loggedClimbUtc.AddHours(-7); //-- We go backwards incase they quick logged...
            var endDate = loggedClimbUtc.AddHours(7);

            var visit = checkInRepo.GetAll().Where( ci => ci.LocationID == loc.ID && 
                ci.Utc > startDate && ci.Utc < endDate && ci.UserID == CfIdentity.UserID).SingleOrDefault(); 
            
            //-- First climb we've logged for the visit
            if (visit == default(CheckIn)) { 
                visit = CreateCheckIn( new CheckIn() { Utc = loggedClimbUtc.AddMinutes(-10), LocationID = loc.ID, Comment = 
                    string.Format("I'm at {0} logging climbs!", loc.Name), IsPrivate = false });
            }

            return visit;
        }

        /// <summary>
        /// This is a bit hack, we are hijacking the Location object & LocationRepository and using it like a dto to execute
        /// a stored procedure - not cleanest - but fast and good.
        /// </summary>
        /// <remarks>It's in the visits service because the procedure is actually looking at the users visits to get the locations</remarks>
        public List<Location> GetUsersIndoorPlaces(Guid userID) { return new LocationRepository().GetUsersIndoorPlaces(userID); }

        public IQueryable<CheckIn> GetLocationsCheckIns(Guid locationID) 
        { return checkInRepo.GetAll().Where(c => c.LocationID == locationID); }

        public IQueryable<CheckIn> GetLocationsClosestCheckIns(Guid locationID, DateTime checkInUtc) {
            return checkInRepo.GetClosestToDate(locationID, checkInUtc); }

        public IQueryable<CheckIn> GetLocationsLatestCheckIns(Guid locationID, int count) { 
            return checkInRepo.GetAll().Where(c => c.LocationID == locationID).OrderByDescending(c=>c.Utc).Take(count); }

        public CheckIn CreateCurrentCheckIn(CheckIn checkIn)
        {
            checkIn.Utc = DateTime.UtcNow;
            return CreateCheckIn(checkIn);
        }

        public CheckIn CreateCheckIn(CheckIn visit)
        {
            var user =  usrRepo.GetByID(CfIdentity.UserID);
            
            var place = AppLookups.GetCacheIndexEntry(visit.LocationID);
            if (place == null) { throw new ArgumentException("Unable to check in - No location found for " + visit.LocationID); }
            
            visit.ID = Guid.NewGuid();
            visit.UserID = user.ID;
            
            if (visit.Latitude.HasValue && visit.Longitude.HasValue)
            {
                //-- It's a verified check-in, let's do something
            }

            //-- TODO check the person isn't checking in again within 1 hour to the same location

            var ci = checkInRepo.Create(visit);

            postSvc.CreateCheckInPost(ci, user.PrivacyPostsDefaultIsPublic);

            if (!user.PlaceMostRecentUtc.HasValue || ci.Utc > user.PlaceMostRecentUtc.Value)
            {
                user.PlaceMostRecent = visit.LocationID;
                user.PlaceMostRecentUtc = visit.Utc;
                usrRepo.Update(user);
            }

            return ci;
        }

        public CheckIn UpdateCheckIn(CheckIn checkIn)
        {
            if (checkIn.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot edit a check in that does not belong to you"); }
            
            checkInRepo.Update(checkIn);

            //-- Update the post so the feed shows accurate data
            postSvc.UpdateCheckInPost(checkIn);

            return checkIn;
        }

        public CheckIn CheckOut(Guid id)
        {
            CheckIn checkIn = checkInRepo.GetByID(id);
            if (checkIn.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot edit a check in that does not belong to you"); }

            checkIn.OutUtc = DateTime.UtcNow;

            return checkInRepo.Update(checkIn);
        }

        public void DeleteCheckIn(CheckIn checkIn)
        {
            if (checkIn.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot delete a check in that does not belong to you"); }

            checkInRepo.DeleteCheckIn(checkIn);

            //-- Delete the post so the feed shows accurate data
            postSvc.DeleteCheckInPost(checkIn);
        }

        public void AddMedia(CheckIn ci, Media media)
        {
            if (ci.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot add media to a check in that does not belong to you"); }
            if (ci.Media.Count >= 5) { throw new NotSupportedException("Climbfind only supports up to 5 pieces of media per check in."); }
            
            checkInRepo.AddMedia(ci, media);
            
            //-- Make sure our feed post renders correctly
            if (ci.Media.Where(m => m.ID == media.ID).Count() == 0)
            {
                ci.Media.Add(media);
            }

            //-- Update the post so the feed shows accurate data
            postSvc.UpdateCheckInPost(ci);
        }

        public void RemoveMedia(CheckIn ci, Media media)
        {
            if (ci.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot remove media from check in that does not belong to you"); }

            checkInRepo.RemoveMedia(ci, media.ID);
            new MediaService().DeleteMedia(media);

            //-- Update the post so the feed shows accurate data
            postSvc.UpdateCheckInPost(ci);
        }

        public LoggedClimb GetLoggedClimbById(Guid id) { return logCRepo.GetByID(id); }
        public IQueryable<LoggedClimb> GetMostRecentLoggedClimbs(int count) { return logCRepo.GetAll().Take(count); }
        public IQueryable<LoggedClimb> GetUsersLoggedClimbs(Guid userID) { return logCRepo.GetAll().Where(c => c.UserID == userID); }
        public IQueryable<LoggedClimb> GetUsersMostRecentLoggedClimbs(Guid userID, int count) { return logCRepo.GetAllInclude().Where(c => c.UserID == userID).OrderByDescending(c => c.Utc).Take(count); }
        public IQueryable<LoggedClimb> GetCheckInsLoggedClimbs(Guid checkInID) { return logCRepo.GetAll().Where(c => c.CheckInID == checkInID); }
        public IQueryable<LoggedClimb> GetClimbsLoggedClimbs(Guid climbID) { return logCRepo.GetAll().Where(c => c.ClimbID == climbID).OrderByDescending(c => c.Utc); }
        public IQueryable<LoggedClimb> GetClimbsRecentSuccessfulLoggedClimbs(Guid climbID, int count) { 
            
            return GetClimbsLoggedClimbs(climbID).OrderByDescending(l=>l.Utc).Where( 
                l=>l.Outcome == (int)ClimbOutcome.Redpoint || 
                   l.Outcome == (int)ClimbOutcome.Flash || 
                   l.Outcome == (int)ClimbOutcome.Onsight).Take(count); }

        public IQueryable<LoggedClimb> GetClimbsLogs(Guid climbID, int count)
        {
            return GetClimbsLoggedClimbs(climbID).OrderByDescending(l => l.Utc).Take(count);
        }

        public IQueryable<LoggedClimb> GetLocationsLoggedClimbs(Guid locID, DateTime startDate, DateTime endDate)
        {
            return logCRepo.GetAll().OrderByDescending(l => l.Utc).Where(
                l => l.Denorm_LocationID == locID &&
                     l.Utc > startDate && l.Utc < endDate); 
        }

        public IQueryable<LoggedClimb> GetLocationsLatestLoggedClimbs(Guid locID, int count)
        {
            return logCRepo.GetAll().OrderByDescending(l => l.Utc).Where(
                l => l.Denorm_LocationID == locID).Take(count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkIn"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public LoggedClimb LogClimb(CheckIn checkIn, LoggedClimb log)
        {
            var validClimbs = LogClimbAuthorize(checkIn, log);
                        
            var climb = validClimbs.Where(c => c.ID == log.ClimbID).SingleOrDefault();
            log.ClimbName = string.Format("{0} {1}", climb.GradeLocal, climb.Name);
            log.ID = Guid.NewGuid();
            log.UserID = CfIdentity.UserID;
            log.Denorm_LocationID = checkIn.LocationID;
            log.CheckInID = checkIn.ID;
            
            //-- If the check in is not live (current-ish) we use the check in time as the log reference
            if (log.Utc == default(DateTime)) { log.Utc = DateTime.UtcNow; }
            if (checkIn.Utc < DateTime.UtcNow.AddDays(-1)) { log.Utc = checkIn.Utc; }
            
            var existingWithClimb = checkIn.LoggedClimbs.Where(l=>l.ClimbID == climb.ID);
            if (existingWithClimb.Count() > 0) {
                var existing = existingWithClimb.First();
                DeleteLoggedClimb(null, existing.ID);
                checkIn.LoggedClimbs.Remove(existing);
            }

            logCRepo.Create(log);

            //-- Make sure our feed post renders correctly
            checkIn.LoggedClimbs.Add(log);

            //-- Create our opinion against the climb object
            //-- note we forfeit the creation of a post for the feed about the opinion by passing in "Guid.Empty"
            new ContentService().CreateOpinion(new Opinion() { ID = Guid.NewGuid(), Comment = log.Comment, Utc = log.Utc, 
                    ObjectID = log.ClimbID, Rating = log.Rating, UserID = CfIdentity.UserID }, Guid.Empty);

            //-- Update the post so the feed shows accurate data
            postSvc.UpdateCheckInPost(checkIn);

            return log;
        }

        public LoggedClimb LogClimbUpdate(LoggedClimb log)
        {      
            logCRepo.Update(log);

            //-- Create our opinion against the climb object
            //-- note we forfeit the creation of a post for the feed about the opinion by passing in "Guid.Empty"
            new ContentService().CreateOpinion(new Opinion() { ID = Guid.NewGuid(), Comment = log.Comment, Utc = log.Utc, 
                    ObjectID = log.ClimbID, Rating = log.Rating, UserID = CfIdentity.UserID }, Guid.Empty);

            //-- Update the post so the feed shows accurate data
            postSvc.UpdateCheckInPost(new CheckInRepository().GetCheckInByID(log.CheckInID));

            return log;
        }

        public void DeleteLoggedClimb(CheckIn ci, Guid loggedClimbID)
        {
            var log = GetLoggedClimbById(loggedClimbID);
            //-- TODO more
            logCRepo.Delete(log.ID);

            //-- Update the post so the feed shows accurate data
            //-- If ci is null it meas we're deleting an existing entry that is about to be updated and the post will
            //-- be updated regardless
            if (ci != null) { postSvc.UpdateCheckInPost(ci); }
        }
    }
}
