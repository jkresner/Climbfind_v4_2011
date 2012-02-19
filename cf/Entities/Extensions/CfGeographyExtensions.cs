using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using cf.Entities.Interfaces;
using Microsoft.SqlServer.Types;
using NetFrameworkExtensions.SqlServer.Types;

namespace cf.Entities
{
    /// <summary>
    /// Compare different cf geography objects
    /// </summary>
    public static partial class CfGeographyExtensions
    {
        /// <summary>
        /// Checks all areas in related areas to see if any completely contains the child place and returns the ones that do
        /// </summary>
        /// <param name="childPlace"></param>
        /// <param name="relatedAreas"></param>
        /// <returns></returns>
        public static List<Area> GetParentAreas(this IPlaceWithGeo childPlace, IEnumerable<Area> relatedAreas)
        {
            var parentAreas = new List<Area>();
            var cGeom = childPlace.Geo.AsGeom();

            foreach (var a in relatedAreas)
            {
                var ageom = a.Geo.AsGeom();
                
                //-- First check we're not comparing the area with itself, then if the current checking area completely contains the 
                //-- child area we add it to the parent areas list
                if (childPlace.IDstring != a.ID.ToString() && ageom.STContains(cGeom)) { parentAreas.Add(a); }
            }

            return parentAreas;
        }

        /// <summary>
        /// Checks all areas in related areas to see if any intersect with the test place and returns the ones that do
        /// </summary>
        /// <param name="place"></param>
        /// <param name="relatedAreas"></param>
        /// <returns></returns>
        public static List<Area> GetIntersectingAreas(this IPlaceWithGeo testPlace, IList<Area> relatedAreas)
        {
            var intersectingAreas = new List<Area>();

            foreach (var a in relatedAreas)
            {
                if (testPlace.IDstring != a.ID.ToString() && a.Geo.STIntersects(testPlace.Geo)) { intersectingAreas.Add(a); }
            }

            return intersectingAreas;
        }

        /// <summary>
        /// Checks all areas in related areas to see if any do not intersect with the test place and returns them
        /// </summary>
        /// <param name="place"></param>
        /// <param name="relatedAreas"></param>
        /// <returns></returns>
        public static List<Area> GetNonIntersectingAreas(this IPlaceWithGeo testPlace, IList<Area> relatedAreas)
        {
            List<Area> notIntersectingAreas = new List<Area>();

            foreach (var a in relatedAreas) 
            {
                if (testPlace.IDstring != a.ID.ToString() && !a.Geo.STIntersects(testPlace.Geo)) { notIntersectingAreas.Add(a); }
            }

            return notIntersectingAreas;
        }

        /// <summary>
        /// Takes a list of areas and returns only the most encapsulating areas by filtering out areas that fall inside other areas
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public static List<Area> RemoveAllChildAreas(this IList<Area> areas)
        {
            List<Area> parentAreas = new List<Area>(), childAreas = new List<Area>();
            
            //-- Sort the areas by largest area and then check to see if any of the smaller areas are inside the larger ones
            var areasSortedBySize = areas.OrderByDescending(a => a.ShapeArea).ToList();

            foreach (var a in areasSortedBySize)
            {
                //-- If we've already identified that the area fits inside another area, we don't need to check again
                if (!childAreas.Contains(a))
                {
                    //-- Loop through the whole collection to see if any are contained by the current
                    for (int j = 0; j < areasSortedBySize.Count; j++)
                    {
                        var testArea = areasSortedBySize[j];
                        //-- don't check if we're comparing against itself as it's always going to put all the areas into child areas
                        if (testArea.ID != a.ID)
                        {
                            //-- If testArea is contained by 'a' it's a child area!
                            if (a.Geo.STContains2(testArea.Geo)) { childAreas.Add(testArea); }
                        }
                    }
                }
            }

            //-- Our parent areas are ones we haven't marked as child areas :)
            foreach (var a in areasSortedBySize) { if (!childAreas.Contains(a)) { parentAreas.Add(a); } }

            return parentAreas;
        }

        /// <summary>
        /// Used in the mapping service to stop parent areas hiding sub areas of the current focused area
        /// </summary>
        /// <param name="area"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
        /// <remarks>Use for GetProvinceItems, GetCityItems, GetAreaItems</remarks>
        /// <example>(1) The Bay Area hiding all the sub areas of San Francisco. (2) Patagonia hiding EVERYTHING in Argentina/Chile hahah        /// </example>
        public static List<Area> RemoveIntersectingNonChildAreas(this IPlaceWithGeo testPlace, IList<Area> areas)
        {
            var nonParentAreas = new List<Area>();

            foreach (var a in areas)
            {
                if (testPlace.IDstring != a.ID.ToString())
                {
                    if (testPlace.Geo.STContains2(a.Geo)) { nonParentAreas.Add(a); }
                }
            }

            return nonParentAreas;
        }

        /// <summary>
        /// Get locations that do not fall within any of the test areas
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="areasToTest"></param>
        /// <returns></returns>
        /// <remarks>
        /// Useful for representing outdoor pages: When looking at an area that has sub areas and locations that fall within the
        /// main area being displayed but none of the sub areas (areas parameter to this method). Hence of the page we want to
        /// display the areas and also the orphan locations
        /// </remarks>
        public static List<Location> GetOrphanLocations(this List<Location> locations, IList<Area> areasToTest)
        {
            var orphans = new List<Location>();
            foreach (var l in locations)
            {
                if (!l.PlaceIntersectsAnArea(areasToTest)) { orphans.Add(l); }
            }

            return orphans;
        }

        /// <summary>
        /// Checks if a place falls within on of the test areas
        /// </summary>
        /// <param name="testPlace"></param>
        /// <param name="areasToTest"></param>
        /// <returns></returns>
        public static bool PlaceIntersectsAnArea(this IPlaceWithGeo testPlace, IList<Area> areasToTest)
        {
            foreach (var a in areasToTest) { if (testPlace.Geo.STIntersects(a.Geo)) { return true; } }

            return false;
        }
    }
}
