using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using cf.Entities;

namespace cf.Entities
{
    /// <summary>
    /// Dto carrying data used to render a bing map in specific way (e.g. zoom, center, heading etc.)
    /// </summary>
    public partial class PlaceBingMapView : IGuidKeyObject 
    {
        public static PlaceBingMapView GetDefaultCountrySettings(Country c)
        {
            return new PlaceBingMapView()
            {
                //-- Note we don't set the id on purpose because this flags to the save logic that it's a new record to add
                //ID = l.ID,
                MapTypeId = "road",
                Bounds = string.Empty,
                CenterOffset = "0",
                MapCenterLatitude = c.Latitude.ToString(),
                MapCenterLongitude = c.Longitude.ToString(),
                Zoom = "5"
            };
        }
        
        public static PlaceBingMapView GetDefaultIndoorSettings(ILocation l)
        {
            return new PlaceBingMapView()
            {
                //-- Note we don't set the id on purpose because this flags to the save logic that it's a new record to add
                //ID = l.ID,
                MapTypeId = "road",
                Bounds = string.Empty,
                CenterOffset = "0",
                MapCenterLatitude = l.Latitude.ToString(),
                MapCenterLongitude = l.Longitude.ToString(),
                Zoom = "14"
            };
        }
    }
}
