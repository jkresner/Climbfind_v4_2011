using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;

namespace cf.Web.Mvc.Helpers
{
    public static class CfSocialExtensions
    {
        public static MvcHtmlString FacebookShare(this HtmlHelper helper)
        {
            var domain = "http://www.climbfind.com";
            var pageUrl = HttpContext.Current.Request.Url.AbsolutePath.ToString();
            var pageUrlEncoded = HttpUtility.UrlEncode(domain + pageUrl);
            var iframe = string.Format(@"<iframe src=""http://www.facebook.com/plugins/like.php?href={0}&amp;send=true&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font&amp;height=21"" scrolling=""no"" frameborder=""0"" style=""border:none; overflow:hidden; width:450px; height:21px;"" allowTransparency=""true""></iframe>", pageUrlEncoded);
            return new MvcHtmlString(iframe);
        }

        const string addThisMediaHtmlFormat = @"<div class=""addthis_toolbox addthis_default_style addthis_32x32_style"" addthis:title=""{0}"" addthis:url=""{1}"">
                    <a class=""addthis_button_preferred_1""></a>
                    <a class=""addthis_button_preferred_2""></a>
                    <a class=""addthis_button_compact""></a>
                    <a class=""addthis_counter addthis_bubble_style""></a>
                </div>";

        const string addThisMediaScirpt = @"<script type=""text/javascript""> var addthis_config = { ""data_track_clickback"": true };</script>
            <script type=""text/javascript"" src=""http://s7.addthis.com/js/250/addthis_widget.js#pubid=ra-4df9436f251af994""></script>";

        public static MvcHtmlString AddThisPersonalityMediaShare(this HtmlHelper helper, UserPersonalityMedia media, bool includeScriptReference)
        {
            string url = string.Format("{0}/personality-media/{1}", Stgs.WebRt, media.ID);
            
            return AddThisMediaShare(helper, url, media.Media.Title, includeScriptReference);
        }

        public static MvcHtmlString AddThisMediaShare(this HtmlHelper helper, string url, string title, bool includeScriptReference)
        {
            var addThis = string.Format(addThisMediaHtmlFormat, title, url);

            if (includeScriptReference) { addThis += addThisMediaScirpt; }

            return new MvcHtmlString(addThis);
        }
    }
}