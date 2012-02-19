using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    ///
    /// </summary>
    public enum ConversationStatus : byte
    {
        Unknown = 0,
        Unread = 21,
        Read = 31, //(Visible)
        AllDelete = 41,
        Spam = 51,
        Blocked = 61
    }
}
