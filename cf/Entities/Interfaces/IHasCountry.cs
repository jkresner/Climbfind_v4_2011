using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Interfaces
{
    /// <summary>
    /// Declares that an object "has" a country context
    /// </summary>
    public interface IHasCountry
    {
        /// <summary>
        /// Country ID so we can show the flag, iso and any other country data (through AppLookups)
        /// </summary>
        byte CountryID { get; }
    }
}
