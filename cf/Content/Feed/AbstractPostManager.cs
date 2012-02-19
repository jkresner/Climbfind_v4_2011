using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.DataAccess.Repositories;
using cf.Dtos;

namespace cf.Content.Feed
{
    /// <summary>
    /// The post manager is responsible for maintaining the integrity of a post based on it's template version while an object it
    /// is associated with goes through updates / deletes.
    /// </summary>
    internal abstract class AbstractPostManager
    {
        public abstract int PostTypeID { get; }
        public abstract string TemplateKey { get; }
        public abstract string PostSlugFormat { get; }
        
        protected Post Create(Post post) { new PostRepository().Create(post); return post; }
        protected Post Update(Post post) { new PostRepository().Update(post); return post; }
        protected void Delete(Post post) { if (post != null) { new PostRepository().Delete(post.ID); } }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="place"></param>
        /// <param name="isPrivate"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual Post CreatePost(Guid id, Guid userID, CfCacheIndexEntry place, bool isPublic, dynamic data)
        {
            if (data == null) { throw new ArgumentNullException("Cannot create template with null dynamic data"); }

            var post = new Post(id, userID, place.ID, place.TypeID, isPublic);
            post.TypeID = PostTypeID;
            post.TemplateKey = TemplateKey;
            post.TemplateData = GetDataJson(data);
            post.SlugUrlPart = GetPostSlugUrlPart(id, place);
            post.LastActivityUtc = DateTime.UtcNow;

            return Create(post);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public string GetPostSlugUrlPart(Guid id, CfCacheIndexEntry place)
        {
            // e.g. "checkin-{0}-{1:MMdd}{2}"
            return string.Format(PostSlugFormat, place.NameUrlPart, DateTime.Now, id.ToString().Substring(0,8));
        }

        public Post UpdatePost(Post post, dynamic data)
        {
            if (data == null) { throw new ArgumentNullException("Cannot update template with null dynamic data"); }
            post.TemplateData = GetDataJson(data);
            return Update(post);
        }

        public virtual void DeletePost(Post post) { Delete(post); }

        public abstract string GetDataJson(dynamic data);

        /// <summary>
        /// Stop the feed from crashing by removing special characters that screw things up
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        protected string Sanitize(string rawText)
        {
            var sanitizedText = rawText.Replace(@"\", "") //-- Stop "Unrecognized escape sequence"
                    .Replace(@"""", "'"); //-- Stop our json becoming malformed

            return System.Web.HttpUtility.HtmlEncode(sanitizedText);
        }
    }
}
