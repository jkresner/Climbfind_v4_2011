using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Entities.Interfaces;
using NetFrameworkExtensions;

namespace cf.Dtos
{
    public class PostRendered : Post
    {
        public bool IsStale { get; set; }
        public string UserDisplayName { get; set; }
        public string UserAvatar { get; set; }
        public string UserSlugUrl { get; set; }
        public string SexString { get; set; }
        public string Content { get; set; }

        public PostRendered(Post post, Profile profile, ISearchable place)
        {
            if (profile == null || place == null) { IsStale = true; }
            else
            {
                ID = post.ID;
                UserID = post.UserID;
                Utc = post.Utc;
                IsPublic = post.IsPublic;
                LastActivityUtc = post.LastActivityUtc;
                TypeID = post.TypeID;
                PlaceID = post.PlaceID;
                PostComments = post.PostComments;

                UserDisplayName = profile.DisplayName;
                UserAvatar = profile.Avatar;
                UserSlugUrl = profile.SlugUrl;
                SexString = profile.IsMale ? "his" : "her";
            }                        
        }
    }
}
