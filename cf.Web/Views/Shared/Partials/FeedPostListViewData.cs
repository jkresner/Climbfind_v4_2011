using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Dtos;

namespace cf.Web.Views.Shared
{
    public class FeedPostListViewData
    {
        public List<PostRendered> FeedPosts { get; set; }
        public bool UserHasDeletePostRights { get; set; }
    }
}