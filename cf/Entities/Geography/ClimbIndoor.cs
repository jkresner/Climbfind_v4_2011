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
    public partial class ClimbIndoor : IKeyObject<Guid>, ISearchable, IHasClimbSlugBits, IRatableGeo
    {
        public override bool Equals(object obj)
        {
            var o = obj as ClimbIndoor;
            if (o == null) { return false; }

            if (this.CountryID != o.CountryID) return false;
            if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            if (this.ID != o.ID) return false;
            if (this.Name != o.Name) return false;
            if (this.NameUrlPart != o.NameUrlPart) return false;
            if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            if (this.GradeLocal != o.GradeLocal) return false;
            if (this.GradeCfNormalize != o.GradeCfNormalize) return false;
            if (this.Avatar != o.Avatar) return false;
            if (this.LocationID != o.LocationID) return false;
            if (this.SetDate != o.SetDate) return false;
            if (this.SetterID != o.SetterID) return false;
            if (this.HeightInMeters != o.HeightInMeters) return false;
            
            if (this.DiscontinuedDate != o.DiscontinuedDate) return false;
            if (this.ClimbTypeID != o.ClimbTypeID) return false;
            if (this.MarkingColor != o.MarkingColor) return false;
            if (this.MarkingType != o.MarkingType) return false;
            if (this.NameShort != o.NameShort) return false;
            if (this.SectionID != o.SectionID) return false;
            if (this.SetterAnonymous != o.SetterAnonymous) return false;
            
            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
