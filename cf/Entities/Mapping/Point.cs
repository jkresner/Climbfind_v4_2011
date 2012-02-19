using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace cf.Entities
{
    public class Point
    {
        private readonly double longitude;
        private readonly double latitude;

        public double Longitude { get { return longitude; } }
        public double Latitude { get { return latitude; } }

        public Point(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public static Point Parse(string coordinates)
        {
            string[] c = coordinates.Split(' ');

            if (c.Length != 2)
            {
                throw new ArgumentException("Insufficient coordinates. There must be at least 2 numbers");
            }

            double latitude;
            double longitude;

            if (!double.TryParse(c[0], NumberStyles.Any, CultureInfo.InvariantCulture, out latitude) ||
                !double.TryParse(c[1], NumberStyles.Any, CultureInfo.InvariantCulture, out longitude))
            {
                throw new ArgumentException("Coordinates could not be parsed into double numbers");
            }

            return new Point(latitude, longitude);
        }

        public override string ToString()
        {
            return latitude.ToString(CultureInfo.InvariantCulture) + " " + longitude.ToString(CultureInfo.InvariantCulture);
        }
    }
}