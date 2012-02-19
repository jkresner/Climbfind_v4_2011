using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Might be used for posts etc.</remarks>
    public interface IRatable
    {
        double? Rating { get; set; }
        int RatingCount { get; set; }
    }
    
    /// <summary>
    /// Used for bouncing objects up in value
    /// </summary>
    /// <remarks>Created for the rating summary view</remarks>
    public interface IRatableGeo : IRatable, IHasCfType
    {
        Guid ID { get; set; }
        string Name { get; set; }
    }
}
