using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using NetFrameworkExtensions;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfileDto
    {
        public string ID { get; set; }
        public byte CountryID { get; set; }
        public string Avatar { get; set; }
        public string DisplayName { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string FacebookID { get; set; }
        public bool IsMale { get; set; }
        public string Home { get; set; }
        public string Fav1 { get; set; }
        public string Fav2 { get; set; }
        public string Fav3 { get; set; }
        public string Fav4 { get; set; }

        public ProfileDto(Profile p)
        {
            ID = p.ID.ToString("N");
            CountryID = p.CountryID;
            Avatar = p.Avatar;   
            DisplayName = p.DisplayName;
            NickName = p.NickName;
            FacebookID = p.FacebookID.ToString();
            IsMale = p.IsMale;
            Home = GetPlaceName(p.PlaceHome);
            Fav1 = GetPlaceName(p.PlaceFavorite1);
            Fav2 = GetPlaceName(p.PlaceFavorite2);
            Fav3 = GetPlaceName(p.PlaceFavorite3);
            Fav4 = GetPlaceName(p.PlaceFavorite4);
        }

        private string GetPlaceName(Guid? id)
        {
            string name = null;
            if (id.HasValue)
            {
                var p = cf.Caching.CfCacheIndex.Get(id.Value);
                if (p != null) { name = p.Name; }
            }
            return name;
        }
    }
}
