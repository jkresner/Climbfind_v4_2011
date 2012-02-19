using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos
{
    /// <summary>
    /// Dto representing a result from calling Bing or Google Geocode service
    /// </summary>
    public class GeocodeResult : IEquatable<GeocodeResult>
    {
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string Address { get; set; }
        public GeoBoundingBox Box { get; set; }

        /// <summary>
        /// Tells us if two geocode results are equivalent
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is useful when we calls a service in multiple ways or two different services, we can iterate through the union of
        /// the results and remove anything that looks the same
        /// </remarks>
        public bool Equals(GeocodeResult other) { return Name == other.Name && Lat == other.Lat && Lon == other.Lon && Box.Equals(other.Box); }
    }

    public class GeocodeResultEqualityComparer : IEqualityComparer<GeocodeResult>
    {
        public bool Equals(GeocodeResult left, GeocodeResult right) { return left.Equals(right); }
        public int GetHashCode(GeocodeResult r) { return (r.Name + r.Lat + r.Lon).GetHashCode(); }
    }
}
