using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using NetFrameworkExtensions;
using cf.Web.Mvc.Helpers;
using cf.Entities;

namespace cf.Web.Models
{
    public class Bing7MapViewOptionsViewModel
    {
        //-- This property is better as a string, then it isn't a required field
        public string ID { get; set; }
        
        public string Bounds { get; set; }
        public string CenterOffset { get; set; }
        public string Heading { get; set; }
        
        [Required]
        public string MapCenterLatitude { get; set; }
        public string MapCenterLongitude { get; set; }
        public string MapTypeId { get; set; }
        public string Zoom { get; set; }
                
        public Bing7MapViewOptionsViewModel() { }

        public Bing7MapViewOptionsViewModel(PlaceBingMapView mapView) 
        {
            if (mapView != null)
            {
                ID = mapView.ID.ToString();
                Bounds = mapView.Bounds;
                CenterOffset = mapView.CenterOffset;
                Heading = mapView.Heading;
                MapCenterLatitude = mapView.MapCenterLatitude;
                MapCenterLongitude = mapView.MapCenterLongitude;
                MapTypeId = mapView.MapTypeId;
                Zoom = mapView.Zoom;
            }
        }
    }
}
