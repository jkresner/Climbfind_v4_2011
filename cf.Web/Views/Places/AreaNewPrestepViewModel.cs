using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Validation;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Places
{
    public class AreaNewPrestepViewModel
    {
        [Required(ErrorMessage = "Please select a province")]
        public string countryUrlPart { get; set; }

        [Required(ErrorMessage = "Please select a province")]
        public string areaUrlPart { get; set; }
        
        [Required(ErrorMessage = "Please select the area type (city or other)")]
        public string type { get; set; }
    }
}