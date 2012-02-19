using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using System.ComponentModel;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    [MetadataType(typeof(PlaceEdit_Validation))]
    public class LocationIndoorEditViewModel
    {
        public Guid ID { get; set; }
        public CfType Type { get; set; }
        public string Name { get; set; }

        public string NameShort { get; set; }
        
        public string NameUrlPart { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string SearchSupportString { get; set; }
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string MapAddress { get; set; }

        [Required]
        //http://regexlib.com/DisplayPatterns.aspx?cattabindex=1&categoryId=2
        [RegularExpression(@"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,4}(/\S*)?$", ErrorMessage = "We need a valid website, so that we can chase up more details about this location.")]
        public string Website { get; set; }

        public string BlogRssUrl { get; set; }

        [DisplayName("Contact email address")]
        public string ContactEmail { get; set; }

        [DisplayName("Contact phone number")]
        [Required]
        public string ContactPhone { get; set; }
        
        public string Prices { get; set; }

        [DisplayName("Floor space in square meters")]
        public int FloorspaceInSqMeters { get; set; }

        [DisplayName("Height in meters")]
        public int HeightInMeters { get; set; }

        [DisplayName("Number of rope lines")]
        public int NumberOfLines { get; set; }
        
        public bool HasTopRope { get; set; }
        public bool HasBoulder { get; set; }
        public bool HasLead { get; set; }

        public Bing7MapWithLocationViewModel LatLongEditorModel { get; set; }
        
    }
}