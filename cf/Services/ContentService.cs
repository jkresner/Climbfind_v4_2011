using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using System.IO;
using cf.Content.Images;
using cf.Identity;
using cf.Caching;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using cf.DataAccess.Interfaces;

namespace cf.Services
{
    /// <summary>
    /// Responsible for manipulating content that does not have associated logical functionality
    /// </summary>
    /// <remarks>
    /// If content is related to logical functionality the code that performs the content saving etc happens in the service that is
    /// related to the domain object. E.g. Setting an image for a location happens in the geo server
    /// </remarks>
    public partial class ContentService : AbstractCfService
    {
        ImageManager imgManager { get { if (_imagManager == null) { _imagManager = new ImageManager(); } return _imagManager; } } ImageManager _imagManager;
        OpinionRepository rateRepo { get { if (_rateRepo == null) { _rateRepo = new OpinionRepository(); } return _rateRepo; } } OpinionRepository _rateRepo;
        
        /// <summary>
        /// Basic constructor
        /// </summary>
        public ContentService() { } 

        /// <summary>
        /// Save an image to the temporary store
        /// </summary>
        /// <param name="stream">Steam that is the raw bytes of the image</param>
        /// <returns>Url that can be used to save the new</returns>
        /// <remarks>
        /// We use this to save uploaded images before we give the user ther 
        /// </remarks>
        public string SaveTempImage(Stream stream)
        {
            string fileName = string.Format("tmp-{0:MMddhhmmss}.jpg", DateTime.Now);

            imgManager.SaveTempImage(stream, fileName);

            return string.Format(@"{0}/temp/{1}", Stgs.ImgsRt, fileName);   
        }

        /// <summary>
        /// Deletes an image that was save to the temporary image store
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void DeleteTempImage(string url)
        {
            if (url.StartsWith(@"http://images.climbfind.com/temp/tmp-"))
            {
                var fileName = Path.GetFileName(url);
                var filePath = url.Replace(fileName, "").Replace(@"http://images.climbfind.com", "");

                imgManager.DeleteImage(filePath, fileName);    
            }
        }

        /// <summary>
        /// Opinions stuff
        /// </summary>
        public Opinion GetOpinion(Guid objectID, Guid userID) { return GetOpinionsOnObject(objectID).Where(r => r.UserID == userID).SingleOrDefault(); }
        public Opinion GetOpinionByID(Guid id) { return rateRepo.GetByID(id); }
        public IQueryable<Opinion> GetOpinionsOnObject(Guid id) { return rateRepo.GetAll().Where(r=>r.ObjectID==id); }
        public IQueryable<Opinion> GetLatestOpinionsOnObject(Guid id, int count) { return rateRepo.GetAll().Where(r => r.ObjectID == id).OrderByDescending(r => r.Utc).Take(count); }
        public IQueryable<Opinion> GetUsersOpinions(Guid id) { return rateRepo.GetAll().Where(r => r.UserID == id); }
        public IQueryable<Opinion> GetUsersLatestOpinions(Guid id, int count) { return rateRepo.GetAll().Where(r=>r.UserID == id).OrderByDescending(r => r.Utc).Take(count); }
        public Opinion GetUserOpinionOnPlace(Guid userID, Guid placeID) { return rateRepo.GetAll().Where(r => r.UserID == userID && r.ObjectID == placeID).SingleOrDefault(); }     
        
        public IQueryable<Opinion> GetLatestOpinions(int count) { return rateRepo.GetAll().OrderByDescending(r => r.Utc).Take(count); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Opinion CreateOpinion(Opinion obj, Guid placeID)
        {
            var user = CfPerfCache.GetClimber(CfIdentity.UserID);
            //-- TODO, got to think of the real world replications of compiling opinions over time
            //-- and letting this behavior continue / also peoples opinions do change overtime?
            
            //-- The behavior here is to delete old opinions and just overwrite them with the new one
            var existingObjectOpinion = rateRepo.GetAll().Where(r => r.ObjectID == obj.ObjectID && r.UserID == CfIdentity.UserID).SingleOrDefault();
            var existingOpinion = (existingObjectOpinion != default(Opinion));
            if (existingOpinion)
            {
                rateRepo.Delete(existingObjectOpinion.ID);
            }

            obj.ID = Guid.NewGuid();
            obj.UserID = CfIdentity.UserID;
            obj.Utc = DateTime.UtcNow;
            
            rateRepo.Create(obj);
            var objectsRatins = rateRepo.GetAll().Where( r=>r.ObjectID == obj.ObjectID).ToList();

            UpdateRatedObject(obj.ObjectID, objectsRatins);

            //-- Guid.Empty is a sign that the opinion is on a climb - which we don't want to create a post for (as check-ins will go crazy)
            //-- flooding the feed with climb opinions.
            if (placeID != Guid.Empty) 
            {
                if (!existingOpinion) { postSvc.CreateOpinionPost(obj, placeID, user.PrivacyPostsDefaultIsPublic); }
                else { postSvc.UpdateOpinionPost(obj); }
            }

            return obj;
        }

        private void UpdateRatedObject(Guid id, List<Opinion> allOpinions)
        {
            var cacheEntry = AppLookups.GetCacheIndexEntry(id);
            if (cacheEntry.Type == CfType.ClimbOutdoor || cacheEntry.Type == CfType.ClimbIndoor) { 
                UpdateOpinion<ClimbRepository, Climb>(id, allOpinions); }
            else if (cacheEntry.Type == CfType.ClimbingArea ||
                cacheEntry.Type == CfType.Province ||
                cacheEntry.Type == CfType.City) { UpdateOpinion<AreaRepository, Area>(id, allOpinions); }
            else if (cacheEntry.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing) { UpdateOpinion<LocationIndoorRepository, LocationIndoor>(id, allOpinions); }
            else if (cacheEntry.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing) { UpdateOpinion<LocationOutdoorRepository, LocationOutdoor>(id, allOpinions); }
            else
            {
                throw new NotImplementedException("UpdateRatedObject does not yet support type : " + cacheEntry.Type.ToString());
            }

        }

        private void UpdateOpinion<TRepo, TEntity>(Guid id, List<Opinion> allObjectsOpinions) 
            where TRepo : IKeyEntityWriter<TEntity, Guid>, new()
            where TEntity : IKeyObject<Guid>, IRatable, new() 
        {
            var repo = new TRepo();
            var obj = repo.GetByID(id);
            obj.RatingCount = allObjectsOpinions.Count();
            if (obj.RatingCount == 0) { obj.Rating = null; }
            else { obj.Rating = allObjectsOpinions.Average(r => r.Rating); }
            repo.Update(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void DeleteOpinion(Opinion obj)
        {
            if (obj.UserID != CfIdentity.UserID & !CfPrincipal.IsGod())
            {
                throw new AccessViolationException("Cannot delete Opinion that was not added by you");
            }

            rateRepo.Delete(obj.ID);

            var objectsRatins = rateRepo.GetAll().Where(r => r.ObjectID == obj.ObjectID).ToList();
            UpdateRatedObject(obj.ObjectID, objectsRatins);

            postSvc.DeleteOpinionPost(obj);
        }
    }
}
