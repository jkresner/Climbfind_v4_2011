using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public enum ModFlagReason
    {
        NonExistantObject = 4001,
        DuplicateObject = 4003,
        GeographyIncorrect = 4005,
        SpellingMistake = 4007,
        ImageIncorrect = 4009,
        ContentIncorrect = 4011,
        CopyrightInfringement = 4013,
        PoorContent = 4015
    }
}
