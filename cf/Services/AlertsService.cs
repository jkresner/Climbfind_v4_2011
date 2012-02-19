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
using PCNWorkItem = cf.Entities.PartnerCallNotificationWorkItem;
using cf.Dtos;
using cf.Instrumentation;
using cf.Mail;
using cf.DataAccess.Azure;
using cf.Dtos.Cloud;
using Microsoft.WindowsAzure.StorageClient;

namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AlertsService
    {
        public const string MessagesQueueName = "alr-message";
        public const string PartnerCallQueueName = "alr-partner-call";
        public const string CommentQueueName = "alr-comment-on-post";
        
        PartnerCallRepository pcRepo { get { if (_pcRepo == null) { _pcRepo = new PartnerCallRepository(); } return _pcRepo; } } PartnerCallRepository _pcRepo;
        PartnerCallSubscriptionRepository pcsRepo { get { if (_pcsRepo == null) { _pcsRepo = new PartnerCallSubscriptionRepository(); } return _pcsRepo; } } PartnerCallSubscriptionRepository _pcsRepo;
        pcWorkRepository pcWRepo { get { if (_pcWRepo == null) { _pcWRepo = new pcWorkRepository(); } return _pcWRepo; } } pcWorkRepository _pcWRepo;
        AlertRepository alertRepo { get { if (_alertRepo == null) { _alertRepo = new AlertRepository(); } return _alertRepo; } } AlertRepository _alertRepo;
        UserSiteSettingsRepository uStgsRepo { get { if (_uStgsRepo == null) { _uStgsRepo = new UserSiteSettingsRepository(); } return _uStgsRepo; } } UserSiteSettingsRepository _uStgsRepo;

        public AlertsService() { }

        /// <summary>
        /// Basic alert entity read operations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Alert GetAlertByID(Guid id) { return alertRepo.GetByID(id); }
        public IQueryable<Alert> GetUsersAlerts(Guid id) { return alertRepo.GetAll().Where(r => r.UserID == id); }
        public IQueryable<Alert> GetUsersLatestAlerts(Guid id, int count)
        {
            return alertRepo.GetAll().Where(r => r.UserID == id && r.ByFeed).OrderByDescending(r => r.Utc).Take(count);
        }

        /// <summary>
        /// Basic settings read operations
        /// </summary>
        public UserSiteSettings GetUserSiteSettings(Guid id) 
        {
            var stgs = uStgsRepo.GetByID(id);
            if (stgs == null)
            {
                stgs = uStgsRepo.Create(new UserSiteSettings() { ID = id, CommentsOnMediaAlertFeed = true, 
                    CommentsOnMediaEmailRealTime = true, CommentsOnPostsAlertFeed = true, CommentsOnPostsEmailRealTime = true,
                    MessagesEmailRealTime = true, MessagesAlertFeed = true, RssEnabled = false, RssID = Guid.NewGuid() } ); 
            }
            
            return stgs;
        }

        public UserSiteSettings UpdatedUserSiteSettings(UserSiteSettings settings)
        {
            if (settings.ID != CfIdentity.UserID) { throw new UnauthorizedAccessException("Cannot update settings that do not belong to you"); }

            return uStgsRepo.Update(settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="fromID"></param>
        /// <param name="content"></param>
        public void EnqueMessageWorkItem(Guid toID, Guid fromID, string content)
        {
            var dto = new MessageAlertWorkItem() { ToID = toID, FromID = fromID, Content = content };

            if (Stgs.IsDevelopmentEnvironment) { ProcessMessageWorkItem(dto); }
            else
            {
                var queueMessage = new CloudQueueMessage(dto.ToJson());
                new QueueRepository().PutMessage(MessagesQueueName, queueMessage);
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessageWorkItem(MessageAlertWorkItem message)
        {
            var settings = GetUserSiteSettings(message.ToID);
            if (settings.MessagesAlertFeed || settings.MessagesEmailRealTime || settings.MessagesMobileRealTime)
            {
                var excerpt = message.Content;
                if (excerpt.Length > 30) { excerpt = excerpt.Substring(0, 30); }

                var from = CfPerfCache.GetClimber(message.FromID);
                var msg = string.Format("<a href='/climber/{0}'>{1}</a> sent you a <a href='/message/{0}'>Message</a><i> \"{2}...\"</i></a>",
                      from.ID, from.DisplayName, excerpt);

                //-- Not sure why PostID is neccessary.... hmmmmmm
                var a = new Alert() { ID = Guid.NewGuid(), Utc = DateTime.UtcNow, TypeID = (byte)AlertType.Message, UserID = message.ToID, Message = msg };
                if (settings.MessagesAlertFeed) { a.ByFeed = true; }
                if (settings.MessagesEmailRealTime) { a.ByEmail = true; MailMan.SendUserMessageEmail(CfPerfCache.GetClimber(message.ToID), from, message.Content); }
                if (settings.MessagesMobileRealTime && !string.IsNullOrEmpty(settings.DeviceTypeRegistered)) { a.ByMobile = true;
                    MobilePush_MessageAlert(from.DisplayName, CfPerfCache.GetClimber(message.ToID).Email, settings.DeviceTypeRegistered);
                }
                new AlertRepository().Create(a);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="fromID"></param>
        /// <param name="postID"></param>
        /// <param name="content"></param>
        public void EnqueCommentWorkItem(Guid byID, Guid postID, string content)
        {
            var dto = new CommentAlertWorkItem() { ByID = byID, PostID = postID, Content = content };

            if (Stgs.IsDevelopmentEnvironment) { ProcessCommentWorkItem(dto); }
            else
            {
                var queueMessage = new CloudQueueMessage(dto.ToJson());
                new QueueRepository().PutMessage(CommentQueueName, queueMessage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ProcessCommentWorkItem(CommentAlertWorkItem comment)
        {
            var post = new PostRepository().GetByID(comment.PostID);
            var userIDsOnThread = post.PostComments.Where(pc => pc.UserID != comment.ByID).Select(pc => pc.UserID).Distinct().ToList();
            var by = CfPerfCache.GetClimber(comment.ByID);

            var excerpt = comment.Content;
            if (excerpt.Length > 30) { excerpt = excerpt.Substring(0, 30); }
            var msg = string.Format("<a href='/climber/{0}'>{1}</a> made a <a href='/post/{2}'>comment</a><i> \"{3}...\"</i></a>",
                          by.ID, by.DisplayName, comment.PostID, excerpt);

            //-- Also need to let the person who posted know incase they haven't commented on their own post or already part of the commentors
            if (by.ID != post.UserID && !userIDsOnThread.Contains(post.UserID)) { userIDsOnThread.Add(post.UserID); }

            foreach (var uID in userIDsOnThread)
            {
                var settings = GetUserSiteSettings(uID);
                if (settings.CommentsOnPostsAlertFeed || settings.CommentsOnPostsEmailRealTime || settings.CommentsOnPostsMobileRealTime)
                {
                    var toUser = CfPerfCache.GetClimber(uID);

                    var a = new Alert() { ID = Guid.NewGuid(), Utc = DateTime.UtcNow, TypeID = (byte)AlertType.CommentOnPost, UserID = toUser.ID, Message = msg };
                    if (settings.CommentsOnPostsAlertFeed) { a.ByFeed = true; }
                    if (settings.CommentsOnPostsEmailRealTime) { a.ByEmail = true; MailMan.SendCommentEmail(toUser, by, comment.PostID, comment.Content); }
                    if (settings.CommentsOnPostsMobileRealTime && !string.IsNullOrEmpty(settings.DeviceTypeRegistered)) { a.ByMobile = true;
                        MobilePush_CommentAlert(comment.PostID, by.DisplayName, toUser.Email, settings.DeviceTypeRegistered);
                    }
                    new AlertRepository().Create(a);
                }
            }           
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="fromID"></param>
        /// <param name="content"></param>
        public void EnquePartnerCallWorkItem(Guid byID, Guid placeID, Guid partnerCallID)
        {
            var dto = new PartnerCallAlertWorkItem() { ByID = byID, PlaceID = placeID, PartnerCallID = partnerCallID };

            if (Stgs.IsDevelopmentEnvironment) { ProcessPartnerCallWorkItem(dto); }
            else
            {
                var queueMessage = new CloudQueueMessage(dto.ToJson());
                new QueueRepository().PutMessage(PartnerCallQueueName, queueMessage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        public void ProcessPartnerCallWorkItem(PartnerCallAlertWorkItem dto)
        {
            IfNullThrowArgumentExceptionAndClearMsg(dto, "Cannot process PartnerCallAlertWorkItem, dto object is null.", "");
                        
            var pc = pcRepo.GetByID(dto.PartnerCallID);
            IfNullThrowArgumentExceptionAndClearMsg(pc, "Cannot process PartnerCallAlertWorkItem[{0}], PartnerCall is null.", dto.ToJson());

            var post = new PostRepository().GetByID(pc.ID);
            if (post == null) { pcRepo.Delete(pc.ID); return; } //-- If they've deleted the post, let's not send out anything and delete the original call...
    
            var place = AppLookups.GetCacheIndexEntry(pc.PlaceID);
            //if (place == null) //-- Maybe we tried to read when 
            //{
            //
            //}

            IfNullThrowArgumentExceptionAndClearMsg(place, "Cannot process PartnerCallAlertWorkItem[{0}] for place[{1}], Place is null.", pc.ID, pc.PlaceID);
            
            var by = CfPerfCache.GetClimber(pc.UserID);
            IfNullThrowArgumentExceptionAndClearMsg(by, "Cannot process PartnerCallAlertWorkItem[{0}] for byUser[{1}], User is null.", pc.ID, pc.UserID);
            
            PCNWorkItem pcnWi = new PartnerCallNotificationWorkItem() { ID = Guid.NewGuid(), CreatedUtc = DateTime.UtcNow,
                  CountryID = place.CountryID, OnBehalfOfUserID = by.ID, PartnerCallID = pc.ID, PlaceID = pc.PlaceID };

            var alerts = new List<Alert>();
            
            var msg = string.Format("<a href='/climber/{0}'>{1}</a> posted a <a href='/partner-call/{2}'>PartnerCall&trade;</a> for <a href='{3}'>{4}</a>",
                by.ID, by.DisplayName, pc.ID, place.SlugUrl, place.Name);

            var deduciblePlaces = CfPerfCache.GetGeoDeduciblePlaces(place);
            var subscriptionsByUsers = pcsRepo.GetSubscriptionsForPlaces( deduciblePlaces.Select(p=>p.ID).ToList() )
                                                                .Where(s=>s.UserID != pc.UserID).GroupBy( s=>s.UserID );

            //-- We only want to create one Alert per user for a single partner call
            foreach (var subsUserSet in subscriptionsByUsers)
            {
                try 
                {
                    var subscribedUser = CfPerfCache.GetClimber(subsUserSet.Key);
                    if (subscribedUser == null) { throw new ArgumentException(string.Format("Cannot process partner call subscription alerts for user[{0}] as user is null", subsUserSet.Key)); }

                    var matchingSubs = new List<PCSubscription>();
                    foreach (var sub in subsUserSet) {
                        if (sub.PlaceID == place.ID || !sub.ExactMatchOnly) //-- Here we make sure we don't include subscription with ExactMatchOnly chosen
                        {
                            //-- Make sure we match on Indoor/Outdoor preferences
                            if ((sub.ForIndoor && pc.ForIndoor) || (sub.ForOutdoor && pc.ForOutdoor)) { matchingSubs.Add(sub); }
                        }
                    }

                    if (matchingSubs.Count > 0)
                    {
                        var alert = new Alert() { ID = Guid.NewGuid(), Utc = DateTime.UtcNow, TypeID = (byte)AlertType.PartnerCall,
                            PostID = pc.ID, UserID = subscribedUser.ID, Message = msg };

                        alert.ByFeed = true; //-- Here we always put partner calls in the alert feed (unless at a later date we decide to change this).

                        //-- Default notifications
                        bool sendEmail = false;
                        bool sendMobilePush = false;

                        int count = 1;
                        string subscriptionPlaces = string.Empty;
                        
                        foreach (var sub in matchingSubs)
                        {
                            if (sub.EmailRealTime) { sendEmail = true; }
                            if (sub.MobileRealTime) { sendMobilePush = true; }

                            var p = AppLookups.GetCacheIndexEntry(sub.PlaceID);

                            if (count == 1) { subscriptionPlaces = p.Name; }
                            else if (count == matchingSubs.Count) { alert.Message += string.Format(" & {0}", p.Name); }
                            else { alert.Message += string.Format(", {0}", p.Name); }

                            count++;
                        }

                        if (matchingSubs.Count == 1) { alert.Message += ", <i>matching subscription for " + subscriptionPlaces + ".</i>"; }
                        else { alert.Message += ", <i>matching subscriptions for " + subscriptionPlaces + ".</i>"; }

                        if (sendEmail)
                        {
                            MailMan.SendPartnerCallEmail(subscribedUser, by, place, pc, subscriptionPlaces);
                            alert.ByEmail = true;
                        }

                        //if (sendMobilePush)
                        //{
                        //SendAlertByMobilePush(alert);
                        //    alert.ByMobile = true;
                        //}

                        alerts.Add(alert);
                    }
                } 
                catch (Exception ex) // Here we have a try catch incase it fails for one user we can try and still process the others
                {
                    CfTrace.Error(ex);
                }
            }
            
            pcnWi.NotificationsSent = alerts.Count();
            pcnWi.ProcessedUtc = DateTime.UtcNow;
            pcWRepo.Create(pcnWi);

            //-- We have to check this again to see if they delete the post while we were processing
            var postagain = new PostRepository().GetByID(pc.ID);
            if (postagain != null) 
            {
                new AlertRepository().Create(alerts);
            } 

            CfTrace.Information(TraceCode.PartnerCallNofiticatonWorkItemProcess, "Processed {4} <a href='/climber/{0}'>{1}'s</a> <a href='/partner-call/{2}'>PartnerCall&trade;</a> for {3}.",
                by.ID, by.DisplayName, pc.ID, place.Name, alerts.Count);
        }


        private void IfNullThrowArgumentExceptionAndClearMsg(object obj, string format, params object[] args)
        {
            if (obj == null) { throw new ArgumentNullException(string.Format(format, args)); } 
        }
    }
}
