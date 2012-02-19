using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using cf.Entities.Validation;

namespace cf.Web.Views.Places
{
    [MetadataType(typeof(LocationIndoorCreate_Validation))]
    public class LocationIndoorNewViewModel
    {
        public byte CountryID { get; set; }
        public byte TypeID { get; set; } 
        public string Website { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}