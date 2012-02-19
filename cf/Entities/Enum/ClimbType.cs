using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum ClimbType : int
    {
        Unspecified = 0,
        TopRopeOnly = 1,
        TopRopeAndLead = 2,
        LeadOnly = 3,
        Boulder = 4
    }
}
