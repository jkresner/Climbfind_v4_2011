using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using System.Web.Script.Serialization;
using cf.Dtos;
using cf.Caching;
using cf.DataAccess.Repositories;
using cf.Entities.Enum;

namespace cf.Content.Feed
{
    /// <summary>
    /// Responsible for rendering the content of a feed posts
    /// </summary>
    public static class ContentRenderer
    {
        public static readonly JavaScriptSerializer Serializer;

        static ContentRenderer()
        {
            Serializer = new JavaScriptSerializer();
            Serializer.RegisterConverters(new[] { new NetFrameworkExtensions.Runtime.Serialization.DynamicJsonConverter() });
        }

        public static List<PostRendered> BindPostsContent(List<Post> posts, ClientAppType clientType)
        {
            if (clientType == ClientAppType.CfiPhone) { return BindPostsContentForIphone(posts); }
            return BindPostsContentForWeb(posts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        public static List<PostRendered> BindPostsContentForWeb(List<Post> posts)
        {
            var webPosts = new List<PostRendered>();

            foreach (var p in posts)
            {
                var place = AppLookups.GetCacheIndexEntry(p.PlaceID);

                var post = new PostRendered(p, p.Profile, place);
                if (!post.IsStale)
                {
                    post.Content = ContentRenderer.BindContentForWeb(p);
                    post.PostComments = p.PostComments;
                    webPosts.Add(post);
                }
            }

            return webPosts;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        public static List<PostRendered> BindPostsContentForIphone(List<Post> posts)
        {
            var mobPosts = new List<PostRendered>();

            foreach (var p in posts)
            {
                var place = AppLookups.GetCacheIndexEntry(p.PlaceID);

                var post = new PostRendered(p, p.Profile, place);
                if (!post.IsStale)
                {
                    post.Content = ContentRenderer.BindContentForMobile(p);
                    post.PostComments = p.PostComments;
                    mobPosts.Add(post);
                }
            }

            return mobPosts;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static string BindContentForWeb(Post post)
        {
            string content = string.Empty;

            var postMgr = PostTemplateLibrary.Get(post.TemplateKey);

            if (postMgr != null)
            {
                dynamic obj = Serializer.Deserialize(post.TemplateData, typeof(object));

                content = postMgr.Render(obj);
            }
            else
            {
                //-- Trace failed render?
                content = "Failed to render content for template " + post.TemplateKey;
            }

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static string BindContentForMobile(Post post)
        {
            string content = string.Empty;

            var postMgr = PostTemplateLibrary.Get(post.TemplateKey);

            if (postMgr != null)
            {
                dynamic obj = Serializer.Deserialize(post.TemplateData, typeof(object));
                content = postMgr.RenderMobile(obj);// post.TemplateData;
            }
            else
            {
                //-- Trace failed render?
                content = "Failed to render content for template " + post.TemplateKey;
            }

            return content.Replace("&#39;","'");
        }
    }
}
