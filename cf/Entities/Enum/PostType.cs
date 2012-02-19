using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// Used to distinguish between different types of places that implement the IPlace interface
    /// </summary>
    /// <remarks>
    /// It is one of the mechanisms that allow us to build unique slugs for each place
    /// </remarks>
    public enum PostType
    {
        Unknown = 0,
        //-- Adding Places
        ContentAdd = 101,
        //LocationAdd = 111,
        //ClimbAdd = 121,
        Opinion = 501,
        MediaOpinion = 511,
        //-- Editing Profile
        Visit = 2111,
        PartnerCall = 3111,
        Talk = 4111,
        Introduction = 5001,       
        PersonalityMedia = 5111,
    }
}
