using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using NetFrameworkExtensions;
using cf.Dtos.Cache;
using cf.Entities.Interfaces;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class PartnerCallDto
    {
        public string ID { get; set; }
        public string PlaceID { get; set; }
        public byte Type { get; set; }
        public byte Country { get; set; }
        public string PlaceName { get; set; }
        public bool Indoor { get; set; }
        public bool Outdoor { get; set; }
        public string CreatedUtc { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public byte PerferredLevel { get; set; }
        public string Comment { get; set; }
        public string ByID { get; set; }
        public string By { get; set; }
        public string ByPic { get; set; }
        
        public PartnerCallDto() {}

        public PartnerCallDto(CfCacheIndexEntry p, PartnerCall pc, IUserBasicDetail user)
        {
            ID = pc.ID.ToString("N");
            PlaceID = p.ID.ToString("N");
            Type = (byte)p.Type;
            Country = p.CountryID;
            PlaceName = p.Name;
            Indoor = pc.ForIndoor;
            Outdoor = pc.ForOutdoor;
            CreatedUtc = pc.CreatedUtc.ToEpochTimeString();
            StartDateTime = pc.StartDateTime.ToString("h:mm tt ddd MMM dd");
            if (!pc.HasDefaultEndDate)
            {
                EndDateTime = pc.EndDateTime.ToString("h:mm tt ddd MMM dd");
            }
            PerferredLevel = pc.PreferredLevel;
            Comment = pc.Comment;
            ByID = user.ID.ToString("N");
            By = user.DisplayName;
            ByPic = user.Avatar;
        }
    }
}
