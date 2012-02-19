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
    [MetadataType(typeof(LocationOutdoor_Validation))]
    public partial class LocationOutdoor : IKeyObject<Guid>, ILocation, IPlaceSearchable, ISearchable, IRatableGeo
    {
        public Microsoft.SqlServer.Types.SqlGeography Geo 
        {
            get { return Microsoft.SqlServer.Types.SqlGeography.Point(Latitude, Longitude, 4326); } set { {;} } 
        }

        public bool HasDescription { get { return !(String.IsNullOrWhiteSpace(Description)); } }
        
        public bool HasAvatar { get { return !(String.IsNullOrWhiteSpace(Avatar)); } }
        public string AvatarRelativeUrl { get { if (HasAvatar) { return string.Format("/places/od/{0}", Avatar); } return string.Empty; } }

        //-- Computed Properties
        public string ShortDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : NameShort; } }

        //-- Computed Properties
        public string VerboseDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : string.Format("{0} ({1})", Name, NameShort); } }


        public override bool Equals(object obj)
        {
            var o = obj as LocationOutdoor;
            if (o == null) { return false; }

            if (this.CountryID != o.CountryID) return false;
            if (this.Description.GetEmptyIfNull() != o.Description.GetEmptyIfNull()) return false;
            if (this.ID != o.ID) return false;
            if (this.Latitude != o.Latitude) return false;
            if (this.Longitude != o.Longitude) return false;
            if (this.Name != o.Name) return false;
            if (this.NameShort.GetEmptyIfNull() != o.NameShort.GetEmptyIfNull()) return false;
            if (this.NameUrlPart != o.NameUrlPart) return false;
            if (this.SearchSupportString.GetEmptyIfNull() != o.SearchSupportString.GetEmptyIfNull()) return false;
            if (this.TypeID != o.TypeID) return false;

            if (this.Geo != null && o.Geo == null) return false;
            else if (this.Geo == null && o.Geo != null) return false;
            else if (this.Geo != null && o.Geo != null && this.Geo.GetWkt() != o.Geo.GetWkt()) return false;

            if (this.Approach != o.Approach) return false;
            if (this.AccessIssues != o.AccessIssues) return false;
            if (this.AccessClosed != o.AccessClosed) return false;
            if (this.Altitude != o.Altitude) return false;
            if (this.Cautions != o.Cautions) return false;
            if (this.ClimbingSummerAM != o.ClimbingSummerAM) return false;
            if (this.ClimbingSummerPM != o.ClimbingSummerPM) return false;
            if (this.ClimbingWinterAM != o.ClimbingWinterAM) return false;
            if (this.ClimbingWinterPM != o.ClimbingWinterPM) return false;
            if (this.ShadeAfternoon != o.ShadeAfternoon) return false;
            if (this.ShadeMidday != o.ShadeMidday) return false;
            if (this.ShadeMorning != o.ShadeMorning) return false;
            
            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }        
    }
}
