using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Types;
using cf.Entities;
using cf.Entities.Enum;
using cf.Entities.Interfaces;

namespace cf.Dtos
{
    public class PlaceWithModDetails
    {
        ObjectModMeta _modDetails { get; set; }
        CfCacheIndexEntry _place { get; set; }
        public bool PlaceDeleted { get { return _place == null; } }

        public PlaceWithModDetails(CfCacheIndexEntry place, ObjectModMeta modDetails)
        {
            _place = place;
            _modDetails = modDetails;
        }

        public Guid ID { get { return _modDetails.ID; } }
        public string IDstring { get { return _place.IDstring; } }
        public string Name { get { return _place.Name; } }
        public string NameShort { get { return _place.NameShort; } }
        public string NameUrlPart { get { return _place.NameUrlPart; } }
        public string SlugUrl { get { return _place.SlugUrl; } }
        public byte CountryID { get { return _place.CountryID; } }
        public CfType Type { get { return _place.Type; } }
        
        public int CQR { get { return _modDetails.CQR; } }
        public bool Verified { get { 
            return _modDetails.VerifiedAvatar == 3 &&
                   _modDetails.VerifiedDetails == 3; } }

        public bool VerifiedInThePast { get { return _modDetails.VerifiedLastUtc != null; } }
        public DateTime VerifiedLastUtc { get { return _modDetails.VerifiedLastUtc.Value; } }

        public string LastChangeType { get { return _modDetails.LastChangeType; } }
        public DateTime LastChangedUtc { get { return _modDetails.LastChangedUtc; } }
        public Guid LastChangedModUserID { get { return _modDetails.LastChangedUserID; } }
        public Guid LastChangedActionID { get { return _modDetails.LastChangedActionID; } }

        public Guid CreatedActionID { get { return _modDetails.CreatedActionID; } }
        public Guid CreatedByModUserID { get { return _modDetails.CreatedByUserID; } }
        public DateTime CreatedUtc { get { return _modDetails.CreatedUtc; } }
        public Guid DetailsLastChangedByModUserID { get { return _modDetails.DetailsLastChangedByUserID.Value; } }
        public DateTime DetailsLastChangedUtc { get { return _modDetails.DetailsLastChangedUtc.Value; } }
    }
}
