using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Entities.Interfaces;

namespace cf.Dtos.Mobile.V1
{
    public abstract class ClimbDetailDto
    {
        public string LocID { get; set; }
        public string LocName { get; set; }
        //public LocationResultDto Location { get; set; }
        //public List<ClimbDetailLoggedClimbDto> Logs { get; set; }

        public ClimbDetailDto() { }

        public ClimbDetailDto(LocationEF loc)
        {
            LocID = loc.ID.ToString("N");

            if (!string.IsNullOrWhiteSpace(loc.NameShort)) { LocName = loc.NameShort; }
            else { LocName = loc.Name; }
            //Logs = new List<ClimbDetailLoggedClimbDto>();, IEnumerable<LoggedClimb> logs
            //foreach (var l in logs.OrderByDescending(l => l.Utc)) { Logs.Add(new ClimbDetailLoggedClimbDto(l)); }
        }
    }

    public class ClimbDetailSentClimbDto
    {
        public string ID { get; set; }
        public byte Experience { get; set; }
        public byte Outcome { get; set; }
        //public byte Rating { get; set; }
        public string Comment { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }

        public ClimbDetailSentClimbDto() { }
        public ClimbDetailSentClimbDto(LoggedClimb log, IUserBasicDetail by)
        {
            ID = log.ID.ToString("N");
            Comment = log.Comment;
            Experience = log.Experince;
            Outcome = log.Outcome;
            By = by.DisplayName;
            ByID = by.ID.ToString("N");
            ByPic = by.Avatar;
            
            //Rating = log.Rating;
            //Utc = utc.ToEpochTimeString(); 
        }
    }
}
