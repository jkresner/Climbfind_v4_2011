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
using cf.Content.Feed;
using cf.Mail;

namespace cf.Services
{
    /// <summary>
    /// Responsible for saving, updating, deleting and retrieving posts for the feed
    /// </summary>
    public partial class PostService : AbstractCfService
    {
        public const int ResultsPostCount = 20;
        PostRepository postRepo { get { if (_postRepo == null) { _postRepo = new PostRepository(); } return _postRepo; } } PostRepository _postRepo;
        public PostService() { }

        /// <summary>
        /// Basic Data Access
        /// </summary>
        public Post GetPostByID(Guid id) { return postRepo.GetByID(id); }
        public Post UpdatePost(Post obj) { return postRepo.Update(obj); }
        public void DeletePost(Post obj)
        {
            var currentUserID = CfIdentity.UserID;
            var userHasRightsToDeletePost = (currentUserID == obj.UserID) || CfPrincipal.IsGod();
            if (!userHasRightsToDeletePost) { throw new AccessViolationException("Delete Post: Cannot delete this post, it does not belong to the current user."); }

            postRepo.Delete(obj.ID);
        }

        public PostRendered GetPostRenderedByID(Guid id, ClientAppType clientType)
        {
            var post = postRepo.GetPostsInclude().Where(p=>p.ID == id).SingleOrDefault(); 
            var postsRendered = ContentRenderer.BindPostsContent(new List<Post>() {post }, clientType);
            return postsRendered.First();
        }

        /// <summary>
        /// Select feed posts relevant to the users preferences PLUS their feed places and return as a FeedResultSet
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public FeedResultSet GetUsersFeed(Guid userID, PostType postType, ClientAppType clientType)
        {
            var result = new FeedResultSet();

            //-- These are the preferences WITHOUT dedec places (don't want deduc in the filters down the left)
            var userPlacePreferences = (from c in new ProfileRepository().GetFeedPlacePreferences(userID)
                    where AppLookups.GetCacheIndexEntry(c) != null
                    select AppLookups.GetCacheIndexEntry(c)).ToList();

            result.Places = userPlacePreferences;

            //-- For the actual feed posts we use deduc
            var feedPlaces = GetGeoDeducPlaces(userPlacePreferences.Select(p => p.ID));
            result.Posts = GetPostsForPlaces(feedPlaces, postType, clientType);
            
            return result;
        }

        /// <summary>
        /// Select feed posts relevant to the users current geo-context
        /// </summary>
        public List<PostRendered> GetGeoFeed(double lat, double lon, PostType postType, ClientAppType clientType)
        {
            var intersectingAreas = new AreaRepository().GetIntersectingAreasOfPoint(lat, lon).ToList();
            var feedPlaces = GetGeoDeducPlaces(intersectingAreas.Select(p => p.ID));

            if (feedPlaces.Count < 2) { feedPlaces.AddRange(from p in new MobileSvcRepository().GetNearestLocationsV0(lat, lon, 4) select CfCacheIndex.Get(p.ID)); }

            return GetPostsForPlaces(feedPlaces, postType, clientType);
        }

        /// <summary>
        /// Select feed post relevant to a single location (and NO deduc places)
        /// </summary>
        public List<PostRendered> GetPostForLocation(Guid locationID, PostType postType, ClientAppType clientType)
        {
            var feedPlaces = new List<CfCacheIndexEntry>() { CfCacheIndex.Get(locationID) };
            return GetPostsForPlaces(feedPlaces, postType, clientType);
        }

        /// <summary>
        /// Select feed post relevant to an area
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public List<PostRendered> GetPostForArea(Guid areaID, PostType postType, ClientAppType clientType)
        {
            var feedPlaces = GetGeoDeducPlaces(new List<Guid> { areaID}); 
            return GetPostsForPlaces(feedPlaces, postType, clientType);
        }

        /// <summary>
        /// Underlying helper
        /// </summary>
        /// <param name="places"></param>
        /// <param name="postType"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        private List<PostRendered> GetPostsForPlaces(List<CfCacheIndexEntry> places, PostType postType, ClientAppType clientType)
        {
            if (places.Count == 0) { return GetPostForEverywhere(postType, clientType); }

            var feedPlaceIDs = places.Select(p => p.ID).Distinct().ToList();

            IQueryable<Post> query = postRepo.GetPostsInclude()
                .Where(c => c.IsPublic && feedPlaceIDs.Contains(c.PlaceID))
                .OrderByDescending(c => c.LastActivityUtc)
                .Take(ResultsPostCount);

            if (postType != PostType.Unknown) { query = query.Where(p => p.TypeID == (int)postType); }

            return ContentRenderer.BindPostsContent(query.ToList(), clientType);
        }

        /// <summary>
        /// Get distinct, no-null list of deduc CacheIndexEntry for a given set of place ids
        /// </summary>
        /// <param name="placeIDs"></param>
        /// <returns></returns>
        private List<CfCacheIndexEntry> GetGeoDeducPlaces(IEnumerable<Guid> placeIDs)
        {
            var geoDeduciblePlaces = new List<CfCacheIndexEntry>();

            foreach (var id in placeIDs)
            {
                var place = CfCacheIndex.Get(id);
                if (place != null)
                {
                    //-- Have to add if check for opinions of provinces bug
                    if (place.Type == CfType.Province) { geoDeduciblePlaces.Add(place); }
                    else { geoDeduciblePlaces.AddRange(CfPerfCache.GetGeoDeduciblePlaces(place)); }
                }
            }

            var feedPlaceIDs = geoDeduciblePlaces.Select(p => p.ID).Distinct().ToList();

            return (from c in feedPlaceIDs
                    where AppLookups.GetCacheIndexEntry(c) != null
                    select AppLookups.GetCacheIndexEntry(c)).ToList();
        }

        /// <summary>
        /// Basically the latest posts on the whole site/mobile
        /// </summary>
        public List<PostRendered> GetPostForEverywhere(PostType type, ClientAppType clientType)
        {
            var query = postRepo.GetPostsInclude().Where(p => p.IsPublic).OrderByDescending(c => c.LastActivityUtc).Take(ResultsPostCount);
            if (type != PostType.Unknown) { query = query.Where(p => p.TypeID == (int)type); }
            
            return ContentRenderer.BindPostsContent(query.ToList(), clientType);
        }

        /// <summary>
        /// Select feed post made by a single user
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public List<PostRendered> GetPostForUser(Guid userID, ClientAppType clientType, int count)
        {
            var posts = postRepo.GetAll().Where(p => p.UserID == userID).OrderByDescending(p => p.Utc).Take(count).ToList();
            
            return ContentRenderer.BindPostsContent(posts,clientType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public PostComment CreateComment(Post post, string message)
        {
            var comment = postRepo.CreatePostComment(post, 
                new PostComment() { ID = Guid.NewGuid(), Message = message, PostID = post.ID, UserID = CfIdentity.UserID, Utc = DateTime.UtcNow });

            //-- Log replies to partner calls!
            if (post.TypeID == (int)PostType.PartnerCall) { new cf.Services.PartnerCallService().LogPartnerCallCommentReply(post, comment); }

            //-- Send off Alerts!!
            new AlertsService().EnqueCommentWorkItem(CfIdentity.UserID, post.ID, comment.Message);

            return comment;
        }


        public void DeleteComment(Guid postID, Guid commentID)
        {
            var post = GetPostByID(postID);
            var comment = post.PostComments.Where(c => c.ID == commentID).Single();
            
            var userID = CfIdentity.UserID;

            var userHasRightsToDeletePost = (userID == post.UserID) || (userID == comment.UserID) || CfPrincipal.IsGod();
            if (!userHasRightsToDeletePost) { throw new AccessViolationException("Delete Post: Cannot delete this comment, because neither the post nor the comment belong to the current user."); }
            
            postRepo.DeletePostComment(post.ID, commentID);
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public Post CreateTalkPost(Guid placeID, string comment)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            var postMgr = new  cf.Content.Feed.V0.TalkPostManager();
            dynamic data = postMgr.CreateTemplateDynamicData(new { Comment = comment, Place = place });
            return postMgr.CreatePost(Guid.NewGuid(), CfIdentity.UserID, place, true, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        /// <remarks>
        /// Here we require a placeID because climbs belong to a place so OnObjectID is not a place, it just so happens that
        /// when we're adding areas and locations placeID == o.OnObjectID
        /// </remarks>
        internal Post CreateContentAddPost(ModAction o, Guid placeID, bool isPublic)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            var postMgr = new cf.Content.Feed.V0.ContentAddPostManager();
            dynamic data = postMgr.CreateTemplateDynamicData(GetPostPlace(o.OnObjectID));
            return postMgr.CreatePost(o.OnObjectID, o.UserID, place, isPublic, data);
        }

        //-- Here there is a difference between the posts Content (which may be a climb) and the post's Place
        internal Post UpdateContentAddPost(ModAction o) { return UpdateTypedPost(o.OnObjectID, new { Content = GetPostPlace(o.OnObjectID) }); }
        internal void DeleteContentAddPost(ModAction o) { DeleteTypedPost(postRepo.GetByID(o.OnObjectID)); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="placeID"></param>
        /// <param name="isPrivate"></param>
        /// <returns></returns>
        internal Post CreateOpinionPost(Opinion o, Guid placeID, bool isPublic)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            var postMgr = new cf.Content.Feed.V0.OpinionPostManager();
            dynamic data = postMgr.CreateTemplateDynamicData(o, GetPostPlace(o.ObjectID));
            return postMgr.CreatePost(o.ID, o.UserID, place, isPublic, data);
        }

        //-- Here there is a difference between the posts Content (which may be a climb) and the post's Place
        internal Post UpdateOpinionPost(Opinion o) { return UpdateTypedPost(o.ID, new { Content = GetPostPlace(o.ObjectID) }); }
        internal void DeleteOpinionPost(Opinion o) { DeleteTypedPost(postRepo.GetByID(o.ID)); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        internal Post CreateCheckInPost(CheckIn ci, bool isPublic) 
        {
            var place = AppLookups.GetCacheIndexEntry(ci.LocationID);
            var postMgr = new cf.Content.Feed.V1.CheckInPostManager();
            dynamic data = postMgr.CreateTemplateDynamicData(ci, place);
            return postMgr.CreatePost(ci.ID, ci.UserID, ci.Utc, place, isPublic, data);
        }

        //, Media = o.Media.ToList(), Climbs = o.LoggedClimbs }

        internal Post UpdateCheckInPost(CheckIn o) { return UpdateTypedPost(o.ID, new { CheckIn = o, Place = GetPostPlace(o.LocationID) }); }
        internal void DeleteCheckInPost(CheckIn obj) { DeleteTypedPost(postRepo.GetByID(obj.ID)); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        internal Post CreatePartnerCallPost(PartnerCall obj, bool isPublic)
        {
            var place = GetPostPlace(obj.PlaceID);
            var postMgr = new cf.Content.Feed.V0.PartnerCallPostManager();
            dynamic data = postMgr.CreateTemplateDynamicData(obj, place);
            return postMgr.CreatePost(obj.ID, obj.UserID, place, isPublic, data);
        }
        internal Post UpdatePartnerCallPost(PartnerCall o) { return UpdateTypedPost(o.ID, new { PartnerCall = o, Place = GetPostPlace(o.PlaceID) }); }
        internal void DeletePartnerCallPost(PartnerCall o) { DeleteTypedPost(postRepo.GetByID(o.ID)); }
        
        /// <summary>
        /// General purpose updated
        /// </summary>
        /// <param name="post"></param>
        private Post UpdateTypedPost(Guid postID, dynamic templateObjectInputs)
        {
            var post = postRepo.GetByID(postID);
            if (post != null)
            {
                var postMgr = PostTemplateLibrary.Get(post.TemplateKey);
                dynamic data = postMgr.CreateTemplateDynamicData(templateObjectInputs);
                return postMgr.UpdatePost(post, data);
            }
            return null;
        }

        /// <summary>
        /// General purpose delete
        /// </summary>
        /// <param name="post"></param>
        private void DeleteTypedPost(Post post)
        {
            if (post != null)
            {
                PostTemplateLibrary.Get(post.TemplateKey).DeletePost(post);
            }
        }

        /// <summary>
        /// This is just syntax sugar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private CfCacheIndexEntry GetPostPlace(Guid id) { return AppLookups.GetCacheIndexEntry(id); }
    }
}
