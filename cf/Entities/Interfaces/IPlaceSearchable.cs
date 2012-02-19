using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// Base object interface whose purpose is to expose our extension methods to all our objects.
    /// All other interface inherit from this interface.
    /// </summary>
    public interface IPlaceSearchable : ISearchable, IHasPlaceSlugBits
    {
        string NameShort { get; }
        string Description { get; set; }
        string SearchSupportString { get; set; }
    }
}
