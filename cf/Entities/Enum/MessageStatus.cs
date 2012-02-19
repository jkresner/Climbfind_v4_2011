using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum MessageStatus : byte
    {
        Unknown = 0,
        Unread = 21,
        Read = 31, //(Visible)
        Delete = 41,
        Spam = 51
    }
}