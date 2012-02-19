using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;

namespace cf.Entities
{
    public abstract class Place<T> : IPlaceWithGeo, IKeyObject<T> where T : IEquatable<T>
    {
        public T ID { get; set; }
        public string IDstring { get { return ID.ToString(); } }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string NameUrlPart { get; set; }
        public string SearchSupportString { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public bool HasAvatar { get { return !string.IsNullOrWhiteSpace(Avatar); } }
        public abstract string AvatarRelativeUrl { get; }
        public double? Rating { get; set; }
        public int RatingCount { get; set; }

        public SqlGeography Geo { get; set; }
        public byte CountryID { get; set; }

        public abstract CfType Type { get; }
        
        //-- Computed Properties
        public string ShortDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : NameShort; } }

        //-- Computed Properties
        public string VerboseDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : string.Format("{0} ({1})", Name, NameShort); } }

        //-- Computed Properties
        public string SlugUrl { get { return cf.Content.CfUrlProvider.GetSlugUrl(this); } }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool InitializedForSlug { get { 
            return !string.IsNullOrWhiteSpace(NameUrlPart) && CountryID != 0; } }
    }
}
