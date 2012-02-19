using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetFrameworkExtensions
{
    public static class BoolExtensions
    {
        public static string ToYesNo(this bool boolean)
        {
            if (boolean) { return ("Yes"); }
            return ("No");
        }

        public static string ToYesNoUnknown(this bool? boolean)
        {
            if (!boolean.HasValue) { return ("Unknown"); }
            else if (boolean.Value) { return ("Yes"); }
            else return ("No");
        }
    }
}
