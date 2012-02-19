using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Entities
{
    public partial class Post : IGuidKeyObject, IRatable 
    {
        public Post() { }

        public Post(Guid id, Guid userID, Guid placeID, byte placeTypeID, bool isPublic) 
        {
            ID = id;
            PlaceID = placeID; 
            PlaceTypeID = placeTypeID;
            Utc = DateTime.UtcNow;
            IsPublic = isPublic;
            UserID = userID;
        }
    }
}
