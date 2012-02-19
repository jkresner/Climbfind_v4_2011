using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.SqlServer.Types;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    public class MapItemCollection
    {
        public Collection<MapItem> Items { get; set; }
        
        public MapItemCollection() { Items = new Collection<MapItem>(); }
        
        public MapItem AddPoint(string title, string description, string link, string img, string pointType, double latitude, double longitude)
        {
            var displayImg = string.Format("d{0}", pointType);
            if (!string.IsNullOrWhiteSpace(img)) { 
                displayImg = img; 
            }
            
            MapItemPoint p = new MapItemPoint(new Point(latitude, longitude)) { T = title, L = link, I = displayImg, CT = pointType };
            Items.Add(p);
            return p;
        }
 
        /// <summary>
        /// This is when we are coloring in the polygon on the map, so no need to add an popup (with image, link etc.)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="polygonCoordinates"></param>
        /// <returns></returns>
        public MapItemPolygon AddPolygon(string title, string polygonCoordinates)
        {
            return AddPolygon(title, ParsePoints(polygonCoordinates));
        }

        private MapItemPolygon AddPolygon(string itemTitle, Point[] corners)
        {
            if (corners.Length < 3)
            {
                throw new ArgumentException("A polygon should contain at least 3 points");
            }

            if (corners[0].Latitude != corners[corners.Length - 1].Latitude ||
                corners[0].Longitude != corners[corners.Length - 1].Longitude)
            {
                throw new ArgumentException("The last and first points in a polygon should be the same");
            }

            MapItemPolygon p = new MapItemPolygon(corners) { T = itemTitle };
            Items.Add(p);
            return p;
        }

        public MapItemPolygon AddPolygonWithInteriorRings(string itemTitle, string polygon, List<string> interiors)
        {
            List<Point[]> _interiors = new List<Point[]>();
            foreach (string i in interiors) { _interiors.Add(ParsePoints(i)); }

            return AddPolygonWithInteriorRings(itemTitle, ParsePoints(polygon), _interiors);
        }

        private MapItemPolygon AddPolygonWithInteriorRings(string itemTitle, Point[] polygon, List<Point[]> interiors)
        {
            if (polygon.Length < 3)
            {
                throw new ArgumentException("A polygon should contain at least 3 points");
            }

            if (polygon[0].Latitude != polygon[polygon.Length - 1].Latitude ||
                polygon[0].Longitude != polygon[polygon.Length - 1].Longitude)
            {
                throw new ArgumentException("The last and first points in a polygon should be the same");
            }

            List<Point> totalShape = new List<Point>(polygon);

            foreach (var interior in interiors)
            {
                if (interior.Length < 3)
                {
                    throw new ArgumentException("A interior should contain at least 3 points");
                }

                if (interior[0].Latitude != interior[interior.Length - 1].Latitude ||
                    interior[0].Longitude != interior[interior.Length - 1].Longitude)
                {
                    throw new ArgumentException("The last and first points in a interior should be the same");
                }

                totalShape.AddRange(new List<Point>(interior));
            }

            //-- To close off the polygon add the start point
            totalShape.Add(new Point(totalShape[0].Latitude, totalShape[0].Longitude));

            MapItemPolygon p = new MapItemPolygon(totalShape.ToArray()) { T = itemTitle };
            Items.Add(p);
            return p;
        }


        public MapItemLine AddLine(string itemTitle, string itemDescription, string line)
        {
            return AddLine(itemTitle, itemDescription, ParsePoints(line));
        }

        private MapItemLine AddLine(string itemTitle, string itemDescription, Point[] line)
        {
            if (line.Length < 2)
            {
                throw new ArgumentException("A line should contain at least 2 points");
            }
            MapItemLine p = new MapItemLine(line) { T = itemTitle, D = itemDescription };
            Items.Add(p);
            return p;
        }


        public void AppendGeographyToGeoMapItemCollection(SqlGeography geog, string shapeName)
        {
            int numGeometries = (int)geog.STNumGeometries();

            for (int i = 1; i <= numGeometries; i++)
            {
                AppendGeographyType(geog.STGeometryN(i), shapeName);
            }
        }

        private void AppendGeographyType(SqlGeography geog, string shapeName)
        {
            var geoType = geog.STGeometryType();
            if (geoType == "Polygon")
            {
                AddItemsWithPossibleInterior(geog, shapeName);
            }
            else if (geoType == "Multipolygon")
            {
                AppendGeographyToGeoMapItemCollection(geog, shapeName);
            } 
        }

        private void AddItemsWithPossibleInterior(SqlGeography geog, string shapeName)
        {
            XNamespace ns = "http://www.opengis.net/gml";

            var coordinatesXml = XElement.Parse(geog.AsGml().Value);

            var totalXmlvalue = coordinatesXml.Value;

            var exteriorNode = (from c in coordinatesXml.Elements(ns + "exterior") select c).Single();
            var interiorNodes = (from c in coordinatesXml.Elements(ns + "interior") select c).ToList();

            if (interiorNodes.Count == 0)
            {
                AddPolygon(shapeName, exteriorNode.Value);
            }
            else
            {
                var coordinatesString = exteriorNode.Value;
                List<string> interiorCoordinates = new List<string>();
                foreach (var interiorset in interiorNodes) { interiorCoordinates.Add(interiorset.Value); }
                AddPolygonWithInteriorRings(shapeName, exteriorNode.Value, interiorCoordinates);
            }
        }


        private static Point[] ParsePoints(string line)
        {
            string[] s = line.Split(' ');

            if (s.Length % 2 != 0)
            {
                throw new ArgumentException("There must be an even number of points in a line or polygon");
            }

            List<Point> _line = new List<Point>();

            for (int x = 0; x < s.Length + 1 / 2; x += 2)
            {
                var shortPoint = s[x];
                var shortPoint2 = s[x + 1];
                if (shortPoint.Length > 8) { shortPoint = shortPoint.Substring(0, 8); }
                if (shortPoint2.Length > 8) { shortPoint2 = shortPoint2.Substring(0, 8); }

                Point p = new Point(double.Parse(shortPoint, CultureInfo.InvariantCulture), double.Parse(shortPoint2, CultureInfo.InvariantCulture));
                _line.Add(p);
            }
            return _line.ToArray();
        }
    }
}
