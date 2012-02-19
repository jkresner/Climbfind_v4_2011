using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;

namespace NetFrameworkExtensions.SqlServer.Types
{
    /// <summary>
    /// Perform calculations on SqlGeography types
    /// </summary>
    public static partial class SqlGeographyExtensions
    {
        /// <summary>
        /// Returns a geometry version of a geography instance
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        /// <remarks>This is useful because many functions are implemented for geometry that are not implemented for geography, e.g. STContains</remarks>
        public static SqlGeometry AsGeom(this SqlGeography geo)
        {
            SqlString sstring = new SqlString(new string(geo.STAsText().Value));
            SqlGeometry geom = SqlGeometry.Parse(sstring);

            return geom;
        }
        
        /// <summary>
        /// Checks if one geography is completely contained in another
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="testIfChild"></param>
        /// <returns></returns>
        /// <remarks>By casting the type as a geometry (a bit of a hack hah)</remarks>
        public static bool STContains2(this SqlGeography parent, SqlGeography testIfChild)
        {
            SqlString sstring = new SqlString(new string(parent.STAsText().Value));
            SqlGeometry pgeom = SqlGeometry.Parse(sstring);

            SqlString csstring = new SqlString(new string(testIfChild.STAsText().Value));
            SqlGeometry cgeom = SqlGeometry.Parse(csstring);

            return pgeom.STContains(cgeom).Value;
        }

        /// <summary>
        /// Returns the Well Known Text version of a geography as a .net string
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        public static string GetWkt(this SqlGeography geo)
        {
            return new string(geo.STAsText().Value);
        }

        /// <summary>
        /// Return a SqlGeography instance from a latitude and longitude value
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="logitude"></param>
        /// <returns></returns>
        public static SqlGeography GetGeoPoint(double latitude, double logitude)
        {
            var wkt = string.Format("POINT({0} {1})", logitude, latitude).ToCharArray();
            SqlChars text = new SqlChars(wkt);
            return SqlGeography.STGeomFromText(text, 4326);    
        }
    }
}
