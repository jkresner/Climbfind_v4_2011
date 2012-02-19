using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    public partial class ModProfile : IGuidKeyObject 
    {
        public static ModProfile InstansiateNewModProfile(Guid userID)
        {
            return new ModProfile()
            {
                ID = userID,
                ClimbsAdded = 0,
                VerifiedEdits = 0,
                Reputation = 0,
                PlacesAdded = 0,
                LastActivityUtc = DateTime.UtcNow,
                ModeratorSinceUtc = DateTime.UtcNow,
                Role = "ModCommunity"
            };
        }
    }
}
