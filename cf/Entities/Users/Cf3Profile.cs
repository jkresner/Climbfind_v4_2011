using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    internal class Cf3Profile : cf.DataAccess.cf3.ClimberProfile, IKeyObject<Guid>
    {
        public string Sex
        {
            get
            {
                if (IsMale == true) { return "Male"; }
                if (IsMale == false) { return "Female"; }
                return "Unknown";
            }
        }

        public string HisHerString
        {
            get
            {
                if (IsMale == true) { return "his"; }
                if (IsMale == false) { return "her"; }
                return "their";
            }
        }

        public string ContactNumber
        {
            get
            {
                if (String.IsNullOrEmpty(ContractPhoneNumber)) { return "Not supplied"; }
                else { return ContractPhoneNumber; }
            }
        }

        public bool IsUnfinished { get { return IsDefault || ImageNotUploaded; } }
        public bool IsDefault { get { return FullName == "Unknown"; } }
        public bool ImageNotUploaded { get { return String.IsNullOrEmpty(ProfilePictureFile) || ProfilePictureFile == "Default.jpg"; } }
    }
}
