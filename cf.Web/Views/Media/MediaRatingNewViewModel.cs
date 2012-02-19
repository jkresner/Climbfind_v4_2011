using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Entities.Validation;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Media
{
    public class MediaRatingNewViewModel
    {
        public Guid MediaID { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
    }
}