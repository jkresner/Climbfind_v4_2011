using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class ClimbOutdoorDetailDto : ClimbDetailDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Grade { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        
        public byte ClimbType { get; set; }
        public byte ClimbTerrain { get; set; }
        public int Pitches { get; set; }
        public string DescWhere { get; set; }
        public string DescStart { get; set; }
        public string DescGear { get; set; }
        public string Safety { get; set; }
        public byte Index { get; set; }

        public Nullable<double> Rating { get; set; }
        public int RatingCount { get; set; }

        public ClimbOutdoorDetailDto() { }

        public ClimbOutdoorDetailDto(cf.Entities.ClimbOutdoor c) : base(c.Location)
        {
            ID = c.ID.ToString("N");
            Name = c.Name;
            Avatar = c.Avatar;
            Tags = c.ClimbTags.GetCategoriesString();
            ClimbType = c.ClimbTypeID;
            Description = c.Description;
            ClimbTerrain = c.ClimbTerrainID;
            Pitches = c.NumberOfPitches;
            Grade = c.GradeLocal;
            DescWhere = c.DescriptionWhere;
            DescStart = c.DescriptionStart;
            DescGear = c.DescriptionGear;
            Rating = c.Rating;
            RatingCount = c.RatingCount;
            Index = c.LocationPostionIndex;

            if (!String.IsNullOrWhiteSpace(c.SafetyRating) || !String.IsNullOrWhiteSpace(c.DescriptionSafety))
            {
                Safety = string.Format("{0} {1}", c.SafetyRating, c.DescriptionSafety);
            }


        }
    }
}
