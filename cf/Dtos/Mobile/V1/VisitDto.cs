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
    public class VisitDto
    {
        public string ID { get; set; }
        public string LocID { get; set; }
        public string LocName { get; set; }
        public string ThumbUrl { get; set; }
        public string Utc { get; set; }
        public string UtcOut { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }
        public string Comment { get; set; }
        public List<VisitMediaDto> Media { get; set; }
        public List<VisitLoggedClimbDto> Climbs { get; set; }

        public VisitDto() {
            Media = new List<VisitMediaDto>();
            Climbs = new List<VisitLoggedClimbDto>();
        }

        public VisitDto(CheckIn c, CachedLocationDetails l, string by, string byPic)
        {
            ID = c.ID.ToString("N");
            LocID = c.LocationID.ToString("N");
            LocName = l.ShortDisplayName;
            Utc = c.Utc.ToEpochTimeString();
            if (c.OutUtc.HasValue) { UtcOut = c.OutUtc.Value.ToEpochTimeString(); }         
            By = by;
            ByID = c.UserID.ToString("N");
            ByPic = byPic;
            Comment = c.Comment;
            Climbs = new List<VisitLoggedClimbDto>();
            foreach (var cl in c.LoggedClimbs) { Climbs.Add(new VisitLoggedClimbDto(cl)); }
            Media = new List<VisitMediaDto>();
            foreach (var m in c.Media) { Media.Add(new VisitMediaDto() { ID = m.ID.ToString("N"), ThumbUrl = m.ThumbUrlRelative() }); }
            
            if (Media.Count > 0) { ThumbUrl = Media[0].ThumbUrl; } 
            else if (l.HasAvatar) { ThumbUrl = string.Format("/media/tm/{0}", l.Avatar); }
        }
    }

    public class VisitMediaDto 
    { 
        public string ID { get; set; }
        public string ThumbUrl { get; set; }
    }

    public class VisitLoggedClimbDto
    {
        public string ID { get; set; }
        public string ClimbID { get; set; }
        public string Name { get; set; }
        public byte Experience { get; set; }
        public byte Outcome { get; set; }
        public string Utc { get; set; }

        public VisitLoggedClimbDto() { }
        public VisitLoggedClimbDto(LoggedClimb l)
        {
            ID = l.ID.ToString("N");
            ClimbID = l.ClimbID.ToString("N");
            Name = l.ClimbName;
            Experience = l.Experince;
            Outcome = l.Outcome;
            Utc = l.Utc.ToEpochTimeString(); 
        }
    }
}
