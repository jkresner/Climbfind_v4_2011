using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Entities.Interfaces;
using cf.Content.Images;
using cf.Caching;
using cf.Entities.Enum;
using cf.Identity;
using NetFrameworkExtensions;
using System.IO;

namespace cf.Services
{
    public partial class GeoService
    {
        AreaRepository areaRepo { get { if (_areaRepo == null) { _areaRepo = new AreaRepository(); } return _areaRepo; } } AreaRepository _areaRepo;
        CountryRepository countryRepo { get { if (_countryRepo == null) { _countryRepo = new CountryRepository(); } return _countryRepo; } } CountryRepository _countryRepo;

        //-- Country Stuff
        public Country GetCountryByID(byte id) { return countryRepo.GetByID(id); }
        public Country UpdateCountry(Country obj) 
        {
            UpdateCountryAuthorization(obj);
            return countryRepo.Update(obj); 
        }
        
        //-- Area Stuff
        public Area GetAreaByID(Guid id) { return areaRepo.GetByID(id); }        
        public Area GetArea(byte countryID, string nameUrlPart) { return areaRepo.GetByCountryAndNamePartUrl(countryID, nameUrlPart); }
        public IList<Area> GetAreasAll() 
        {
            GetAreasAllAuthorization();
            return areaRepo.GetAll().ToList(); 
        }       
        public IList<Area> GetAreasOfCountry(byte id) { return areaRepo.GetAreasOfCountry(id); }
        public IList<Area> GetCitiesAndMajorClimbingAreasOfCountry(byte id) { return areaRepo.GetCitiesAndMajorClimbingAreasOfCountry(id); }
        public IList<Area> GetIntersectingAreas(Area area) { return areaRepo.GetIntersectingAreas(area.ID); }
        public IList<Area> GetIntersectingAreasWithGeoInflate(Area area) { return areaRepo.GetIntersectingAreasWithGeoInflate(area.ID); }
        public IList<Area> GetIntersectingAreasOfPoint(double latitude, double logitude) { return areaRepo.GetIntersectingAreasOfPoint(latitude, logitude).ToList(); }
        public IList<Area> GetIntersectingAreas(ILocation location) { return areaRepo.GetIntersectingAreasOfLocation(location.ID); }
        public IList<Area> GetRelatedAreas(Area area) { return areaRepo.GetRelatedAreas(area.ID).Where(p => p.ID != area.ID).ToList(); }
        public IList<Area> GetRelatedAreas(ILocation location) { return areaRepo.GetRelatedAreasOfLocation(location.ID); }

        /// <summary>
        /// Logic for Partner Calls notifications system
        /// </summary>
        /// <param name="places"></param>
        /// <returns></returns>
        public List<cf.Dtos.CfCacheIndexEntry> GetGeoDeduciblePlaces(cf.Dtos.CfCacheIndexEntry place) 
        {
            var places = new List<cf.Dtos.CfCacheIndexEntry>();
            if (place.Type == CfType.Country || place.Type == CfType.Province) { throw new Exception("GetGeoDeduciblePlaces is not available for countries and provinces: "+place.ID); }
            else if (place.Type.ToPlaceCateogry() == PlaceCategory.Area)
            {
                foreach (var loc in GetLocationsOfArea(place.ID)) { places.Add(((Place<Guid>)loc).ToCacheIndexEntry()); }
                foreach (var area in areaRepo.GetIntersectingAreas(place.ID)) 
                { 
                    if (area.Type != CfType.Province && !area.DisallowPartnerCalls) { places.Add(area.ToCacheIndexEntry()); } 
                }
            }
            else
            {
                foreach (var area in areaRepo.GetIntersectingAreasOfLocation(place.ID))
                {
                    if (area.Type != CfType.Province && !area.DisallowPartnerCalls) { places.Add(area.ToCacheIndexEntry()); }
                }
            }

            places.Add(place); //-- Make sure we include our original place
            return places; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Area CreateArea(Area obj)
        {
            CreateAreaAuthorization(obj);

            var user = CfPerfCache.GetClimber(CfIdentity.UserID);

            obj.NameUrlPart = obj.Name.ToUrlFriendlyString();

            //-- Incase we are creating a freeform area place with no suggested center (from bing / google)
            if (obj.Latitude == 0 & obj.Longitude == 0)
            {
                obj.Latitude = obj.Geo.EnvelopeCenter().Lat.Value;
                obj.Longitude = obj.Geo.EnvelopeCenter().Long.Value;
            }

            var area = areaRepo.Create(obj);
            var modPlaceDetail = objModMetaRepo.Create( new ObjectModMeta(area, currentUser.UserID) );

            //-- Update the cache
            AppLookups.AddIndexEntryToCache( area.ToCacheIndexEntry() );

            ClaimObject(obj);

            //-- Save post for the feed
            //PostService.CreateAreaCreatedPost(currentUser.UserID, area);

            var action = SaveModActionAndUpdateModProfile(ModActionType.AreaAdd, null, obj, modPlaceDetail,
                 (m, actionID) => m.SetCreated(actionID), mp => mp.PlacesAdded++, "added {0} {1}", area.Type, area.Name);

            postSvc.CreateContentAddPost(action, obj.ID, user.PrivacyPostsDefaultIsPublic);

            //-- Shoot off a notification to moderators
            //-- Calculate all areas that completely contain this area
            // var moderatorsToNotify = new List<ModProfile>();
            // var areasContainingThisArea = ???
            // foreach (var area in areasContainingThisArea) {
            //  var modsWatchingArea = modProfileRepo.GetPlacesModerators(area.ID); 
            // moderatorsToNotify.Add(modsWatchingArea);
            //}
            // foreach (var mod in moderatorsToNotify) { NotifyModeratorOfAreaCreated(mod); }

            return area;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public Area UpdateArea(Area original, Area updated) 
        {
            var meta = UpdateAreaAuthorization(original, updated);
           
            if (!original.Equals(updated))
            {
                areaRepo.Update(updated);

                //-- Refresh the cache
                AppLookups.UpdateIndexEntryInCache(updated.ToCacheIndexEntry());
        
                //-- Send off notifications

                var action = SaveModActionAndUpdateModProfile(ModActionType.AreaEdit, original, updated, meta,
                    (m, actionID) => m.SetDetailsChanged(actionID),
                    null, "edited {0} {1}", updated.Type, updated.Name);

                postSvc.UpdateContentAddPost(action);
            }

            return updated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteArea(Area obj) 
        {
            var meta = DeleteAreaAuthorization(obj);

            //-- TODO: Shoot off notifications

            areaRepo.Delete(obj); 
            
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

            var action = SaveModActionAndUpdateModProfile(ModActionType.AreaDelete, obj, null, meta,
                      (m, actionID) => m.SetDeleted(actionID), null, "deleted {0} {1}", obj.Type, obj.Name);

            postSvc.DeleteContentAddPost(action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="originalImageUrl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public string SaveAreaAvatar(Area obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SaveAreaAvatarAuthorization(obj, stream, cropOpts);
            var original = obj.GetCloneWithGeo();

            //-- Also save the image to the location's media real
            var media = new MediaService().CreateImageMedia(new Media() { FeedVisible = true, Title = "Climbing pic of " + obj.Name, ContentType = "image/jpg" }, obj.ID, stream);

            var newImgUrl = SaveAvatar240Thumb(stream, obj.Avatar, obj.NameUrlPart, ImageManager.ClimbinAreaPath,
                fileName => { obj.Avatar = fileName; areaRepo.Update(obj); }, cropOpts, media.Content);

            //-- No change occurs to the mod's profile because the image has to be verified by other users
            SaveModActionAndUpdateModProfile(ModActionType.AreaSetAvatar, original, obj, meta,
                (m, actionID) => m.SetAvatarChanged(actionID), null, "set map img {0}", obj.Name);

            return newImgUrl;
        }
    }
}