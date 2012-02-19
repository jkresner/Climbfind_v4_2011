using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// Used to distinguish between different types of alerts
    /// </summary>
    /// <remarks>
    /// It is one of the mechanisms that allow us to link back to an original post from an alert
    /// </remarks>
    public enum AlertType : byte
    {
        Unknown = 0,
        Message = 121,
        CommentOnPost = 131,
        CommentOnMedia = 141,
        PartnerCall = 151,
        NewRoutes = 161
    }
}
