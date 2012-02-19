using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities;
using cf.Entities.Interfaces;

namespace cf.Content
{
    /// <summary>
    /// Generates unique url parts and full slug urls for cf Entities
    /// </summary>
    public static class CfUrlProvider
    {
        /// <summary>
        /// Constants used for building urls
        /// </summary>
        public const string CountryUrlPrefix = "rock-climbing";
        public const string CountryUrlFormat = "/{0}-{1}";
        public const string ClimberUrlPrefix = "climber";
        public const string AreaUrlPrefix = "rock-climbing";
        public const string OutdoorUrlPrefix = "outdoor-climbing";
        public const string ClimbUrlPrefix = "climb";
        public const string IndoorUrlPrefix = "indoor-climbing";
        public const string MediaUrlPrefix = "media";

        /// <summary>
        /// Dictionaries used for building urls
        /// </summary>
        private static readonly Dictionary<CfType, string> PlaceTypeToUrl;
        private static readonly Dictionary<PlaceCategory, string> PlaceCategoryToUrl;
        private static readonly Dictionary<byte, string> CountryIdToUrl;
        
        /// <summary>
        /// Static constructor to initialize our url building dictionaries
        /// </summary>
        static CfUrlProvider()
        {
            PlaceTypeToUrl = InitializePlaceTypeToUrl();
            PlaceCategoryToUrl = InitializePlaceCategoryToUrl();
            CountryIdToUrl = InitializeCountryIdToUrl();
        }
        
        /// <summary>
        /// To level slug resolving url that delegates to the type specific slug building logic
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetSlugUrl(IHasCfSlugUrl o)
        {
            if (!o.InitializedForSlug) { 
                return "/object-not-initialized"; }
            
            if (o.Type == CfType.User) { return GetUserSlugUrl(o as Profile); }
            if (o.Type == CfType.ClimbIndoor || o.Type == CfType.ClimbOutdoor) { 
                var c = o as IHasClimbSlugBits; return GetClimbSlugUrl(c.CountryID, c.NameUrlPart, c.ID); }

            //-- Else if the type is a place type go further and work on the categories
            var p = o as IHasPlaceSlugBits;
            if (p != null)
            {
                var cat = o.Type.ToPlaceCateogry();
                if (cat == PlaceCategory.Country) { return GetCountrySlugUrl(p.NameUrlPart); }
                else if (cat == PlaceCategory.Area) { return GetAreaSlugUrl(p.CountryID, p.NameUrlPart); }
                else if (cat == PlaceCategory.IndoorClimbing) { return GetLocationIndoorSlugUrl(p.CountryID, p.NameUrlPart); }
                else if (cat == PlaceCategory.OutdoorClimbing) { return GetLocationOutdoorSlugUrl(p.Type, p.CountryID, p.NameUrlPart); }
                else if (cat == PlaceCategory.Business) { throw new NotImplementedException("Business slugs not implemented"); }
                else if (cat == PlaceCategory.MeetingPoint) { throw new NotImplementedException("MeetingPoint slugs not implemented"); }
            }
            
            throw new NotImplementedException("Unknown category type slugs not implemented"); 
        }

        /// <summary>
        /// Build the slug for a user by first checking if they created their own custom slug and if not falls back onto their userID
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        private static string GetUserSlugUrl(Profile userProfile)
        {
            if (!string.IsNullOrWhiteSpace(userProfile.SlugUrl))
            {
                return string.Format("/{0}/{1}", ClimberUrlPrefix, userProfile.SlugUrl); 
            }
            
            return string.Format("/{0}/{1}", ClimberUrlPrefix, userProfile.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private static string GetCountrySlugUrl(string nameUrlPart) { return string.Format(CountryUrlFormat, CountryUrlPrefix, nameUrlPart); }
        private static string GetAreaSlugUrl(byte countryID, string areaNameUrlPart) { return string.Format("/{0}-{1}/{2}", AreaUrlPrefix, GetCountryUrlPartShort(countryID), areaNameUrlPart); }
        private static string GetLocationIndoorSlugUrl(byte countryID, string locationNameUrlPart) { return string.Format("/{0}-{1}/{2}", IndoorUrlPrefix, GetCountryUrlPartShort(countryID), locationNameUrlPart); }
        private static string GetLocationOutdoorSlugUrl(CfType placeType, byte countryID, string locationNameUrlPart)
        { return string.Format("/{0}-{1}/{2}/{3}", OutdoorUrlPrefix, GetCountryUrlPartShort(countryID), GetLocationTypeUrlPart(placeType), locationNameUrlPart); }

        private static string GetClimbSlugUrl(byte countryID, string nameUrlPart, Guid id) { 
            return string.Format("/{0}/{1}/{2}", ClimbUrlPrefix, nameUrlPart, id); }

        private static string GetLocationCategoryUrlPart(PlaceCategory geoType) { return PlaceCategoryToUrl[geoType]; }
        private static string GetLocationTypeUrlPart(CfType geoType) { return PlaceTypeToUrl[geoType]; }
        private static string GetCountryUrlPart(byte id) { return CountryIdToUrl[id]; }
        
        /// <summary>
        /// For seo reasons we try to shorten the url sc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetCountryUrlPartShort(byte id) {
            if (id == 245) { return "usa"; }
            return GetCountryUrlPart(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<PlaceCategory, string> InitializePlaceCategoryToUrl()
        {
            Dictionary<PlaceCategory, string> dic = new Dictionary<PlaceCategory, string>();
            dic.Add(PlaceCategory.Area, "rock-climbing-around");
            dic.Add(PlaceCategory.Business, "climber-friendly-business");
            dic.Add(PlaceCategory.Country, "rock-climbing");
            dic.Add(PlaceCategory.IndoorClimbing, "indoor-climbing");
            dic.Add(PlaceCategory.MeetingPoint, "climber-point-of-interest");
            dic.Add(PlaceCategory.OutdoorClimbing, "outdoor-rock-climbing");
            return dic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<CfType, string> InitializePlaceTypeToUrl()
        {
            Dictionary<CfType, string>  dic = new Dictionary<CfType, string>();
            //-- Todo ? read from a database ?
            dic.Add(CfType.Accommodation, "accommodation");
            dic.Add(CfType.AlpineWall, "alpine-climbing");
            //dic.Add(GeographyType.City, "");
            dic.Add(CfType.ClimbingApproachStart, "approach");
            dic.Add(CfType.ClimbingCarPark, "car-park");
            dic.Add(CfType.CommercialIndoorClimbing, "indoor-climbing");
            //dic.Add(GeographyType.Country, "");
            dic.Add(CfType.ClimbingArea, "climbing-region");
            dic.Add(CfType.Food, "climbers-food");
            dic.Add(CfType.Guide, "climbing-guide");
            dic.Add(CfType.IceWall, "ice-climbing");
            dic.Add(CfType.PrivateIndoorClimbing, "private-climbing");
            //dic.Add(GeographyType.Province, "");
            dic.Add(CfType.Retailer, "climbing-retailer");
            dic.Add(CfType.RockBoulder, "rock-boulder");
            dic.Add(CfType.RockWall, "rock-wall");
            dic.Add(CfType.RockWaterSoloing, "deep-water-soloing");
            dic.Add(CfType.Summit, "summit-climbing");

            return dic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Dictionary<byte, string> InitializeCountryIdToUrl()
        {
            var dic = new Dictionary<byte, string>();
            var countryRepo = new cf.DataAccess.Repositories.CountryRepository();
            foreach (var c in countryRepo.GetAll()) { dic.Add(c.ID, c.NameUrlPart); }
            return dic;
        }
    }
}
