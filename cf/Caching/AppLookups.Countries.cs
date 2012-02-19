using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Entities.Enum;
using cf.Entities.Interfaces;

namespace cf.Caching
{
    /// <summary>
    /// Country related AppLookups methods
    /// </summary>
    public partial class AppLookups
    {
        private static Country usa = null;
        private static Country USA { get { if (usa == null) { usa = Countries.Where(c => c.ID == 245).Single(); } return usa; } }

        public static List<Country> Countries { get { return CPC.Countries; } }
        
        public static Country Country(byte countryID) {
            if (countryID == 83) { return new Country() { Name = "Great Britain", Flag = "gb", NameUrlPart = "/rock-climbing-england" }; }
            return Countries.Where(c => c.ID == countryID).Single(); 
        }
        
        public static Country Country(string countryNamePartUrl) 
        {
            if (string.Compare(countryNamePartUrl, "usa", true) == 0) { return USA; }
            return Countries.Where(c => c.NameUrlPart == countryNamePartUrl).SingleOrDefault(); 
        }
                
        public static string CountryFlag(byte countryID) {
            if (countryID == 0 || countryID == 117) { return "nn.png"; }
            else if (countryID == 83) { return "gb.png"; }
            return Country(countryID).Flag + ".png"; 
        }
        public static string CountryIso2(byte countryID) { return Country(countryID).Iso2; }
        public static List<Area> CountrysProvinces(byte countryID) { return CPC.GetProvinces(countryID); }

        public static string CountryUrl(byte countryID)
        { if (countryID == 0 || countryID == 117) { return "multi-country"; } else { return Country(countryID).NameUrlPart; } }

        public static string CountryName(byte countryID)
        { if (countryID == 0 || countryID == 117) { return "Multi Country"; } else { return Country(countryID).Name; } }
    }
}
