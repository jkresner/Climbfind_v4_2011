using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    public class NewPartnerCallSubscriptionViewModel
    {
        [Required]
        public Guid ParnterCallPlaceID { get; set; }

        public bool ForIndoor { get; set; }
        public bool ForOutdoor { get; set; }

        public bool ExactOnly { get; set; }

        public bool EmailRealtime { get; set; }
        public bool MobileRealtime { get; set; }
    }
}