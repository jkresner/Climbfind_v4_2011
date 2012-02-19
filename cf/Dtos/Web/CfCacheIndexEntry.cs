using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using System.Runtime.Serialization;
using ProtoBuf;

namespace cf.Dtos
{
    /// <summary>
    /// This is how we allows both locations (of all types) and areas (province, city, state) to be used in the feed.
    /// </summary>
    /// <remarks>
    /// This is effectively a slim down version of Areas & Locations (not Countries because the Key is different), and
    /// is loaded only as a get all by a stored procedure written specifically to get these details.
    /// </remarks>
    [ProtoContract]
    public class CfCacheIndexEntry : IGuidKeyObject, IHasCfSlugUrl, ISearchable, IHasPlaceSlugBits, IHasClimbSlugBits
    {
        [ProtoMember(1)] public Guid ID { get; set; }
        [ProtoMember(2)] public byte TypeID { get; set; }
        [ProtoMember(3)] public byte CountryID { get; set; }
        [ProtoMember(4)] public string Name { get; set; }
        [ProtoMember(5)] public string NameUrlPart { get; set; }
        [ProtoMember(6)] public string NameShort { get; set; }

        //-- Computed Properties
        public CfType Type { get { return (CfType)TypeID; } }
        public string IDstring { get { return ID.ToString(); } }
        public string ShortDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : NameShort; } }
        public string SlugUrl { get { return cf.Content.CfUrlProvider.GetSlugUrl(this); } }
        public bool InitializedForSlug { get { return !string.IsNullOrWhiteSpace(NameUrlPart) && CountryID != 0; } }
    }
}
