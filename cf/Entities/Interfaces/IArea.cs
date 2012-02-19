using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// Base object interface whose purpose is to expose our extension methods to all our objects.
    /// All other interface inherit from this interface.
    /// </summary>
    public interface IArea : IPlaceWithGeo
    {
        int GeoReduceThreshold { get; set; }
    }
}
