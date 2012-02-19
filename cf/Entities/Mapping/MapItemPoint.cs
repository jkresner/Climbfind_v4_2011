using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace cf.Entities
{
    public class MapItemPoint : MapItem
    {
        private Point _point;

        public MapItemPoint(Point point)
        {
            GT = "Point";
            _point = point;

            string lat = _point.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = _point.Longitude.ToString(CultureInfo.InvariantCulture);
            C = string.Format(lat + " " + lon); 
        }
    }
}
