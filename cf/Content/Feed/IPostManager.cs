using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Dtos;

namespace cf.Content.Feed
{
    /// <summary>
    /// The post manager is responsible for maintaining the integrity of a post based on it's template version while an object it
    /// is associated with goes through updates / deletes.
    /// </summary>
    internal interface IPostManager
    {
        int PostTypeID { get; }
        string TemplateKey { get; }
        byte TemplateVersion { get; }
        string TemplateWeb { get; }
        string TemplateIphone { get; }
        string TemplateAndriod { get; }
        Post CreatePost(Guid id, Guid userID, CfCacheIndexEntry place, bool isPrivate, dynamic data);
        Post UpdatePost(Post post, dynamic data);
        string GetDataJson(dynamic data);
        dynamic CreateTemplateDynamicData(dynamic objectCollection);
        
        /// <summary>
        /// The value of delete post is the ability add logic to determine if a post should actually be delete upon deletion of
        /// it's parent object
        /// </summary>
        /// <param name="post"></param>
        /// <example>
        /// ??
        /// </example>
        void DeletePost(Post post);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string Render(dynamic data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string RenderMobile(dynamic data);
    }
}
