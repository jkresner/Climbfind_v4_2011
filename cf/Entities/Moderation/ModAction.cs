using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Entities
{
    public partial class ModAction : IGuidKeyObject 
    {
        public ModActionType Type { get { return (ModActionType)TypeID; } }
    }
}
