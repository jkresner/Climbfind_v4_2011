using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation
{
    /// <summary>
    /// This enumeration can be used any way a developer wants to categorize and be able to search any type of event
    /// </summary>
    public enum TraceCode
    {
        None = 0,
        MethodCall = 10,
        FailedHttpRequest = 101,

        //-- Application Events (1000-1999)
        AppStart = 1005,
        AppStartEnd = 1006,
        AppEnd = 1009,
        AppBuildCache = 1105,
        AppBuildSearchIndex = 1205,

        //-- User Account Events (2000-2999)
        UserCreateAccount = 2010,
        UserDeleteAccount = 2012,
        UserLogin = 2017,
        UserChangePassword = 2019,
        UserBecameModerator = 2021,
        
        //-- Email Events (3000-3999)


        //-- Media Events (4000-4999)
        PersistingImage = 4021,
        DeletingImage = 4025,


        //-- CF Domain Events (5000-5999)
        PartnerCallNofiticatonWorkItemProcess = 5021,


        //-- Unused
        Exception = 999999,
        Other = 1000000,
        UnitTest = 1000001          
    }
}
