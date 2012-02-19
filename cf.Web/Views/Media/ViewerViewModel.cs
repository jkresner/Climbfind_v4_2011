using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Entities.Validation;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Media
{
    public class ViewerViewModel
    {
        public Guid ObjectID { get; set; }
        public string ObjectType { get; set; }
        public string Name { get; set; }
        public string SlugUrl { get; set; }
        public string PageTitle { get; set; }
        public IEnumerable<cf.Entities.Media> MediaList { get; set; }
        public bool ShowAddMedia { get; set; }

        public ViewerViewModel(Guid objectID, string objectType, string name, string slugUrl, IEnumerable<cf.Entities.Media> mediaList, string pageTitle)
        {
            ObjectID = objectID;
            ObjectType = objectType;
            Name = name;
            SlugUrl = slugUrl;
            MediaList = mediaList;
            PageTitle = pageTitle;
            ShowAddMedia = true;
        }
    }
}