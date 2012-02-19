using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace cf.Entities
{
    public class MapItemPolygon : MapItem
    {
        private Collection<Point> _polygon;
        
        public MapItemPolygon(Point[] polygon)
        {
            GT = "Polygon";
            
            if (polygon.Length < 3)
            {
                throw new ArgumentException("A polygon must have at least 3 coordinates");
            }

            if (polygon[0].Latitude != polygon[polygon.Length - 1].Latitude ||
                polygon[0].Longitude != polygon[polygon.Length - 1].Longitude)
            {
                throw new ArgumentException("The last and first points in a polygon should be the same");
            }

            _polygon = new Collection<Point>(polygon);
            C = GetSequenceOfPoints(_polygon);
            CT = string.Empty;
            D = string.Empty;
            I = string.Empty;
            L = string.Empty;
            T = string.Empty;
        }
    }
}
