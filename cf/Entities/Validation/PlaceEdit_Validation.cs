using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Entities.Validation
{
    /// <summary>
    /// Properties common to all places for editing
    /// </summary>
    public class PlaceEdit_Validation
    {
        [Required]
        public Guid ID { get; set; }
        
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "* Name is required")]
        [DisplayName("Name of place")]
        public string Name { get; set; }

        [DisplayName("Abbreviated name")]
        [StringLength(16, ErrorMessage = "Abbreviated must be less than 15 characters")]
        public string NameShort { get; set; }

        [Required(ErrorMessage = "* Url part is required")]
        //http://www.dailycoding.com/Utils/javascript/regexp_tester_javascript.html
        [RegularExpression(@"^[a-z\d]((?![\-]{2,})([a-z\d\-]))*[a-z\d]$", ErrorMessage = "* Url part must contain only lower case characters and '-' characters")]
        [DisplayName("Url part")]
        public string NameUrlPart { get; set; }

        [DisplayName("Place type")]
        public string Type { get; set; }

        [DisplayName("Search terms")]
        public string SearchSupportString { get; set; }
    }
}
