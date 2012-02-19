using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    public class TalkViewModel
    {
        [Required(ErrorMessage = "Place required")]
        public Guid TalkPlaceID { get; set; }

        [Required]
        public string TalkComment { get; set; }
    }
}