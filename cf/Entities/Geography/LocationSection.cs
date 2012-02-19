using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using NetFrameworkExtensions;
using cf.Content;

namespace cf.Entities
{
    public partial class LocationSection : IGuidKeyObject
    {
        public LocationSection() { }

        public LocationSection(Guid locID, ClimbType type, ClimbGradeType gradeType, string name) 
        {
            DefaultClimbTypeID = (byte)type;
            DefaultGradeTypeID = (byte)gradeType;
            LocationID = locID;
            Name = name;
        }
    }
}
