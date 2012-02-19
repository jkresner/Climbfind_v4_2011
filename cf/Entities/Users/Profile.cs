using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Content;
using cf.Entities.Enum;

namespace cf.Entities
{
    public partial class Profile : IUserBasicDetail, ISearchable
    {
        public string SlugUrl
        {
            get
            { 
                if (String.IsNullOrEmpty(SlugUrlPart)) { return string.Format("/{0}/{1}", CfUrlProvider.ClimberUrlPrefix, ID); }
                else { return string.Format("/{0}/{1}", CfUrlProvider.ClimberUrlPrefix, SlugUrlPart.ToLower());}
            }
        }
        
        public string IDstring { get { return ID.ToString();} }
        public CfType Type { get { return CfType.User; } }
        public string Name { get { return DisplayName; } }
        public bool HasDefaultUserName { get { return UserName == IDstring.Substring(IDstring.Length - 9, 8); } }
        public string DisplayName
        {
            get
            {
                if (DisplayNameTypeID == 0) { return FullName; }
                if (DisplayNameTypeID == 1) { return UserName; }
                if (DisplayNameTypeID == 2) { return NickName; }
                return FullName;
            }
        }

        public bool HasFavorites { get { return PlaceFavorite1.HasValue ||
                 PlaceFavorite2.HasValue ||
                 PlaceFavorite3.HasValue ||
                 PlaceFavorite4.HasValue; } }

        public bool HasAvatar { get { return !string.IsNullOrWhiteSpace(Avatar);} }
        public bool InitializedForSlug { get { return !string.IsNullOrWhiteSpace(Name); } }
    }
}
