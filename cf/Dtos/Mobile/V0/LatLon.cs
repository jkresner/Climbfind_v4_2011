using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cf.Dtos.Mobile.V0
{
    public class LatLon 
    { 
        public double Lat { get; set; } 
        public double Lon { get; set; }

        public bool Inflate()
        {
            var latLon = HttpContext.Current.Request.Headers["Latlon"];
            if (latLon == null) { return false; }

            try
            {
                var latString = latLon.Split(',')[0];
                var lonString = latLon.Split(',')[1];
                Lat = double.Parse(latString);
                Lon = double.Parse(lonString);

                if (Lat < -90 || Lat > 90) { return false; }
                if (Lon < -180 || Lon > 180) { return false; }
                if (Lon == 0 || Lat == 0) { return false; }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}