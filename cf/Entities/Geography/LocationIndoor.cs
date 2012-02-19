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
using Microsoft.SqlServer.Types;

namespace cf.Entities
{
    [MetadataType(typeof(LocationIndoor_Validation))]
    public partial class LocationIndoor : IKeyObject<Guid>, ILocation, IPlaceSearchable, ISearchable, IRatableGeo
    {
        public SqlGeography Geo { get { return SqlGeography.Point(Latitude, Longitude, 4326); } set { {;} }  }

        public bool HasAvatar { get { return !string.IsNullOrEmpty(Avatar); } }
        public bool HasLogo { get { return !string.IsNullOrEmpty(Logo); } }

        public string AvatarRelativeUrl { get { if (HasAvatar) { return string.Format("/places/id/{0}", Avatar); } return string.Empty; } }
        public string LogoRelativeUrl { get { if (HasLogo) { return string.Format("/places/id/{0}", Logo);} return string.Empty; } }

        public string WebsiteLink { get {
            if (string.IsNullOrWhiteSpace(Website)) { return "unknown"; }
            return Website.StartsWith("http://") ? Website : "http://" + Website; } }

        //-- Computed Properties
        public string ShortDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : NameShort; } }

        //-- Computed Properties
        public string VerboseDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : string.Format("{0} ({1})", Name, NameShort); } }


        public override bool Equals(object obj)
        {
            var o = obj as LocationIndoor;
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

            if (this.Address != o.Address) return false;
            if (this.BlogRssUrl != o.BlogRssUrl) return false;
            if (this.ContactEmail != o.ContactEmail) return false;
            if (this.ContactPhone != o.ContactPhone) return false;
            if (this.FloorspaceInSqMeters != o.FloorspaceInSqMeters) return false;
            if (this.HasBoulder != o.HasBoulder) return false;
            if (this.HasLead != o.HasLead) return false;
            if (this.HasTopRope != o.HasTopRope) return false;
            if (this.HeightInMeters != o.HeightInMeters) return false;
            if (this.IsPrivate != o.IsPrivate) return false;
            if (this.MapAddress != o.MapAddress) return false;
            if (this.NumberOfLines != o.NumberOfLines) return false;
            if (this.Prices != o.Prices) return false;
            if (this.Website != o.Website) return false;

            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}

