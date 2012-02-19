using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using NetFrameworkExtensions;
using NetFrameworkExtensions.SqlServer.Types;

namespace cf.Entities
{
    [MetadataType(typeof(Area_Validation))]
    public class Area : Place<Guid>, IArea, IPlaceSearchable, IGuidKeyObject, IRatableGeo
    {
        public int GeoReduceThreshold { get; set; }
        public byte TypeID { get; set; }
        public double ShapeArea { get; set; }

        public bool NoIndoorConfirmed { get; set; }
        public bool DisallowPartnerCalls { get; set; }

        public Area() { }

        public override CfType Type { get { return (CfType)TypeID; } }

        public override string AvatarRelativeUrl { get { if (HasAvatar) { return "/places/ar/" + Avatar; } return string.Empty; } }

        public override bool Equals(object obj)
        {
            var o = obj as Area;
            if (o == null) { return false; }
            
            if (this.CountryID != o.CountryID) return false;
            if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            if (this.GeoReduceThreshold != o.GeoReduceThreshold) return false;
            if (this.ID != o.ID) return false;
            if (this.Latitude != o.Latitude) return false;
            if (this.Longitude != o.Longitude) return false;
            if (this.Name != o.Name) return false;
            if (this.NameShort.GetEmptyIfNull() != o.NameShort.GetEmptyIfNull()) return false;
            if (this.NameUrlPart != o.NameUrlPart) return false;
            if (this.SearchSupportString.GetEmptyIfNull() != o.SearchSupportString.GetEmptyIfNull()) return false;
            if (this.NoIndoorConfirmed != o.NoIndoorConfirmed) return false;
            if (this.TypeID != o.TypeID) return false;

            if (this.Geo != null && o.Geo == null) return false;
            else if (this.Geo == null && o.Geo != null) return false;
            else if (this.Geo != null && o.Geo != null && this.Geo.GetWkt() != o.Geo.GetWkt()) return false;

            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
