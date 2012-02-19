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
using pcWorkRepository = cf.DataAccess.Repositories.PartnerCallNotificationWorkItemRepository;
using PCSubscription = cf.Entities.PartnerCallSubscription;
using cf.Dtos;

namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PartnerCallService : AbstractCfService
    {
        PartnerCallRepository pcRepo { get { if (_pcRepo == null) { _pcRepo = new PartnerCallRepository(); } return _pcRepo; } } PartnerCallRepository _pcRepo;
        PartnerCallSubscriptionRepository pcsRepo { get { if (_pcsRepo == null) { _pcsRepo = new PartnerCallSubscriptionRepository(); } return _pcsRepo; } } PartnerCallSubscriptionRepository _pcsRepo;
        pcWorkRepository pcWRepo { get { if (_pcWRepo == null) { _pcWRepo = new pcWorkRepository(); } return _pcWRepo; } } pcWorkRepository _pcWRepo;

        public PartnerCallService() { }

        public PartnerCall GetPartnerCallById(Guid id) { return pcRepo.GetByID(id); }
        public IQueryable<PartnerCall> GetUsersPartnerCalls(Guid userID) { return pcRepo.GetAll().Where(c => c.UserID == userID && !c.UserDeleted); }
        public IQueryable<PartnerCall> GetUsersLatestPartnerCalls(Guid userID, int count) { 
            return GetUsersPartnerCalls(userID).OrderByDescending(p=>p.CreatedUtc).Take(count); }
        public IQueryable<PartnerCall> GetPlacesPartnerCalls(Guid placeID) { return pcRepo.GetAll().Where(c => c.PlaceID == placeID && !c.UserDeleted); }
        public IQueryable<PartnerCall> GetPlacesLatestPartnerCalls(Guid placeID, int count) {
            return GetPlacesPartnerCalls(placeID).OrderByDescending(p => p.CreatedUtc).Take(count); ;
        }
        public PartnerCallNotificationWorkItem GetPartnerCallWorkItemByPartnerCallId(Guid id) { return pcWRepo.GetAll().Where(pcwi=>pcwi.PartnerCallID == id).SingleOrDefault(); }


        public IQueryable<PartnerCall> GetPlacesGeoDeducPartnerCalls(Guid placeID)
        {
            var geoDeduciblePlaces = new List<CfCacheIndexEntry>();
            geoDeduciblePlaces.AddRange(CfPerfCache.GetGeoDeduciblePlaces(CfCacheIndex.Get(placeID)));
            var placeIDs = geoDeduciblePlaces.Select(p => p.ID).Distinct().ToList();
            return pcRepo.GetAll().Where(c => placeIDs.Contains(c.PlaceID) && !c.UserDeleted);
        }

        public IQueryable<PartnerCall> GetPlacesGeoDeducLatestPartnerCalls(Guid placeID, int count)
        {
            return GetPlacesGeoDeducPartnerCalls(placeID).OrderByDescending(p => p.CreatedUtc).Take(count); ;
        }

        public IQueryable<PartnerCall> GetPlacesGeoDeducTodayPartnerCalls(Guid placeID)
        {
            var startDate = DateTime.UtcNow.AddDays(-2);
            var tomorrowDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(2);
            
            return GetPlacesGeoDeducPartnerCalls(placeID).Where(
                p => (p.StartDateTime > startDate && p.StartDateTime < endDate) ||
                    (p.StartDateTime < tomorrowDate && p.EndDateTime > startDate))
                    .OrderByDescending(p => p.StartDateTime).Take(5);
        }

        /// <summary>
        /// Create a partner call and it's associated work item object that tracks replies
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public PartnerCall CreatePartnerCall(PartnerCall obj)
        {
            CreatePartnerCallAuthorization(obj);

            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            
            obj.ID = Guid.NewGuid();
            obj.UserID = user.ID;
            obj.CreatedUtc = DateTime.UtcNow;

            //-- Sanitize the call subscription based on place type
            var place = CfCacheIndex.Get(obj.PlaceID);
            if (place.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing) { obj.ForOutdoor = false; }
            else if (place.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { obj.ForIndoor = false; }

            //-- default start time to midday if not set
            if (obj.StartDateTime == obj.StartDateTime.Date) { obj.StartDateTime = obj.StartDateTime.AddHours(12); }
            if (obj.EndDateTime == default(DateTime)) { obj.EndDateTime = obj.StartDateTime.AddHours(5); }
            
            pcRepo.Create(obj);

            postSvc.CreatePartnerCallPost(obj, user.PrivacyPostsDefaultIsPublic);

            new AlertsService().EnquePartnerCallWorkItem(CfIdentity.UserID, obj.PlaceID, obj.ID);

            return obj;
        }

        private void CreatePartnerCallAuthorization(PartnerCall obj)
        {
            var place = CfCacheIndex.Get(obj.PlaceID);

            if (place.Type == CfType.Country || place.Type == CfType.Province)
            {
                throw new Exception("You can not post for a country or a province as this will cause too many people to receive an alert about your partner call."); 
            }
            if (place.Type == CfType.ClimbingArea)
            {
                var area = new AreaRepository().GetByID(place.ID);
                if (area.DisallowPartnerCalls)
                {
                    throw new Exception("You can not post for "+area.Name+" as it has been disallowed by admin probably because it will too many people to receive an alert about your partner call. Please choose a smaller related area."); 
                }
            }
            
            ////-- Check user isn't submitting multiple within 1 hour
            var usersMostRecentCalls = GetUsersLatestPartnerCalls(CfIdentity.UserID, 3).ToList();
            var similarInLastHour = usersMostRecentCalls.Where(pc => pc.ID == obj.ID && pc.CreatedUtc > DateTime.UtcNow.AddHours(-1));
            if (similarInLastHour.Count() > 0) { throw new Exception("You can only post once every hour for the same place other wise you will cause too many people to receive alerts about your calls."); }

            if (!obj.ForIndoor && !obj.ForOutdoor) { throw new ArgumentOutOfRangeException("Cannot create a partner call with neither indoor or outdoor climbing specified"); }
            if (obj.StartDateTime < DateTime.Now.AddDays(-1)) { throw new ArgumentOutOfRangeException("Cannot create a partner call with the start date " + obj.StartDateTime + " as it is in the past"); }

        }

        /// <summary>
        /// Delete a partner call and it's associated work item.
        /// ** TODO Does that make sense to delete both?
        /// </summary>
        /// <param name="obj"></param>
        public void DeletePartnerCall(PartnerCall obj)
        {
            if (obj.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot delete a partner call that does not belong to you"); }
            
            var workItem = pcWRepo.GetAll().Where(i => i.PartnerCallID == obj.ID).SingleOrDefault();
            
            if (workItem == null) { pcRepo.Delete(obj.ID); }
            else if (workItem.NotificationsSent == 0) 
            { 
                pcWRepo.Delete(workItem.ID);
                pcRepo.Delete(obj.ID);
            }
            else
            {
                obj.UserDeleted = true;
                pcRepo.Update(obj);
            }

            postSvc.DeletePartnerCallPost(obj);
        }

        
        /// <summary>
        /// Log when someone responds to a partner call by public comment
        /// </summary>
        /// <param name="post"></param>
        /// <param name="postComment"></param>
        public void LogPartnerCallCommentReply(Post post, PostComment postComment)
        {
            //-- Can't reply to yourself.
            if (post.UserID != postComment.UserID)
            {
                var pc = pcRepo.GetByID(post.ID);
            
                var pcr = new PartnerCallReply() { ID = Guid.NewGuid(), Message = postComment.Message, PartnerCallID = post.ID,
                     TypeID = (byte)PartnerCallReplyType.Comment, Utc = DateTime.UtcNow, UserID = postComment.UserID };

                AddPartnerCallSubscriptionWorkItemReply(pc, pcr);            
            }
        }

        /// <summary>
        /// Log when someone responds to a partner call by private message
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="message"></param>
        public void LogPartnerCallMessageReply(PartnerCall pc, Message message)
        {
            if (pc.UserID == message.SenderID) { throw new ArgumentException("Cannot reply to your own partner call!!"); }

            var pcr = new PartnerCallReply() { ID = Guid.NewGuid(), Message = message.Content, PartnerCallID = pc.ID,
                 TypeID = (byte)PartnerCallReplyType.Message, Utc = DateTime.UtcNow, UserID = message.SenderID };

            AddPartnerCallSubscriptionWorkItemReply(pc, pcr);
        }

        public void LogPartnerCallExternalReply(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Increments the reply counter for a partner call (to keep track of how well Climbfind/P.C's are going)
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="pcr"></param>
        private void AddPartnerCallSubscriptionWorkItemReply(PartnerCall pc, PartnerCallReply pcr)
        {
            var item = pcWRepo.GetAll().Where(i => i.PartnerCallID == pc.ID).SingleOrDefault();
            if (item != default(PartnerCallNotificationWorkItem))
            {
                item.ReplyCount++;
                pcWRepo.Update(item);
            }

            var p = pcRepo.GetAll().Where(i => i.ID == pc.ID).SingleOrDefault();
            p.PartnerCallReplys.Add(pcr);
            pcRepo.Update(p);
        }



        public PCSubscription GetPartnerCallSubscriptionById(Guid id) { return pcsRepo.GetByID(id); }
        public IQueryable<PCSubscription> GetUsersPartnerCallSubscriptions(Guid id) { return pcsRepo.GetAll().Where(ps => ps.UserID == id); }
        public IQueryable<PCSubscription> GetPlacesPartnerCallSubscriptions(Guid placeID)
        {
            return pcsRepo.GetAll().Where(c => c.PlaceID == placeID);
        }

        /// <summary>
        /// Basically check if user will receive notifications for the place they just posted for
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UserHasSubscriptionForRelatedGeo(Guid placeID) { 
            var usersSubscriptions = GetUsersPartnerCallSubscriptions(CfIdentity.UserID).Select(p=>p.PlaceID).ToList();
            if (usersSubscriptions.Contains(placeID)) { return true; }
            else
            {
                var related = CfPerfCache.GetGeoDeduciblePlaces(CfCacheIndex.Get(placeID)).Select(p => p.ID).ToList();
                foreach (var id in related)
                {
                    if (usersSubscriptions.Contains(id)) { return true; }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public PCSubscription CreatePartnerCallSubscription(PCSubscription obj)
        {
            var place = CfCacheIndex.Get(obj.PlaceID);
            
            //-- Sanitize the call subscription based on place type
            if (place.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing) { obj.ForOutdoor = false; }
            else if (place.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { obj.ForIndoor = false; }
            
            if (!obj.ForIndoor && !obj.ForOutdoor) { throw new ArgumentException("Cannot create a subscription without either indoor or outdoor climbing chosen."); }

            //-- Check not adding a duplicate subscription...
            
            //-- Check not adding related subscription??
            
            obj.CreatedUtc = DateTime.UtcNow;
            obj.ID = Guid.NewGuid();
            obj.UserID = CfIdentity.UserID;

            pcsRepo.Create(obj);

            return obj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public PCSubscription UpdatePartnerCallSubscription(PCSubscription obj)
        {
            if (obj.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot update a partner call subscription that does not belong to you"); }
            return obj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void DeletePartnerCallSubscription(PCSubscription obj)
        {
            if (obj.UserID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot delete a partner call subscription that does not belong to you"); }

            pcsRepo.Delete(obj.ID);
        }
    }
}
