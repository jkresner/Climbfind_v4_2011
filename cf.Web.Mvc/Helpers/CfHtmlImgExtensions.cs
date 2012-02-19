using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Caching;
using cf.Entities.Interfaces;

namespace cf.Web.Mvc.Helpers
{
    public static class CfHtmlImgExtensions
    {
        public static MvcHtmlString FlagImage(this HtmlHelper helper, IHasCountry obj)
        {
            if (obj.CountryID == 0 || obj.CountryID == 117) { return new MvcHtmlString(string.Format(@"<img src=""{0}/flags/nn.png"" alt=""Unknown Country"" />", Stgs.StaticRt)); }

            return FlagImage(helper, AppLookups.Country(obj.CountryID));
        }

        public static MvcHtmlString FlagImage(this HtmlHelper helper, Country country)
        {
            return new MvcHtmlString(
              string.Format(@"<img src=""{0}/flags/{1}.png"" alt=""Rock Climbing {2}"" />", Stgs.StaticRt, country.Flag, country.Name));
        }



        public static MvcHtmlString StarImage(this HtmlHelper helper, double? rating)
        {
            if (!rating.HasValue) { return new MvcHtmlString("<i>not yet rated</i>"); }

            var val = rating.Value;
            var imgString = string.Empty;
            if (val >= 5) { imgString = "five"; }
            else if (val >= 4.5) { imgString = "fournhalf"; }
            else if (val >= 4.0) { imgString = "four"; }
            else if (val >= 3.5) { imgString = "threenhalf"; }
            else if (val >= 3.0) { imgString = "three"; }
            else if (val >= 2.5) { imgString = "twonhalf"; }
            else if (val >= 2.0) { imgString = "two"; }
            else if (val >= 1.5) { imgString = "onenhalf"; }
            else if (val >= 1.0) { imgString = "one"; }
            else if (val >= 0.5) { imgString = "half"; }
            else { imgString = "zero"; }
            
            return new MvcHtmlString(
              string.Format(@"<img src=""{0}/ratings/{1}.bmp"" class=""stars"" />", Stgs.StaticRt, imgString));
        }

        public static MvcHtmlString UserPicThumb(this HtmlHelper helper, Guid userID)
        {
            if (userID == Stgs.SystemID) { return new MvcHtmlString("System"); }
            var userProfile = CfPerfCache.GetClimber(userID);
            if (userProfile == null) { return new MvcHtmlString("User deleted"); }

            return UserPicThumb(helper, userProfile.Avatar, userProfile.DisplayName);
        }

        public static string UserDisplayName(this HtmlHelper helper, Guid userID)
        {
            if (userID == Stgs.SystemID) { return "System"; }
            var userProfile = CfPerfCache.GetClimber(userID);
            if (userProfile == null) { return "User deleted"; }

            return userProfile.DisplayName;
        }

        public static MvcHtmlString UserPicThumb(this HtmlHelper helper, string imageFile, string displayName)
        {
            if (string.IsNullOrWhiteSpace(imageFile))
            {
                var blankThumbImage = string.Format(@"<img src=""http://static.climbfind.com/ui/thumb.jpg"" alt=""{0}"" class=""usr"" />", displayName);
                return new MvcHtmlString(blankThumbImage);
            }

            return new MvcHtmlString(string.Format(@"<img src=""http://images.climbfind.com/users/mainTm/{0}"" alt=""{1}"" class=""usr"" />", imageFile, displayName));
        }

        public static MvcHtmlString UserPic240(this HtmlHelper helper, string imageFile, string displayName)
        {
            if (string.IsNullOrWhiteSpace(imageFile))
            {
                var blankThumbImage = string.Format(@"<img src=""http://static.climbfind.com/ui/thumb.jpg"" alt=""{0}"" />", displayName);
                return new MvcHtmlString(blankThumbImage);
            }

            return new MvcHtmlString(string.Format(@"<img src=""http://images.climbfind.com/users/main240/{0}"" alt=""{1}"" />", imageFile, displayName));
        }

    }
}