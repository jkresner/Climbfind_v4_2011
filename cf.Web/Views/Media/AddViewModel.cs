using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Entities.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using cf.Web.Views.Shared.Partials;
using cf.Entities.Enum;

namespace cf.Web.Views.Media
{
    public class AddViewModel : INewMediaViewModel
    {
        public Guid ObjectId { get; set; }
        public string ObjectSlug { get; set; }
        public string ObjectName { get; set; }
        public bool ChooseFromExisting { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public MediaType Type { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; } 
    }
}