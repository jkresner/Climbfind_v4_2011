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
    public class EditViewModel
    {
        [Required]
        public Guid ID { get; set; }
        
        public string ReturnUrl { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}