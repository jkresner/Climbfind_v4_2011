using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// ILocation is used for all the different types of location (indoor, outdoor, business, meeting point etc. etc.)
    /// </summary>
    public interface ILocation : IPlaceWithGeo, IKeyObject<Guid>
    {
        double Latitude { get; }
        double Longitude { get; }
    }
}
