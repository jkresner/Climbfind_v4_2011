using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Validation;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Places
{
    [MetadataType(typeof(AreaCreate_Validation))]
    public class AreaNewViewModel
    {
        [Required(ErrorMessage="Please enter the name!")]
        public string locality { get; set; }
        
        public byte CountryID { get; set; }
        public byte TypeID { get; set; }
        public string Name { get; set; }
        public string WKT { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}