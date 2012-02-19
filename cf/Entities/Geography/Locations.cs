using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using NetFrameworkExtensions;
using System.Runtime.Serialization;

namespace cf.Entities
{
    public partial class LocationEF : IKeyObject<Guid>, ISearchable, IHasCfSlugUrl
    {
        public string IDstring { get { return ID.ToString(); } }
        public CfType Type { get { return (CfType)TypeID; } }
        public string SlugUrl { get { return cf.Content.CfUrlProvider.GetSlugUrl(this); } }

        public bool InitializedForSlug { get { return !string.IsNullOrWhiteSpace(NameUrlPart) && CountryID != 0; } }
    }
}
