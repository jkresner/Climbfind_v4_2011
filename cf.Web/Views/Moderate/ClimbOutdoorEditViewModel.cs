using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using System.ComponentModel;

namespace cf.Web.Models
{
    //[MetadataType(typeof(PlaceEdit_Validation))]
    public class ClimbOutdoorEditViewModel
    {
        public Guid ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string NameUrlPart { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage="Selection required")]
        public string GradeLocal { get; set; }
        public List<int> Categories { get; set; }

        public string NameShort { get; set; }
        public int? HeightInMeters { get; set; }
        public byte ClimbTypeID { get; set; }
        public byte ClimbTerrainID { get; set; }
        
        public string DescriptionStart { get; set; }
        public string DescriptionWhere { get; set; }
        public string DescriptionGear { get; set; }
        public string DescriptionSafety { get; set; }
        public string SafetyRating { get; set; }

        public int NumberOfPitches { get; set; }
        public int LocationPostionIndex { get; set; }

        public ClimbOutdoorEditViewModel()
        {
            Categories = new List<int>();
        }
    }
}