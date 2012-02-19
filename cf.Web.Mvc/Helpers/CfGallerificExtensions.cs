using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Mvc.Helpers
{
    public static class CfGallerificExtensions
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <remarks>The way Gallerific works it takes the url from the thumnail source...</remarks>
        public static IHtmlString RenderForCfGallerific(this HtmlHelper helper, Media m)
        {
            var htmlContent = string.Empty;

            if (m.Type == MediaType.Image) { htmlContent = m.RenderMediaHtml(); }
            else if (m.Type == MediaType.Youtube) { htmlContent = m.RenderMediaHtml(); }
            else if (m.Type == MediaType.Vimeo) { htmlContent = m.RenderMediaHtml(); }
            else { throw new NotImplementedException(m.Type.ToString() + " rendering not yet implement."); }

            return helper.Raw(htmlContent);
        }

        public static IHtmlString RenderThumbForCfGallerific(this HtmlHelper helper, Media m)
        {
            var thumbImageUrl = m.ThumbUrl();
            
            var rating = "<i>Rate me!</i>";
            if (m.Rating.HasValue)
            {
                rating = string.Format("<img src='{0}/ratings/{1}.bmp' class='st' />", Stgs.StaticRt, m.Rating.Value.GetStarsString());
            }

            return helper.Raw(string.Format(thumbFormat, blankPNG, m.Title, rating, thumbImageUrl, m.Title));
        }
        private const string thumbFormat = @"<a class=""thumb"" href=""{0}"" title=""{1}"">{2}<div><img src=""{3}"" alt=""{4}"" class=""tm"" /></div></a>";
        private const string blankPNG = "http://static.climbfind.com/ui/blank.png";
    }
}