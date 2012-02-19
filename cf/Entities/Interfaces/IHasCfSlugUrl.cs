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
    /// </remarks>
    public interface IHasCfSlugUrl : IHasCfType
    {
        string SlugUrl { get; }
        bool InitializedForSlug { get; }
    }
}
