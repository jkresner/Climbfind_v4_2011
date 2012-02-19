using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos
{
    /// <summary>
    /// Dto used to represent a geographical bounding box
    /// </summary>
    public class GeoBoundingBox : IEquatable<GeoBoundingBox>
    {
        public string SouthLat { get; set; }
        public string WestLon { get; set; }
        public string NorthLat { get; set; }
        public string EastLon { get; set; }

        public bool Equals(GeoBoundingBox other)
        {
            return EastLon == other.EastLon &&
                   NorthLat == other.NorthLat &&
                   SouthLat == other.SouthLat &&
                   WestLon == other.WestLon;
        }
    }
}
