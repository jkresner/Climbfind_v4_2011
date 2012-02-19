using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Entities.Validation
{
    public class PlaceCreate_Validation
    {
        [Required]
        public byte CountryID { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "* Name is required")]
        [DisplayName("Name of place")]
        [StringLength(140, ErrorMessage="Name must be less than 140 characters")]
        public string Name { get; set; }

        [DisplayName("Place type")]
        public byte TypeID { get; set; }
    }

    public class Place_Validation : PlaceCreate_Validation
    { 
        [DisplayName("Abbreviated or alternate name")]
        public string NameShort { get; set; }

        [Required(ErrorMessage = "* Url part is required")]
        //http://www.dailycoding.com/Utils/javascript/regexp_tester_javascript.html
        [RegularExpression(@"^[a-z\d]((?![\-]{2,})([a-z\d\-]))*[a-z\d]$", ErrorMessage = "* Url part must contain only lower case characters and '-' characters")]
        [DisplayName("Url part")]
        public string NameUrlPart { get; set; }
    }
}
