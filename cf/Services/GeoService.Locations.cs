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
using System.IO;

namespace cf.Services
{
    public partial class GeoService
    {
        LocationRepository locRepo { get { if (_locRepo == null) { _locRepo = new LocationRepository(); } return _locRepo; } } LocationRepository _locRepo;
        LocationIndoorRepository locIndoorRepo { get { if (_locIndoorRepo == null) { _locIndoorRepo = new LocationIndoorRepository(); } return _locIndoorRepo; } } LocationIndoorRepository _locIndoorRepo;
        LocationOutdoorRepository locOutdoorRepo { get { if (_locOutdoorRepo == null) { _locOutdoorRepo = new LocationOutdoorRepository(); } return _locOutdoorRepo; } } LocationOutdoorRepository _locOutdoorRepo;

        //-- Location Stuff
        public Location GetLocationByID(Guid id) { return locRepo.GetByID(id); }
        //public IList<Location> GetLocationsAll() { return locRepo.GetAll().ToList(); }
        public List<Location> GetLocationsOfArea(Guid id) { return locRepo.GetLocationsOfArea(id); }

        public IList<Location> GetClosestLocationsOfLocation(Guid id) { return locRepo.GetClosestLocationsOfLocation(id).ToList(); }
        public IList<Location> GetClosestLocationsOfPoint(double latitude, double logitude) 
            { return locRepo.GetClosestLocationsOfLocation(latitude, logitude).ToList(); }

        public IList<Location> GetTopLocationsOfCountry(byte id) { return locRepo.GetTopLocationsOfCountry(id).ToList(); }     
    
        //-- LocationIndoor Stuff
        public LocationIndoor GetLocationIndoorByID(Guid id) { return locIndoorRepo.GetByID(id); }
        public LocationIndoor GetLocationIndoor(byte countryID, string nameUrlPart) { return locIndoorRepo.GetByCountryAndNameUrlPart(countryID, nameUrlPart); }
        public IList<LocationIndoor> GetLocationsIndoorAll() { return locIndoorRepo.GetAll().ToList(); }

        //public IQueryable<LocationSetter> GetLocationSetters(Guid id)
        //{
        //    return new LocationSetterRepository().GetAll().Where(ls => ls.LocationID == id);
        //}

        public IQueryable<LocationSection> GetLocationSections(Guid id)
        {
            return new LocationSectionRepository().GetAll().Where(ls => ls.LocationID == id);
        }

        //public Dictionary<Guid, string> GetLocationSettersDictionary(Guid id)
        //{
        //    var dic = GetLocationSetters(id).ToDictionary(ls => ls.ID, ls => ls.SetterInitials);
        //    dic.Add(Guid.Empty, "Unspecified");

        //    return dic;
        //}

                //-- Location Outdoor Stuff
        public LocationOutdoor GetLocationOutdoorByID(Guid id) { return locOutdoorRepo.GetByID(id); }
        public LocationOutdoor GetLocationOutdoor(byte countryID, string nameUrlPart) { return locOutdoorRepo.GetByCountryAndNameUrlPart(countryID, nameUrlPart); }
        public IList<LocationOutdoor> GetLocationsOutdoorAll() { return locOutdoorRepo.GetAll().ToList(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public LocationIndoor CreateLocationIndoor(LocationIndoor obj)
        {
            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            
            CreateLocationIndoorAuthorization(obj);
            
            obj.NameUrlPart = obj.Name.ToUrlFriendlyString();
            
            locIndoorRepo.Create(obj);
            var meta = objModMetaRepo.Create(new ObjectModMeta(obj, currentUser.UserID));

            //-- Refresh the cache
            AppLookups.AddIndexEntryToCache( obj.ToCacheIndexEntry() );
            
            ClaimObject(obj);

            //-- Save post for the feed
            //PostService.CreateLocationCreatedPost(obj);

            var action = SaveModActionAndUpdateModProfile(ModActionType.LocationIndoorAdd, null, obj, meta, 
                (m, actionID) => m.SetCreated(actionID), mp => mp.PlacesAdded++, "added {0} {1}", obj.Type, obj.Name);

            postSvc.CreateContentAddPost(action, obj.ID, user.PrivacyPostsDefaultIsPublic);
            //-- TODO: Calculate all areas that completely contain this area + Shoot off a notification to moderators

            return obj;
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public LocationOutdoor CreateLocationOutdoor(LocationOutdoor obj)
        {
            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            
            CreateLocationOutdoorAuthorization(obj);

            obj.NameUrlPart = obj.Name.ToUrlFriendlyString();

            locOutdoorRepo.Create(obj);
            var modPlaceDetail = objModMetaRepo.Create(new ObjectModMeta(obj, currentUser.UserID));

            //-- Update the cache
            AppLookups.AddIndexEntryToCache(obj.ToCacheIndexEntry());

            ClaimObject(obj);
                      
            //-- Save post for the feed
            //PostService.CreateLocationCreatedPost(obj);
            
            var action = SaveModActionAndUpdateModProfile(ModActionType.LocationOutdoorAdd, null, obj, modPlaceDetail,
                 (m, actionID) => m.SetCreated(actionID), mp => mp.PlacesAdded++, "added {0} {1}", obj.Type, obj.Name);

            postSvc.CreateContentAddPost(action, obj.ID, user.PrivacyPostsDefaultIsPublic);

            //-- Shoot off a notification to moderators

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public LocationOutdoor UpdateLocationOutdoor(LocationOutdoor original, LocationOutdoor updated)
        {
            var meta = UpdateLocationOutdoorAuthorization(original, updated);

            if (!original.Equals(updated))
            {
                locOutdoorRepo.Update(updated);
                
                //-- Refresh the cache
                AppLookups.UpdateIndexEntryInCache(updated.ToCacheIndexEntry());

                var action = SaveModActionAndUpdateModProfile(ModActionType.LocationOutdoorEdit, original, updated, meta,
                    (m, actionID) => m.SetDetailsChanged(actionID),
                    null, "edited {0} {1}", updated.Type, updated.Name);

                postSvc.UpdateContentAddPost(action);
            }
            
            return updated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public LocationIndoor UpdateLocationIndoor(LocationIndoor original, LocationIndoor updated)
        {
            var placeMeta = UpdateLocationIndoorAuthorization(original, updated);

            if (!original.Equals(updated))
            {
                locIndoorRepo.Update(updated);

                AppLookups.UpdateIndexEntryInCache(updated.ToCacheIndexEntry());

                var action = SaveModActionAndUpdateModProfile(ModActionType.LocationIndoorEdit, original, updated, placeMeta,
                    (m, actionID) => m.SetDetailsChanged(actionID), null, "edited {0} {1}", updated.Type, updated.Name);

                postSvc.UpdateContentAddPost(action);
            }

            return updated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteLocationIndoor(LocationIndoor obj)
        {
            var meta = DeleteLocationIndoorAuthorization(obj);

            locIndoorRepo.Delete(obj.ID);

            //-- Update the cache
            AppLookups.RemoveCacheIndexEntry(obj.ID);

            var modWhoAddedArea = modProfileRepo.GetByID(meta.CreatedByUserID);
            if (modWhoAddedArea.PlacesAdded > 0)
            {
                modWhoAddedArea.PlacesAdded--;
                modProfileRepo.Update(modWhoAddedArea);

                //-- Remove the points associated with this place
                var actionsWithPoints = modActionRepo.GetAll().Where(a => a.OnObjectID == meta.ID);
                ReverseActions(actionsWithPoints);
            }

            //-- update the principal details with the details we just updated (if they are the same person who deleted it)
            CfPrincipal.AttachModProfile(modProfileRepo.GetByID(currentUser.UserID));

            //-- Incase the name changed during the life of the object, we want to save the meta with the same name as the object was when it was deleted.            
            meta.Name = obj.VerboseDisplayName;

            var action = SaveModActionAndUpdateModProfile(ModActionType.LocationIndoorDelete, obj, null, meta,
                     (m, actionID) => m.SetDeleted(actionID), null, "deleted {0} {1}", obj.Type, obj.Name);

            postSvc.DeleteContentAddPost(action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteLocationOutdoor(LocationOutdoor obj)
        {
            var meta = DeleteLocationOutdoorAuthorization(obj);
            
            locOutdoorRepo.Delete(obj.ID);

            //-- Update the cache
            AppLookups.RemoveCacheIndexEntry(obj.ID);

            var modWhoAddedArea = modProfileRepo.GetByID(meta.CreatedByUserID);
            if (modWhoAddedArea.PlacesAdded > 0)
            {
                modWhoAddedArea.PlacesAdded--;
                modProfileRepo.Update(modWhoAddedArea);

                //-- Remove the points associated with this place
                var actionsWithPoints = modActionRepo.GetAll().Where(a => a.OnObjectID == meta.ID);
                ReverseActions(actionsWithPoints);
            }

            //-- update the principal details with the details we just updated (if they are the same person who deleted it)
            CfPrincipal.AttachModProfile(modProfileRepo.GetByID(currentUser.UserID));

            meta.Name = obj.VerboseDisplayName;
                        
            var action = SaveModActionAndUpdateModProfile(ModActionType.LocationOutdoorDelete, obj, null, meta,
                     (m, actionID) => m.SetDeleted(actionID), null, "deleted {0} {1}", obj.Type, obj.Name);

            postSvc.DeleteContentAddPost(action);
        }

        /// <summary>
        /// Save the logo for an indoor climbing gym
        /// </summary>
        public string SaveLocationIndoorLogo(LocationIndoor obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SaveLocationIndoorLogoAuthorization(obj, stream, cropOpts);
            var original = obj.GetSimpleTypeClone();

            var newImgUrl = SaveAvatar240Thumb(stream, obj.Logo, obj.NameUrlPart, ImageManager.LogoIndoorPath,
                fileName => { obj.Logo = fileName; locIndoorRepo.Update(obj); }, cropOpts);
            
            //-- No change occurs to the mods profile because the image has to be verified by other users
            SaveModActionAndUpdateModProfile(ModActionType.LocationIndoorSetLogo, original, obj, meta, 
                    null, // don't really have an action FK on the objModMeta just for the logo, so don't update it
                    null, "set indoor logo {0}", obj.Name);

            //-- If we don't yet have an avatar/climbing image, let's use the logo for the avatar/map scroll over image
            //if (String.IsNullOrWhiteSpace(obj.ImageFileMap)) { SaveLocationIndoorAvatar(obj,  stream, cropOpts); }

            return newImgUrl;
        }

        /// <summary>
        /// Saves the climbing image (used as the avatar/map image)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="cropOpts"></param>
        /// <returns></returns>
        //public string SaveLocationIndoorAvatar(LocationIndoor obj, Stream stream, ImageCropOpts cropOpts)
        //{
        //    throw new NotImplementedException("SaveLocationIndoorAvatar");
        //    //var meta = SaveLocationIndoorAvatarAuthorization(obj, stream, cropOpts);
        //    //var original = obj.GetSimpleTypeClone();

        //    ////-- Also save the image to the location's media real
        //    //var media = new MediaService().CreateImageMedia(new Media() { Title = "Climbing pic of " + obj.Name, ContentType = "image/jpg" }, obj.ID, stream);
            
        //    //var newImgUrl = SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, ImageManager.ClimbingIndoorPath,
        //    //    fileName => { obj.Avatar = fileName; locIndoorRepo.Update(obj); }, cropOpts, media.Content);

        //    ////-- No change occurs to the mod's profile because the image has to be verified by other users
        //    //SaveModActionAndUpdateModProfile(ModActionType.LocationIndoorSetAvatar, original, obj, meta,
        //    //    (m, actionID) => m.SetAvatarChanged(actionID), null, "set map img {0}", obj.Name);

        //    //return newImgUrl;
        //}

        public Media SaveLocationAvatar(Location obj, Stream stream, ImageCropOpts cropOpts)
        {
            var locTypePath = ImageManager.ClimbingIndoorPath;
            var actionType = ModActionType.LocationIndoorSetAvatar;
            
            if (obj.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { 
                locTypePath = ImageManager.ClimbingOutdoorPath;
                actionType = ModActionType.LocationOutdoorSetAvatar;
            } 

            var meta = SaveLocationAvatarAuthorization(obj, stream, cropOpts);
            var original = obj.GetSimpleTypeClone();

            //-- Also save the image to the location's media real
            var media = new MediaService().CreateImageMedia(new Media() { FeedVisible = true, Title = "Climbing pic of " + obj.Name, ContentType = "image/jpg" }, obj.ID, stream);
                        
            SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, locTypePath,
                fileName => { obj.Avatar = fileName; locRepo.UpdateLocationAvatar(obj.ID, fileName); }, cropOpts, media.Content);

            //-- No change occurs to the mod's profile because the image has to be verified by other users
            SaveModActionAndUpdateModProfile(actionType, original, obj, meta,
                (m, actionID) => m.SetAvatarChanged(actionID), null, "set map img {0}", obj.Name);

            return media;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="cropOpts"></param>
        /// <returns></returns>
        //public string SaveLocationOutdoorAvatar(LocationOutdoor obj, Stream stream, ImageCropOpts cropOpts)
        //{
        //    var meta = SaveLocationOutdoorAvatarAuthorization(obj, stream, cropOpts);
        //    var original = obj.GetSimpleTypeClone();

        //    //-- Also save the image to the location's media real
        //    var media = new MediaService().CreateImageMedia(new Media() { Title = "Climbing pic of " + obj.Name, ContentType = "image/jpg" }, obj.ID, stream);
            
        //    var newImgUrl = SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, ImageManager.ClimbingOutdoorPath,
        //        fileName => { obj.Avatar = fileName; locOutdoorRepo.Update(obj); }, cropOpts, media.Content);
            
        //    //-- No change occurs to the mods profile because the image has to be verified by other users
        //    SaveModActionAndUpdateModProfile(ModActionType.LocationOutdoorSetAvatar, original, obj, meta,
        //        (m, actionID) => m.SetAvatarChanged(actionID), null, "set map img {0}", obj.Name);

        //    return newImgUrl;
        //}
    }
}
