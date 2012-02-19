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
    /// It helps us to filter action types in type queries
    /// </remarks>
    public enum ModActionType
    {
        Unknown = 0,
        //-- Adding Places
        AreaAdd = 101,
        AreaEdit = 102,
        AreaVerifyEdit = 103,
        AreaEditVerified = 104,
        AreaSetAvatar = 105,
        AreaSetClimbingImage = 106,
        AreaVerifyAvatar = 108,
        AreaImageVerified = 109,
        AreaDelete = 110,
        LocationIndoorAdd = 201,
        LocationIndoorEdit = 202,
        LocationIndoorVerifyEdit = 203,
        LocationIndoorEditVerified = 204,
        LocationIndoorSetAvatar = 205,
        LocationIndoorSetLogo = 206,
        LocationIndoorVerifyImage = 208,
        LocationIndoorImageVerified = 209,
        LocationIndoorDelete = 210,
        LocationOutdoorAdd = 301,
        LocationOutdoorEdit = 302,
        LocationOutdoorVerifyEdit = 303,
        LocationOutdoorEditVerified = 304,
        LocationOutdoorSetAvatar = 305,
        LocationOutdoorVerifyAvatar = 308,
        LocationOutdoorImageVerified = 309,
        LocationOutdoorDelete = 310,
        ClimbAdd = 401,
        ClimbEdit = 402,
        ClimbVerifyEdit = 403,
        ClimbEditVerified = 404,
        ClimbSetAvatar = 405,
        ClimbVerifyAvatar = 408,
        ClimbImageVerified = 409,
        ClimbDelete = 410
    }
}
