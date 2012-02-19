using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    public class NewPartnerCallViewModel
    {
        [Required]
        public Guid ParnterCallPlaceID { get; set; }

        [Required(ErrorMessage = "Start Date required")]
        [RegularExpression("^[A-Z]{1}[a-z]{2}[ ]{1}[0-9]{2}[ ]{1}[A-Z]{1}[a-z]{2}[ ]{1}[2]{1}[0]{1}[0-9]{2}$", ErrorMessage = "Select date")]
        public string StartDate { get; set; }
        [RegularExpression("^[0|1|2]{1}[0-9]{1}[:][0-5]{1}[0-9]{1}$", ErrorMessage = "HH:mm required")]
        public string StartTime { get; set; }

        [RegularExpression("^[A-Z]{1}[a-z]{2}[ ]{1}[0-9]{2}[ ]{1}[A-Z]{1}[a-z]{2}[ ]{1}[2]{1}[0]{1}[0-9]{2}$", ErrorMessage = "Select date")]
        public string EndDate { get; set; }
        [RegularExpression("^[0|1|2]{1}[0-9]{1}[:][0-5]{1}[0-9]{1}$", ErrorMessage = "HH:mm required")]
        public string EndTime { get; set; }
        
        [Required]
        public ClimbingLevelGeneral PreferredLevel { get; set; }

        [Required]
        public string Comment { get; set; }

        public bool ForIndoor { get; set; }
        public bool ForOutdoor { get; set; }
    }
}