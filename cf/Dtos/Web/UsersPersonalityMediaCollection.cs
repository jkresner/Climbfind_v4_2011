using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities;

namespace cf.Dtos
{
    public class UsersPersonalityMediaCollection : Dictionary<PersonalityCategory, UserPersonalityMedia>
    {
        public Guid UserID { get; set; }

        public bool HasAvatar { get { return Avatar != null; } }
        public UserPersonalityMedia Avatar { get; set; }

        public bool HasHeadshot { get { return Headshot != null; } }
        public UserPersonalityMedia Headshot { get; set; }

        public bool HasDaredevil { get { return Daredevil != null; } }
        public UserPersonalityMedia Daredevil { get; set; }

        public bool HasScenic { get { return Scenic != null; } }
        public UserPersonalityMedia Scenic { get; set; }

        public bool HasReady2Rock { get { return Ready2Rock != null; } }
        public UserPersonalityMedia Ready2Rock { get; set; }

        public bool HasFunny { get { return Funny != null; } }
        public UserPersonalityMedia Funny { get; set; }

        public bool HasBestShot { get { return BestShot != null; } }
        public UserPersonalityMedia BestShot { get; set; }

        public bool HasPartnerShot { get { return PartnerShot != null; } }
        public UserPersonalityMedia PartnerShot { get; set; }

        public bool HasPersonality { get { return HasAvatar ||
            HasHeadshot || HasDaredevil || HasScenic || HasReady2Rock || HasFunny || HasBestShot || HasPartnerShot ;} }
    }
}
