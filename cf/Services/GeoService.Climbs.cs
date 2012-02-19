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
using System.IO;
using cf.Identity;
using cf.Dtos;
using NetFrameworkExtensions.Net;

namespace cf.Services
{
    public partial class GeoService
    {
        ClimbRepository climbRepo { get { if (_climbRepo == null) { _climbRepo = new ClimbRepository(); } return _climbRepo; } } ClimbRepository _climbRepo;
 
        public Climb GetClimbByID(Guid id) { return climbRepo.GetByID(id); }
        public ClimbIndoor GetIndoorClimbByID(Guid id) { return climbRepo.GetIndoorClimbByID(id); }
        public ClimbOutdoor GetOutdoorClimbByID(Guid id) { return climbRepo.GetOutdoorClimbByID(id); }
        public IQueryable<Climb> GetClimbsOfLocation(Guid id) { return climbRepo.GetAll().Where(c => c.LocationID == id); }

        public IQueryable<ClimbIndoor> GetClimbsIndoorOfLocation(LocationIndoor loc) { return climbRepo.GetIndoorClimbsOfLocation(loc.ID); }
        public IQueryable<ClimbIndoor> GetCurrentClimbsOfIndoorLocation(LocationIndoor loc)
        {
            return climbRepo.GetIndoorClimbsOfLocation(loc.ID).Where(c =>
                    (!c.SetDate.HasValue || c.SetDate < DateTime.Now)
                    && (!c.DiscontinuedDate.HasValue || (c.DiscontinuedDate.Value > DateTime.Now)));
        }

        public IQueryable<ClimbOutdoor> GetClimbsOutdoorOfLocation(LocationOutdoor loc) { return climbRepo.GetOutdoorClimbsOfLocation(loc.ID); }

        public IList<Climb> GetCurrentClimbsOfLocation(Guid id) { return GetClimbsOfLocationForLogging(id, DateTime.Now); }

        public IList<Climb> GetClimbsOfLocationForLogging(Guid id, DateTime checkInDateTime) 
        {
            var loc = AppLookups.GetCacheIndexEntry(id);
            if (loc.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing)
            {
                return climbRepo.GetIndoorClimbsOfLocation(id).Where( c =>
                    (!c.SetDate.HasValue || c.SetDate < checkInDateTime)
                    && (!c.DiscontinuedDate.HasValue || (c.DiscontinuedDate.Value > checkInDateTime))).ToClimbList(); 
            }
            else
            {
                return climbRepo.GetAll().Where(c => c.LocationID == id).ToList(); 
            }
        }

        public IList<Climb> GetClimbsAll()
        {
            throw new Exception("Should never be pulling back all climbs....");
            //return climbRepo.GetAll().ToList();
        }

        public IList<Climb> GetTopClimbOfCountry(byte id, int count) { return 
            climbRepo.GetAll().Where(c => c.CountryID == id).OrderByDescending(c=>c.Rating).Take(count).ToList(); }

        public IList<Climb> GetTopClimbsOfArea(Guid id, int count)
        {
            return climbRepo.GetTopClimbsOfArea(id, count);
        }

        public LocationSection CreateLocationIndoorSection(LocationSection obj)
        {
            obj.ID = Guid.NewGuid();
            obj.NameUrlPart = obj.Name.ToUrlFriendlyString();

            return new LocationSectionRepository().Create(obj);
        }

        public void AddSetterToLocationIndoor(LocationIndoor loc, Setter setter)
        {
            locIndoorRepo.AddSetter(loc.ID, setter.ID);
        }

        public void RemoveSetterFromLocationIndoor(LocationIndoor loc, Setter setter)
        {
            locIndoorRepo.RemoveSetter(loc.ID, setter.ID);
        }

        public Setter CreateSetter(Setter obj)
        {
            if (obj.Bio == null) { obj.Bio = string.Empty; }
            return new SetterRepository().Create(obj);
        }

        public Setter GetSetterByID(Guid id)
        {
            return new SetterRepository().GetByID(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public ClimbIndoor CreateClimbIndoor(ClimbIndoor obj, List<int> categories)
        {
            //-- The tricky part about this, it that using the Action/Lambda we call create with the ClimbIndoor type argument
            CreateClimb(obj, categories, CfType.ClimbIndoor, () => climbRepo.Create(obj));
            
            //-- Note we've removed adding a content post / mod action for Climb indoor because it doesn't make sense
            //-- to give the setters so much traffic / limelight for their daily jobs

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Called by the parent methods CreateClimbIndoor & CreateClimbOutdoor</remarks>
        private ObjectModMeta CreateClimb<T>(T obj, List<int> categories, CfType climbType, Action saveClimbToDB) where T : Climb
        {
            CreateClimbAuthorization(obj, categories);

            obj.ID = Guid.NewGuid();
            obj.TypeID = (byte)climbType;
            obj.NameUrlPart = obj.Name.ToUrlFriendlyString();
            if (obj.NameShort == null) { obj.NameShort = string.Empty; }
            if (obj.SetterID == Guid.Empty) { obj.SetterID = null; }
            obj.GradeCfNormalize = GradeConvert(obj.GradeLocal);

            //-- Add climb category/tags
            if (categories.Count > 0) { foreach (var c in categories) { obj.ClimbTags.Add(new ClimbTag(obj.ID, c)); } }

            saveClimbToDB();
            
            var meta = objModMetaRepo.Create(new ObjectModMeta(obj, currentUser.UserID));

            AppLookups.AddIndexEntryToCache(obj.ToCacheIndexEntry());

            return meta;
        }

        public ClimbOutdoor CreateClimbOutdoor(ClimbOutdoor obj, List<int> categories)
        {
            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            
            //-- The tricky part about this, it that using the Action/Lambda we call create with the ClimbOutdoor type argument
            var meta = CreateClimb(obj, categories, CfType.ClimbOutdoor, () => climbRepo.Create(obj));

            ClaimObject(obj); // note we only claim outdoor climbs, not indoor....

            var action = SaveModActionAndUpdateModProfile(ModActionType.ClimbAdd, null, (Climb)obj, meta,
                (m, actionID) => m.SetCreated(actionID), mp => mp.ClimbsAdded++, "added climb {0}", obj.Name);

            postSvc.CreateContentAddPost(action, obj.LocationID, user.PrivacyPostsDefaultIsPublic);

            return obj;
        }

        private T UpdateClimb<T>(T original, T updated, List<int> categories, Func<T, T> updateClimbToDb) where T : Climb, new()
        {
            var meta = UpdateClimbAuthorization(original, updated, categories);

            //-- 
            if (!original.Equals(updated))
            {
                updated.GradeCfNormalize = GradeConvert(updated.GradeLocal);

                if (string.IsNullOrWhiteSpace(updated.NameShort)) { updated.NameShort = string.Empty; }
                if (updated.SetterID == Guid.Empty) { updated.SetterID = null; }

                updateClimbToDb(updated);

                AppLookups.UpdateIndexEntryInCache(updated.ToCacheIndexEntry());

                //-- We don't want to save mod trail if it's indoor (the setters working, kinda boring!)
                if (updated.Type == CfType.ClimbOutdoor)
                {
                    var action = SaveModActionAndUpdateModProfile(ModActionType.ClimbEdit, original, updated, meta,
                        (m, actionID) => m.SetDetailsChanged(actionID), null, "edited climb {0}", updated.Name);

                    postSvc.UpdateContentAddPost(action);
                }
            }

            //-- Can't be bothered logging/showing if only the categories were changed
            if (categoryListsDoNotMatch(original.ClimbTags, categories))
            {
                climbRepo.UpdateCategories(updated, categories);
            }

            return updated;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="climb"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        /// <remarks>Used for mobile updating of climb (while setter is taking a picture)</remarks>
        public Climb UpdateClimbGrade(Climb climb, string grade)
        {
            climb.GradeLocal = grade;
            climb.GradeCfNormalize = GradeConvert(climb.GradeLocal);
            return climbRepo.Update(climb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public ClimbIndoor UpdateClimbIndoor(ClimbIndoor original, ClimbIndoor updated, List<int> categories)
        {
            //-- Don't want to show staff the nameUrlPart field
            updated.NameUrlPart = updated.Name.ToUrlFriendlyString();
            return UpdateClimb(original, updated, categories, c => climbRepo.UpdateIndoor(c));
        }

        public ClimbOutdoor UpdateClimbOutdoor(ClimbOutdoor original, ClimbOutdoor updated, List<int> categories)
        {
            return UpdateClimb(original, updated, categories, c => climbRepo.UpdateOutdoor(c));
        }

        private bool categoryListsDoNotMatch(IEnumerable<ClimbTag> oldCategories, List<int> newCategories)
        {
            var orginalCategories = oldCategories.Select(c => c.Category).ToList();
            if (orginalCategories.Count != newCategories.Count) { return true; }
            foreach (var c in orginalCategories) { if (!newCategories.Contains(c)) { return true; } }
            return false;
        }

        /// <summary>
        /// Save the image used to represent the climb in thumbnails and on the main page. Also saves the image to the climbs media stream
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="cropOptions"></param>
        /// <returns></returns>
        public Media SaveClimbAvatar(Climb obj, Stream stream, ImageCropOpts cropOptions)
        {
            ObjectModMeta meta = null;
            
            //-- We don't need to set ModProfile on thread, or retrieve object mod-meta for indoor climbs
            if (obj.Type == CfType.ClimbOutdoor) { meta = SaveClimbAvatarAuthorization(obj, stream, cropOptions); }
            
            var original = obj.GetSimpleTypeClimbClone();

            //-- Also save the image to the climbs's media real
            //-- If the climb is indoor we don't save indoor climb avatar as 
            var media = new MediaService().CreateImageMedia(new Media() { FeedVisible = (obj.Type != CfType.ClimbIndoor), 
                Title = "Climbing pic of " + obj.Name, ContentType = "image/jpg" }, obj.ID, stream);

            SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, ImageManager.ClimbPath,
                fileName => { obj.Avatar = fileName; climbRepo.Update(obj); }, cropOptions, media.Content);
            
            //-- We don't want to save mod trail if it's indoor (the setters working, kinda boring!)
            if (obj.Type == CfType.ClimbOutdoor)
            {
                //-- No change occurs to the mods profile because the image has to be verified by other users
                SaveModActionAndUpdateModProfile(ModActionType.ClimbSetAvatar, original, obj, meta,
                    (m, actionID) => m.SetAvatarChanged(actionID), null, "set climb img {0}", obj.Name);
            }

            return media;
        }


        public string SaveMediaAsClimbAvatar(Climb obj, Media media, ImageCropOpts cropOptions)
        {
            if (obj == null || media == null || media.Type != MediaType.Image) { throw new ArgumentException("SaveMediaAsClimbAvatar invalid arguments"); }
            if (media.ObjectMedias.Where(om => om.OnOjectID == obj.ID).Count() == 0) { throw new ArgumentException("Media does not belong to climb"); }
                   
            var mediaImageUrl = string.Format("{0}/media/{1}", Stgs.ImgsRt, media.Content);
            string newImgUrl = string.Empty;
            using (Stream stream = new ImageDownloader().DownloadImageAsStream(mediaImageUrl))
            {
                var meta = SaveClimbAvatarAuthorization(obj, stream, cropOptions);
                Climb original = obj.GetSimpleTypeClimbClone();
                        
                newImgUrl = SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, ImageManager.ClimbPath,
                    fileName => { obj.Avatar = fileName; climbRepo.Update(obj); }, cropOptions, Path.GetFileName(media.ThumbUrl()));

                //-- No change occurs to the mods profile because the image has to be verified by other users
                SaveModActionAndUpdateModProfile(ModActionType.ClimbSetAvatar, original, obj, meta,
                    (m, actionID) => m.SetAvatarChanged(actionID), null, "set climb img {0}", obj.Name);
            }

            return newImgUrl;
        } 

        /// <summary>
        /// Indoor is more simple than outdoor because it's not maintained in the moderator system & we don't generate posts
        /// for creation of indoor climbs
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteClimbIndoor(ClimbIndoor obj)
        {
            DeleteClimb(obj);
        }

        /// <summary>
        /// Here we keep track of all the moderator happenings & associated post content
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteClimbOutdoor(ClimbOutdoor obj)
        {
            var meta = DeleteClimbOutdoorAuthorization(obj);
            
            DeleteClimb(obj);
            
            var modWhoAdded = modProfileRepo.GetByID(meta.CreatedByUserID);
            if (modWhoAdded.ClimbsAdded > 0)
            {
                modWhoAdded.ClimbsAdded--;
                modProfileRepo.Update(modWhoAdded);

                //-- Remove the points associated with this place
                var actionsWithPoints = modActionRepo.GetAll().Where(a => a.OnObjectID == meta.ID);
                ReverseActions(actionsWithPoints);
            }

            //-- update the principal details with the details we just updated (if they are the same person who deleted it)
            CfPrincipal.AttachModProfile(modProfileRepo.GetByID(currentUser.UserID));
            meta.Name = obj.Name;

            var action = SaveModActionAndUpdateModProfile(ModActionType.ClimbDelete, obj, null, meta,
                     (m, actionID) => m.SetDeleted(actionID), null, "deleted climb {0}", obj.Name);

            postSvc.DeleteContentAddPost(action);
            //-- TODO: Shoot off notifications to claimed users
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Called by the parent methods CreateClimbIndoor & CreateClimbOutdoor</remarks>
        private void DeleteClimb(Climb obj)
        {            
            climbRepo.Delete(obj.ID);

            //-- Remove from the cache
            AppLookups.RemoveCacheIndexEntry(obj.ID);
        }

        private byte GradeConvert(string grade)
        {
            byte rank;

            try { rank = CfEnumExtensions.GetGradeRank(grade); }
            catch { throw new Exception("Grade converter not implemented for grade: " + grade); }

            return rank;
        }
    }
}
