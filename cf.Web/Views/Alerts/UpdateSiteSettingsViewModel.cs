using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    public class UpdateSiteSettingsViewModel
    {
        public bool MessagesEmailRealTime { get; set; }
        public bool MessagesAlertFeed { get; set; }
        public bool MessagesMobileRealTime { get; set; }

        public bool CommentsOnPostsEmailRealTime { get; set; }
        public bool CommentsOnPostsAlertFeed { get; set; }
        public bool CommentsOnPostsMobileRealTime { get; set; }

        public bool CommentsOnMediaEmailRealTime { get; set; }
        public bool CommentsOnMediaAlertFeed { get; set; }
        public bool CommentsOnMediaMobileRealTime { get; set; }
    }
}