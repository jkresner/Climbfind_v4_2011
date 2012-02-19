using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using cf.Entities.Validation;

namespace cf.Web.Models
{
    public class ClimbNewViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        
        [Required(ErrorMessage="Selection required")]
        public string GradeLocal { get; set; }

        [Required]
        public Guid LocationID { get; set; }
        
        public List<int> Categories { get; set; }

        public ClimbNewViewModel()
        {
            Categories = new List<int>();
        }
    }

    public class ClimbIndoorNewViewModel : ClimbNewViewModel
    {
        [Required]
        public string SetDateString { get; set; }
        
        public Guid? SetterID { get; set; }
        public Guid? SectionID { get; set; }
        public bool SetterAnonymous { get; set; }
        public byte ClimbTypeID { get; set; }

        public byte? MarkingType { get; set; }
        public string MarkingColor { get; set; }
        public string LineNumber { get; set; }

        public ClimbIndoorNewViewModel() : base() { }
    }


    public class ClimbOutdoorNewViewModel : ClimbNewViewModel
    {
        [Required]
        public byte ClimbTypeID { get; set; }
        
        public byte ClimbTerrainID { get; set; }
        public int NumberOfPitches { get; set; }
        public string DescriptionWhere { get; set; }
        public string DescriptionStart { get; set; }
        public string DescriptionGear { get; set; }
        public string DescriptionSafety { get; set; }
        public string SafetyRating { get; set; }
    }
}