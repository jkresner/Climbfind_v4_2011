using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using NetFrameworkExtensions;
using cf.Dtos.Cache;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class PartnerCallSubscriptionDto
    {
        public string ID { get; set; }
        public string PlaceID { get; set; }
        public byte Type { get; set; }
        public byte Country { get; set; }
        public string Flag { get; set; }
        public string Name { get; set; }
        public bool Indoor { get; set; }
        public bool Outdoor { get; set; }
        public bool Email { get; set; }
        public bool Mobile { get; set; }
        public bool ExactMatch { get; set; }
        
        public PartnerCallSubscriptionDto() {}

        public PartnerCallSubscriptionDto(CfCacheIndexEntry p, PartnerCallSubscription s)
        {
            ID = s.ID.ToString("N");
            PlaceID = p.ID.ToString("N");
            Type = (byte)p.Type;
            Country = p.CountryID;
            Flag = cf.Caching.AppLookups.CountryFlag(p.CountryID);
            Name = p.Name;
            Indoor = s.ForIndoor;
            Outdoor = s.ForOutdoor;
            Email = s.EmailRealTime;
            Mobile = s.MobileRealTime;
            ExactMatch = s.ExactMatchOnly;
        }
    }
}
