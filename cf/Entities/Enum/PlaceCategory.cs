using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// A less granular categorization of PlaceType used conceptually for routing and grouping similar types of places into 
    /// shared functionality (pages and methods)
    /// </summary>
    public enum PlaceCategory
    {
        Unknown = 0,
        Country = 0111,
        Area = 1111,
        IndoorClimbing = 2111,
        OutdoorClimbing = 3111,
        Business = 4111,
        MeetingPoint = 5111,
        Climb = 9111
    }
}
