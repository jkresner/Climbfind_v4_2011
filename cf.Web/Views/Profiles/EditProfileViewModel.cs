using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Web.Models
{
    public class EditProfileViewModel // : EditPlacePreferencesViewModel
    {
        [Required(ErrorMessage = "Full name required")]
        [DisplayName("Full name")]
        [StringLength(40, ErrorMessage = "Full name must be between 3 and 40 characters", MinimumLength=3)]
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }

        public byte DisplayNameTypeID { get; set; }
        
        public bool IsMale { get; set; }
        public byte CountryID { get; set; }
        
        public string SlugUrlPart { get; set; }

        [StringLength(40, ErrorMessage = "Contact number must be between 8 and 40 characters", MinimumLength = 8)]
        public string ContactNumber { get; set; }

        public string GradesAverage { get; set; }
        public string GradesMaximum { get; set; }

        public bool PrivacyShowFeed { get; set; }
        public bool PrivacyPostsDefaultIsPublic { get; set; }
        public bool PrivacyShowHistory { get; set; }
        public bool PrivacyAllowNewConversations { get; set; }
        public bool PrivacyShowOnPartnerSites { get; set; }
        public bool PrivacyShowInSearch { get; set; }
    }
}