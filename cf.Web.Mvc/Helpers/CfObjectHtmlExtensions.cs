using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Caching;
using cf.Content;
using cf.Entities.Interfaces;
using System.Text;

namespace cf.Web.Mvc.Helpers
{
    public static class CfObjectHtmlExtensions
    {
        public static MvcHtmlString CfLink(this HtmlHelper helper, Guid objID)
        {
            var cachedObj = AppLookups.GetCacheIndexEntry(objID);
            if (cachedObj != null)
            { 
                return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", cachedObj.SlugUrl, cachedObj.Name));    
            }
            else
            {
                var userProfile = CfPerfCache.GetClimber(objID);
                if (userProfile != null)
                {
                    return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", userProfile.SlugUrl, userProfile.DisplayName));
                }
            }
       
            return new MvcHtmlString("");
        }

        public static MvcHtmlString UserProfileLink(this HtmlHelper helper, Guid userID)
        {
            if (userID == Stgs.SystemID) { return new MvcHtmlString("System"); }
            
            var userProfile = CfPerfCache.GetClimber(userID);
            if (userProfile == null) { return new MvcHtmlString("User deleted"); }

            return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", userProfile.SlugUrl, userProfile.DisplayName));
        }

        public static MvcHtmlString ModProfileLink(this HtmlHelper helper, Guid userID)
        {
            if (userID == Stgs.SystemID) { return new MvcHtmlString("System"); }
            
            var userProfile = CfPerfCache.GetClimber(userID);
            return new MvcHtmlString(string.Format(@"<a href=""/Moderate/ActionUserList/{0}"">{1}</a>",
                userID, userProfile.DisplayName));
        }

        public static MvcHtmlString PlaceLink(this HtmlHelper helper, Guid placeID)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            if (place == null) { return new MvcHtmlString(""); }

            return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", place.SlugUrl, place.Name));
        }

        public static MvcHtmlString PlaceLinkShortName(this HtmlHelper helper, Guid placeID)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            if (place == null) { return new MvcHtmlString(""); }

            var name = place.Name;
            if (!string.IsNullOrEmpty(place.NameShort)) { name = place.NameShort; }

            return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", place.SlugUrl, name));
        }

        public static MvcHtmlString PlaceLinkWithBlank(this HtmlHelper helper, Guid placeID)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);
            if (place == null && placeID != Guid.Empty) { return new MvcHtmlString("<i>item deleted</i>"); }
            else if (place == null) { return new MvcHtmlString(""); }

            return new MvcHtmlString(string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>", place.SlugUrl, place.Name));
        }

        public static MvcHtmlString PlaceLinkWithFlag(this HtmlHelper helper, Guid placeID)
        {
            var place = AppLookups.GetCacheIndexEntry(placeID);

            if (place == null) { return new MvcHtmlString(""); }

            var country = AppLookups.Country(place.CountryID);
            
            return new MvcHtmlString(
                  string.Format(@"<img src=""{0}/flags/{1}.png"" /> <a href=""{2}"">{3}</a>",
                    Stgs.StaticRt, country.Flag, place.SlugUrl, place.Name));  
        }

        public static MvcHtmlString PlaceLinkList<T>(this HtmlHelper helper, List<T> places) where T : IPlaceWithGeo
        {
            if (places.Count == 0) { return new MvcHtmlString(""); }
            if (places.Count == 1) { return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", places[0].SlugUrl, places[0].Name)); }
            else
            {
                StringBuilder sb = new StringBuilder();
                var i = 0;
                var lastIndex = places.Count - 1;
                foreach (var p in places)
                {
                    if (i == 0) { sb.AppendFormat(@"<a href=""{0}"">{1}</a>", p.SlugUrl, p.Name); }
                    else if (i == lastIndex) { sb.AppendFormat(@" & <a href=""{0}"">{1}</a>", p.SlugUrl, p.Name); }
                    else { sb.AppendFormat(@", <a href=""{0}"">{1}</a>", p.SlugUrl, p.Name); }
                    i++;
                }
                
                return new MvcHtmlString(sb.ToString());
            }
        }


        public static MvcHtmlString PlaceList<T>(this HtmlHelper helper, List<T> places) where T : IPlaceWithGeo
        {
            if (places.Count == 0) { return new MvcHtmlString(""); }
            if (places.Count == 1) { return new MvcHtmlString(places[0].Name); }
            else
            {
                StringBuilder sb = new StringBuilder();
                var i = 0;
                var lastIndex = places.Count - 1;
                foreach (var p in places)
                {
                    if (i == 0) { sb.AppendFormat(p.Name); }
                    else if (i == lastIndex) { sb.AppendFormat(@" & {0}", p.Name); }
                    else { sb.AppendFormat(@", {0}", p.Name); }
                    i++;
                }

                return new MvcHtmlString(sb.ToString());
            }
        }
    }
}