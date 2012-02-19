using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Post = cf.Entities.Post;
using PostComment = cf.Entities.PostComment;
using cf.DataAccess.Interfaces;
using cf.Entities.Enum;
using cf.Caching;

namespace cf.DataAccess.Repositories
{
    internal class PostRepository : AbstractCfEntitiesEf4DA<Post, Guid>, IKeyEntityAccessor<Post, Guid>,
        IKeyEntityWriter<Post, Guid>
    {
        public PostRepository() : base() { }
        public PostRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IQueryable<Post> GetPostsInclude() { return Ctx.Posts.Include("PostComments").Include("Profile"); }

        public IQueryable<Post> GetPostsForLocation(Guid locationID) { return Ctx.Posts.Include("PostComments").Where(c => c.PlaceID == locationID); }
        public IQueryable<Post> GetUsersPosts(Guid userID) { return Ctx.Posts.Include("PostComments").Where(c => c.UserID == userID); }
        //public IQueryable<Post> GetPostsForArea(Guid areaID)
        //{
        //    List<Guid> placeIDsForArea = CfPerfCache.GetGeoDeduciblePlaces(CfCacheIndex.Get(areaID)).Select(p=>p.ID).ToList();
        //    return Ctx.Posts.Include("PostComments").Where(c => placeIDsForArea.Contains(c.PlaceID));
        //}

        public PostComment CreatePostComment(Post post, PostComment postComment) 
        {
            post.PostComments.Add(postComment);
            post.LastActivityUtc = DateTime.UtcNow;
            Ctx.SaveChanges();
            return postComment;
        }

        public void DeletePostComment(Guid postID, Guid postCommentID)
        {
            Ctx.PostComments.DeleteObject(Ctx.PostComments.Where(entity => entity.ID.Equals(postCommentID)).Single());
            SaveChanges();
        }
    }
}
