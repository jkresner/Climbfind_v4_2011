using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using NetFrameworkExtensions;
using cf.Entities.Interfaces;
using cf.Content.Images;
using cf.Caching;
using cf.Entities.Enum;
using cf.Identity;
using cf.Dtos;

namespace cf.Services
{
    public partial class GeoService
    {
        ObjectModMetaRepository objModMetaRepo { get { if (_modPlaceRepo == null) { _modPlaceRepo = new ObjectModMetaRepository(); } return _modPlaceRepo; } } ObjectModMetaRepository _modPlaceRepo;
        ModProfileRepository modProfileRepo { get { if (_modProfileRepo == null) { _modProfileRepo = new ModProfileRepository(); } return _modProfileRepo; } } ModProfileRepository _modProfileRepo;
        ModActionRepository modActionRepo { get { if (_modActionRepo == null) { _modActionRepo = new ModActionRepository(); } return _modActionRepo; } } ModActionRepository _modActionRepo;
        
        /// <summary>
        /// Record the action taken by a moderator by setting the action on the object which it is related to, updates the moderators profiles (adds points)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <param name="modMeta"></param>
        /// <param name="modMetaUpdateAction"></param>
        /// <param name="modProfileUpdateAction"></param>
        /// <param name="descriptionFormat"></param>
        /// <param name="descriptionArgs"></param>
        internal ModAction SaveModActionAndUpdateModProfile<T>(ModActionType type, T original, T updated, ObjectModMeta modMeta,
            Action<ObjectModMeta, Guid> modMetaUpdateAction, Action<ModProfile> modProfileUpdateAction,
            string descriptionFormat, params object[] descriptionArgs) where T : IOOObject, new ()
        {
            ModAction action;
            //-- Generate a unique ID for the action
            var newActionID = Guid.NewGuid();

            //-- Look up the points associated with the action
            byte reputationPointsForAction = type.GetPoints();

            //-- Description (Used for the mod action feed)
            string actionDescription = currentUser.FullName + " " + string.Format(descriptionFormat, descriptionArgs);

            //-- TODO wrap this in a transaction/single data hit
            {
                //-- Step 1 save the mod action
                action = modActionRepo.Create(new ModAction()
                {
                    ID = newActionID,
                    TypeID = (int)type,
                    OnObjectID = modMeta.ID,
                    Description = actionDescription,
                    Data = original.DifferenceAsJson(updated, original.GetCompareIgnorePropertyNames()),
                    UserID = currentUser.UserID,
                    Utc = DateTime.UtcNow,
                    Points = reputationPointsForAction,
                    Comment = string.Empty
                });

                //-- Step 2 update the objModMeta Action Foreign Key & update the object
                if (modMetaUpdateAction != null) { modMetaUpdateAction(modMeta, newActionID); }
                objModMetaRepo.Update(modMeta);

                //-- Step 3 is update the moderators profile            
                if (modProfileUpdateAction != null)
                {
                    modProfileUpdateAction(CfPrincipal.ModDetails);
                }
                CfPrincipal.ModDetails.LastActivityUtc = DateTime.UtcNow;
                CfPrincipal.ModDetails.Reputation += reputationPointsForAction;
                modProfileRepo.Update(CfPrincipal.ModDetails);
            }

            return action;
        }

        /// <summary>
        /// Deducts the points that were originally assigned from an action (or actions) taken by a user
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public IQueryable<ModAction> ReverseActions(IQueryable<ModAction> actions)
        {
            var modUserIDs = actions.Select(a => a.UserID).Distinct().ToList();
            var modProfiles = modProfileRepo.GetAll().Where(p => modUserIDs.Contains(p.ID));
            foreach (var a in actions)
            {
                modProfiles.Where(p => p.ID == a.UserID).Single().Reputation -= a.Points;
                a.Points = 0;
            }
            modActionRepo.Update(actions);
            modProfileRepo.Update(modProfiles);
            return actions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ModProfile GetModProfile(Guid userID)
        {
            var modProfile = modProfileRepo.GetByID(userID);
            if (modProfile == default(ModProfile)) { modProfile = modProfileRepo.Create(ModProfile.InstansiateNewModProfile(userID)); }
            
            return modProfile;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<ObjectModMeta> GetModeratorsClaimedObjects(Guid userID)
        {
            return objModMetaRepo.GetModeratorsObjects(userID);
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<ModAction> GetLastHundredActions()
        {                       
            return modActionRepo.GetAll().OrderByDescending(a=>a.Utc).Take(100).ToList();
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<ModAction> GetModeratorsActions(Guid userID)
        {
            return modActionRepo.GetAll().Where(a=>a.UserID == userID).ToList();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ModAction GetModAction(Guid ID)
        {
            return modActionRepo.GetByID(ID);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ObjectModMeta GetObjectModeMeta(Guid ID)
        {
            return objModMetaRepo.GetByID(ID); 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="placeID"></param>
        /// <returns></returns>
        public IList<ModAction> GetModeratorActionsOnObject(Guid placeID)
        {
            return GetModeratorActionsOnObjects(new List<Guid>() { placeID });
        }

        public IList<ModAction> GetModeratorActionsOnObjects(List<Guid> placeIDs)
        {
            return modActionRepo.GetAll().Where(a => placeIDs.Contains(a.OnObjectID))
                .OrderByDescending(a=>a.Utc).Take(100).ToList();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public ObjectModMeta GetObjectModMetaOrSystemCreate(ISearchable place)
        {
            var modMeta = objModMetaRepo.GetByID(  new Guid(place.IDstring) );

            if (modMeta == default(ObjectModMeta)) 
            {
                modMeta = new ObjectModMeta(place, Stgs.SystemID);
                objModMetaRepo.Create(modMeta);
            }

            return modMeta;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ObjectModMeta ClaimObject(ISearchable obj)
        {
            ClaimObjectAuthorization(obj);

            var placeID = new Guid(obj.IDstring);
            var modsPlaces = objModMetaRepo.GetModeratorsObjects(currentUser.UserID);
            var alreadyHasPlace = modsPlaces.Where(p=>p.ID == placeID).Count() > 0;
            if (alreadyHasPlace) { return modsPlaces.Where(p => p.ID == placeID).Single(); }
            else
            {
                return objModMetaRepo.ClaimObject(obj, currentUser.UserID);
            }
        }

        /// <summary>
        /// User must be authenticated and have a moderator profile
        /// </summary>
        public void ClaimObjectAuthorization(ISearchable obj)
        {
            var modUserID = currentUser.UserID;

            var modProfile = modProfileRepo.GetByID(modUserID);

            if (modProfile == null) { throw new AccessViolationException("Claim Object: User does not have a moderator profile. User must first add content to get a moderator profile."); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void UnclaimObject(CfCacheIndexEntry obj)
        {
            UnclaimObjectAuthorization(obj);

            objModMetaRepo.UnclaimObject(obj, currentUser.UserID);
        }

        public void UnclaimObjectAuthorization(CfCacheIndexEntry obj)
        {
            var modUserID = currentUser.UserID;

            var modProfile = modProfileRepo.GetByID(modUserID);

            if (modProfile == null) { throw new AccessViolationException("UnclaimPlace: User does not have a moderator profile. User must first add content to get a moderator profile."); }
        }

        /// <summary>
        /// Throws access violation error for naughty users
        /// </summary>
        /// <param name="actionName"></param>
        public ObjectModMeta SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(ISearchable obj)
        {
            SetModDetailsOnPrincipal();
            
            //-- Stop a user doing damage if they've been down voted
            var negativeReputationThreshhold = -3 * Stgs.ModRepDownVote;
            if (currentUser.Reputation < negativeReputationThreshhold)
            {
                //-- Get calling method name
                var callingMethod = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                var action = callingMethod.Replace("Authorization", "");
                throw new AccessViolationException(action + ": You're getting BAD reputation on our system for submitting bad content. We've locked your account. If you believe this is unjust, email the moderator team.");
            };
            
            return GetObjectModMetaOrSystemCreate(obj);
        }

        public void SetModDetailsOnPrincipal()
        {
            if (CfPrincipal.ModDetails == null)
            {
                var modProfile = GetModProfile(currentUserID);
                CfPrincipal.AttachModProfile(modProfile);
            }
        }
    }
}
