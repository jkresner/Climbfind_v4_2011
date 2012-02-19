using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using System.Web.Script.Serialization;
using cf.Entities.Enum;
using Omu.ValueInjecter;
using Microsoft.SqlServer.Types;
using NetFrameworkExtensions;
using NetFrameworkExtensions.SqlServer.Types;
using cf.Dtos;

namespace cf.Entities
{
    public static partial class CfEntityExtensions
    {
        public static CfCacheIndexEntry ToCacheIndexEntry(this Place<Guid> o)
        {
            return new CfCacheIndexEntry() { ID = o.ID, CountryID = o.CountryID, Name = o.Name, NameShort = o.NameShort, TypeID = (byte)o.Type, NameUrlPart = o.NameUrlPart };
        }
        
        /// <summary>
        /// Needed for LocationIndoor & LocationOutdoor (which do not inherit from Place<Guid>)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static CfCacheIndexEntry ToCacheIndexEntry(this ILocation o)
        {
            return new CfCacheIndexEntry() { ID = o.ID, CountryID = o.CountryID, Name = o.Name, NameShort = o.NameShort, TypeID = (byte)o.Type, NameUrlPart = o.NameUrlPart };
        }
        
        public static CfCacheIndexEntry ToCacheIndexEntry(this Climb o)
        {
            return new CfCacheIndexEntry() { ID = o.ID, CountryID = o.CountryID, Name = o.Name, NameShort = "", TypeID = (byte)o.Type, NameUrlPart = o.NameUrlPart };
        }

        public static List<Climb> ToClimbList(this IEnumerable<ClimbIndoor> l)
        {
            List<Climb> list = new List<Climb>();
            foreach (var c in l) { list.Add(c); }
            return list;
        }

        public static string ShortestName(this CfCacheIndexEntry o)
        {
            string name = string.Empty;
            if (o != null) { 
                name = o.Name;
                if (!string.IsNullOrEmpty(o.NameShort)) { name = o.NameShort; }
            }
            return name;
        }
        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string CountryName(this IHasCountry entity)
        {
            return cf.Caching.AppLookups.Country(entity.CountryID).Name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="property"></param>
        /// <param name="setter"></param>
        public static void SetEmptyIfNull<T>(this T t, Func<T, string> property, Action<T, string> setter) where T : IOOObject
        {
            if (string.IsNullOrWhiteSpace(property(t))) { setter(t, String.Empty); }
        }
        
        public static T GetCloneWithGeo<T>(this T obj) where T : Place<Guid>, new()
        {
            T t = new T();
            t.InjectFrom<CloneInjection>(obj);
            t.Geo = SqlGeography.Parse(obj.Geo.GetWkt());
            return t;
        }

        /// <summary>
        /// Not sure why the TypeID is not carrying and it's causing exceptions in the Url Slug generator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetSimpleTypeClimbClone<T>(this T obj) where T : Climb, new()
        {
            T t = new T();
            t.TypeID = obj.TypeID;
            t.InjectFrom<SimpleTypeCloneInjection>(obj);
            return t;
        }

        public static T GetSimpleTypeClone<T>(this T obj) where T : IOOObject, new()
        {
            T t = new T();
            t.InjectFrom<SimpleTypeCloneInjection>(obj);
            return t;
        }

        public static string GetCategoriesString(this ICollection<ClimbTag> categories)
        {
            if (categories.Count == 0) { return "Not specified"; }
            
            var sb = new StringBuilder();
            foreach (var c in categories)
            {
                sb.Append(((ClimbTagCategory)c.Category).ToString());
                sb.Append(", ");
            }

            var len = sb.Length;
            return sb.ToString().Substring(0, len - 2);
        }

        public static List<T> RatedSample<T>(this IEnumerable<T> source, int count) where T : IRatable
        {
            List<T> ratedList = source.Where(t => t.RatingCount > 0).ToList();
            List<T> unratedList = source.Where(t => t.RatingCount == 0).ToList();

            if (ratedList.Count > count) { return ratedList.OrderByDescending(r => r.Rating).Take(count).ToList(); }
            else
            {
                List<T> mixed = ratedList.OrderByDescending(r => r.Rating).ToList();
                mixed.AddRange(unratedList.RandomSample(count - ratedList.Count));
                return mixed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>
        /// Is called on any moderator actions saving
        /// </remarks>
        public static List<string> GetCompareIgnorePropertyNames<T>(this T obj) where T : IOOObject, new()
        {
            //-- Location is necessary for avatar update
            if (typeof(T) == typeof(Location)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "Type", "TypeID", "VerboseDisplayName", "HasAvatar", "AvatarRelativeUrl" }; }
            else if (typeof(T) == typeof(LocationIndoor)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "TypeID", "VerboseDisplayName", "ShortDisplayName", "HasAvatar","HasLogo","AvatarRelativeUrl", "LogoRelativeUrl", "HasDescription", "Latitude", "Longitude" }; }
            else if (typeof(T) == typeof(LocationOutdoor)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "TypeID", "VerboseDisplayName", "ShortDisplayName", "AvatarRelativeUrl", "HasAvatar", "HasDescription", "Latitude", "Longitude" }; }
            //-- Climb is necessary on delete.
            else if (typeof(T) == typeof(Climb)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "Type", "TypeID", "VerboseDisplayName", "HasAvatar", "AvatarRelativeUrl" }; }
            else if (typeof(T) == typeof(ClimbIndoor)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "Type", "TypeID", "VerboseDisplayName", "HasAvatar", "AvatarRelativeUrl" }; }
            else if (typeof(T) == typeof(ClimbOutdoor)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "Type", "TypeID", "VerboseDisplayName", "HasAvatar", "AvatarRelativeUrl" }; }
            else if (typeof(T) == typeof(Area)) { return new List<string>() { "InitializedForSlug", "SlugUrl", "IDstring", "TypeID", "VerboseDisplayName", "MapImageRelativeUrl", "HasMapImage", "GeoReduceThreshold" }; }
            else
            {
                throw new NotImplementedException("GetCompareIgnorePropertyNames not implemented for " + typeof(T).Name);
            }
        }
    }
}
