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
    public partial class Media : IKeyObject<Guid>, IRatable
    {
        public string IDstring { get { return ID.ToString(); } }
        public string SlugUrl { get { return ""; } }
        public MediaType Type { get { return (MediaType)TypeID; } }
    }
}
