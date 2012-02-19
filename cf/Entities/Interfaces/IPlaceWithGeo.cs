using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// A read only interface (only getters) that all our geography types (Country, Area, Location, LocationIndoor, 
    /// LocationPOI, LocationBusiness) adhere too. 
    /// </summary>
    /// <remarks>
    /// IDstring is used so that we can have both Countries and Areas/Locations implement this interface
    /// </remarks>
    public interface IPlaceWithGeo : IPlaceSearchable, ISearchable, IHasCountry, IHasCfSlugUrl, IRatable
    {
        SqlGeography Geo { get; }
        string Avatar { get; }
    }
}
