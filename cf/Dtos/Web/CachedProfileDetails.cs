using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using System.Runtime.Serialization;
using ProtoBuf;
using cf.Content;

namespace cf.Dtos
{
    /// <summary>
    /// Used to maintain session variables to speed up climb entry
    /// </summary>
    [ProtoContract]
    public class CachedProfileDetails : IUserBasicDetail
    {
        [ProtoMember(1)] public Guid ID { get; set; }
        [ProtoMember(2)] public string DisplayName { get; set; }
        [ProtoMember(3)] public string FullName { get; set; }
        [ProtoMember(4)] public string Avatar { get; set; }
        [ProtoMember(5)] public byte CountryID { get; set; }
        [ProtoMember(6)] public string Email { get; set; }
        [ProtoMember(7)] public string SlugUrlPart { get; set; }
        [ProtoMember(8)] public bool IsMale { get; set; }
        [ProtoMember(9)] public bool PrivacyPostsDefaultIsPublic { get; set; }

        public bool HasAvatar { get { return !string.IsNullOrWhiteSpace(Avatar); } }
        //public bool InitializedForSlug { get { return !string.IsNullOrWhiteSpace(Name); } }
        public string SlugUrl { get { if (String.IsNullOrEmpty(SlugUrlPart)) { return string.Format("/{0}/{1}", CfUrlProvider.ClimberUrlPrefix, ID); }
                else { return string.Format("/{0}/{1}", CfUrlProvider.ClimberUrlPrefix, SlugUrlPart); } } }

        public CachedProfileDetails() { }
        public CachedProfileDetails(cf.Entities.Profile p)
        {
            if (p != null)
            {
                ID = p.ID;
                DisplayName = p.DisplayName;
                Avatar = p.Avatar;
                CountryID = p.CountryID;
                FullName = p.FullName;
                Email = p.Email;
                SlugUrlPart = p.SlugUrlPart;
                IsMale = p.IsMale;
                PrivacyPostsDefaultIsPublic = p.PrivacyPostsDefaultIsPublic;
            }
            else
            {
                ID = Guid.Empty;
                DisplayName = "Deleted user";
            }
        }


    }
}
