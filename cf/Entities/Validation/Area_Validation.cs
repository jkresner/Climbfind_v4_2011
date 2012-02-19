using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Entities.Validation
{
    public class AreaCreate_Validation : PlaceCreate_Validation
    {
        [Required(ErrorMessage = "WKT is required")]
        [DisplayName("Area boundaries")]
        public string WKT { get; set; }
    }

    public class Area_Validation : AreaCreate_Validation
    {
        [Required(ErrorMessage = "Geo reduce threshold required. If you don't know what you're doing leave it as '0'")]
        [Range(0, 5000)]
        public int GeoReduceThreshold { get; set; }

        [DisplayName("Search support string")]
        public string SearchSupportString { get; set; }
    }
}
