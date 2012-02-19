using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum ClimbOutcome : byte
    {
        Unknown = 0,
        Attempt = 11,
        Breaks = 17,
        Redpoint = 23,
        Flash = 27,
        Onsight = 31
    }
}
