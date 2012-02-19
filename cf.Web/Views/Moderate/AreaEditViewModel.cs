using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using cf.Entities.Validation;
using cf.Web.Models;

namespace cf.Web.Views.Moderate
{
    public class AreaEditViewModel
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
        [StringLength(15, ErrorMessage="Abbreviated must be less than 15 characters")]
        public string NameShort { get; set; }

        [Required(ErrorMessage = "* Url part is required")]
        //http://www.dailycoding.com/Utils/javascript/regexp_tester_javascript.html
        [RegularExpression(@"^[a-z\d]((?![\-]{2,})([a-z\d\-]))*[a-z\d]$", ErrorMessage = "* Url part must contain only lower case characters and '-' characters")]
        [DisplayName("Url part")]
        public string NameUrlPart { get; set; }

        [DisplayName("Place type")]
        public string Type { get; set; }

        [DisplayName("Search terms (inc. alternate names, historical climbers / area visionaries, nearby towns)")]
        public string SearchSupportString { get; set; }

        [Required]
        public string WKT { get; set; }

        [Required]
        public int GeoReduceThreshold { get; set; }

        [Required]
        public string PlaceType { get; set; }

        public string Description { get; set; }

        [DisplayName("Outdoor climbing ONLY")]
        public bool NoIndoorConfirmed { get; set; }

        public Bing7GeoJsonMapViewModel MapModel { get; set; }

        public AreaEditViewModel()
        {
            //-- This is required for model binding on edit else there is an exception
            MapModel = new Bing7GeoJsonMapViewModel();
        }
    }
}