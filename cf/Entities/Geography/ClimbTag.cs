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
    public partial class ClimbTag
    {
        public ClimbTag() { }
        
        /// <summary>
        /// Construct default climb with an instantiated empty list of categories
        /// </summary>
        public ClimbTag(Guid climbID, int category)
        {
            this.ID = Guid.NewGuid();
            this.ClimbID = climbID;
            this.Category = category;
        }
    }
}
