using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using NetFrameworkExtensions;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class ClimbIndoorDetailDto : ClimbDetailDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Alt { get; set; }
        public string Avatar { get; set; }
        public string Grade { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public string Lnum { get; set; }
        public string Mark { get; set; }
        public byte ClimbType { get; set; }
        public string Set { get; set; }
        public string Discontinued { get; set; }
        public string SectionName { get; set; }
        public string SectionID { get; set; }
        public bool SetAnon { get; set; }
        public string SetterInitials { get; set; }
        public string SetterID { get; set; }
        public string SetterAvatar { get; set; }

        public Nullable<double> Rating { get; set; }
        public int RatingCount { get; set; }

        public ClimbIndoorDetailDto() { }

        public ClimbIndoorDetailDto(cf.Entities.ClimbIndoor c) : base(c.Location)
        {
            ID = c.ID.ToString("N");
            Avatar = c.Avatar;
            Tags = c.ClimbTags.GetCategoriesString();
            ClimbType = c.ClimbTypeID;
            Description = c.Description;
            if (c.SetDate.HasValue) { Set = c.SetDate.Value.ToEpochTimeString(); }
            if (c.DiscontinuedDate.HasValue) { Discontinued = c.DiscontinuedDate.Value.ToEpochTimeString(); }
            Grade = c.GradeLocal;
            Lnum = c.LineNumber;
            Mark = string.Format("{0} {1}", c.MarkingColor, c.MarkingType == 2 ? "tape" : "holds");
            Name = c.Name;
            Alt = DtoHelper.GetPGAltName(c);
            Rating = c.Rating;
            RatingCount = c.RatingCount;
            if (c.SectionID.HasValue)
            {
                SectionID = c.SectionID.Value.ToString("N");
                SectionName = c.LocationSection.Name;
            }
            if (c.SetterID.HasValue && !c.SetterAnonymous)
            {
                SetterID = c.Setter.ID.ToString("N");
                SetterInitials = c.Setter.Initials;
                SetterAvatar = "";// c.Setter.Profile.Avatar;
            }
        }
    }
}
