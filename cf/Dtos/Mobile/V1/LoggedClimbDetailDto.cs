using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using NetFrameworkExtensions;
using cf.Dtos.Cache;

namespace cf.Dtos.Mobile.V1
{    
    /// <summary>
    /// 
    /// </summary>
    public class LoggedClimbDetailDto
    {
        public string ID { get; set; }
        public string ClimbID { get; set; }
        public string CheckInID { get; set; }
        public string Name { get; set; }
        public string ByID { get; set; }
        public string ByName { get; set; }
        public string Avatar { get; set; }
        public string Grade { get; set; }
        public byte Outcome { get; set; }
        public byte Experience { get; set; }
        public byte GradeOpinion { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public string Utc { get; set; }

        public LoggedClimbDetailDto() { }

        public LoggedClimbDetailDto(LoggedClimb log, string climbGrade, string dtoAvatar, string byName)
        {
            ID = log.ID.ToString("N");
            ClimbID = log.ClimbID.ToString("N");
            CheckInID = log.CheckInID.ToString("N");
            Name = log.ClimbName;
            ByID = log.UserID.ToString("N");
            Grade = climbGrade;
            Avatar = dtoAvatar;
            ByName = byName;          
            Rating = log.Rating;
            Comment = log.Comment;
            Outcome = log.Outcome;
            Experience = log.Experince;
            GradeOpinion = log.GradeOpinion.Value;
            Utc = log.Utc.ToEpochTimeString();
        }
    }
}
