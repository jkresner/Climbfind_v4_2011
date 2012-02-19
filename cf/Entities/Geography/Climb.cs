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
    public partial class Climb : IKeyObject<Guid>, ISearchable, IHasClimbSlugBits, IRatableGeo
    {
        public string IDstring { get { return ID.ToString(); } }
        public string SlugUrl { get { return CfUrlProvider.GetSlugUrl(this); } }
        public CfType Type { get { return (CfType)this.TypeID;} }

        public bool HasAvatar { get { return !(String.IsNullOrWhiteSpace(Avatar)); } }
        public string AvatarRelativeUrl { get { if (!HasAvatar) { return string.Empty;} return string.Format("/places/cl/{0}", Avatar); } }

        public double? RatingScore { get { if (Rating.HasValue) { return (Rating * RatingCount) / 2; } return null; } }

        /// <summary>
        /// Construct default climb with an instantiated empty list of categories
        /// </summary>
        public Climb() { }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
            
            //var o = obj as Climb;
            //if (o == null) { return false; }

            //if (this.CountryID != o.CountryID) return false;
            //if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            //if (this.ID != o.ID) return false;
            //if (this.Name != o.Name) return false;
            //if (this.NameUrlPart != o.NameUrlPart) return false;
            //if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            //if (this.DescriptionWhere != o.DescriptionWhere) return false;
            //if (this.GradeLocal != o.GradeLocal) return false;
            //if (this.GradeCfNormalize != o.GradeCfNormalize) return false;
            //if (this.Avatar != o.Avatar) return false;
            //if (this.LocationID != o.LocationID) return false;
            //if (this.NumberOfPitches != o.NumberOfPitches) return false;
            //if (this.ParentID != o.ParentID) return false;
            //if (this.DiscontinuedDate != o.DiscontinuedDate) return false;
            //if (this.SetDate != o.SetDate) return false;
            //if (this.SetterID != o.SetterID) return false;
            //if (this.DescriptionSafety != o.DescriptionSafety) return false;
            //if (this.DescriptionStart != o.DescriptionStart) return false;
            //if (this.HeightInMeters != o.HeightInMeters) return false;
            //if (this.LocationPostionIndex != o.LocationPostionIndex) return false;
            
            //return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public bool InitializedForSlug { get { return !string.IsNullOrWhiteSpace(NameUrlPart); } }
    }
}
