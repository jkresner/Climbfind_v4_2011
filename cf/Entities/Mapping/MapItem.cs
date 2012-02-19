using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace cf.Entities
{
    public class MapItem
    {
        //-- Geography type (point, line, polygon, circle)
        public string GT { get; protected set; }
        //-- Coordinates
        public string C { get; protected set; }
        //-- Title
        public string T { get; set; }
        //-- Description        
        public string D { get; set; }
        //-- Climbing Place Type (Used for the pin icon)
        public string CT { get; set; }
        //-- Link
        public string L { get; set; }
        //-- Image
        public string I { get; set; }

        protected static string GetSequenceOfPoints(Collection<Point> line)
        {
            StringBuilder coordinates = new StringBuilder();

            foreach (Point x in line) { coordinates.Append(x.ToString() + " "); }

            return coordinates.ToString();
        }
    }
}
