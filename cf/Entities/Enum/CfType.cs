using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// Used to distinguish between different types of cf domain objects that implement the IHasCfSlugUrl interface
    /// </summary>
    /// <remarks>
    /// It is one of the mechanisms that allow us to build unique slugs for each place
    /// </remarks>
    public enum CfType : byte
    {
        Unknown = 0,
        //-- Areas
        Country = 1,
        Province = 2,
        City = 3,
        ClimbingArea = 7,
        //-- Locations (non-climbing)
        CommercialIndoorClimbing = 10,
        SportsCenter = 11,
        PrivateIndoorClimbing = 12,
        RockWall = 21,
        RockBoulder = 23,
        RockWaterSoloing = 25,
        AlpineWall = 31,
        Summit = 41,
        IceWall = 51,
        //-- Locations (non-climbing / commercial)
        Food = 80,
        Accommodation = 83,
        Retailer = 85,
        Guide = 90,
        //-- Locations (non-climbing / commercial)
        ClimbingCarPark = 93,
        ClimbingApproachStart = 97,
        ClimbIndoor = 101,
        ClimbOutdoor = 103,
        User = 120
    }
}
