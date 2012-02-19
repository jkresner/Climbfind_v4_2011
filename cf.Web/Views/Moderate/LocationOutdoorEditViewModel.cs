using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using System.ComponentModel;
using cf.Entities.Enum;

namespace cf.Web.Models
{
    [MetadataType(typeof(PlaceEdit_Validation))]
    public class LocationOutdoorEditViewModel
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

        public string Approach { get; set; }
        public string AccessIssues { get; set; }
        public bool AccessClosed { get; set; }
        public int Altitude { get; set; }
        public bool? ClimbingWinterAM { get; set; }
        public bool? ClimbingWinterPM { get; set; }
        public bool? ClimbingSummerAM { get; set; }
        public bool? ClimbingSummerPM { get; set; }
        public string Cautions { get; set; }

        public bool ShadeMorning { get; set; }
        public bool ShadeMidday { get; set; }
        public bool ShadeAfternoon { get; set; }

        public Bing7MapWithLocationViewModel MapView { get; set; }

        public LocationOutdoorEditViewModel()
        {
            //-- This is required on postback to have the object to populate when model binding
            MapView = new Bing7MapWithLocationViewModel();
        }        
    }
}