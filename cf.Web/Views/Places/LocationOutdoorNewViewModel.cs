using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using cf.Entities.Validation;
using cf.Web.Models;
using cf.Entities.Enum;

namespace cf.Web.Views.Places
{
    public class LocationOutdoorNewViewModel
    {
        public byte CountryID { get; set; }

        [Required(ErrorMessage = "* Name is required")]
        [DisplayName("Name of place")]
        [StringLength(140, ErrorMessage = "Name must be less than 140 characters")]
        public string Name { get; set; }

        [DisplayName("Place type")]
        public CfType Type { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public Bing7MapViewOptionsViewModel ViewOptions { get; set; }
    }
}