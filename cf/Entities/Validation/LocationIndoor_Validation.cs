using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Entities.Validation
{
    public class LocationIndoorCreate_Validation : PlaceCreate_Validation 
    {
        //http://regexlib.com/DisplayPatterns.aspx?cattabindex=1&categoryId=2
        [RegularExpression(@"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$", ErrorMessage = "We need a valid website, so that we can chase up more details about this location.")]
        [Required]
        public string Website { get; set; }

        [Required]
        [DisplayName("Address on website")]
        public string Address { get; set; }
    }

    public class LocationIndoor_Validation : LocationIndoorCreate_Validation
    { 
        //[DisplayName("Contact email address")]
        public string ContactEmail { get; set; }

        //[DisplayName("Contact phone number")]
        public string ContactPhone { get; set; }
    }
}
