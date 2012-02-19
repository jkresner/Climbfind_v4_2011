using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using System.Web.Script.Serialization;
using cf.Entities.Enum;
using Omu.ValueInjecter;
using Microsoft.SqlServer.Types;
using cf.Content.Images;
using cf.Dtos;

namespace cf.Entities
{
    public static partial class CfMediaExtensions
    {    
        /// <summary>
        /// Render the pure media
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<IOpinion> ToIOpinionList(this IEnumerable<MediaOpinion> list)
        {
            var iList = new List<IOpinion>();
            foreach (var o in list) { iList.Add(o); }
            return iList;
        }
        
        
        /// <summary>
        /// Render the pure media
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string RenderMediaHtml(this Media m)
        {
            var htmlContent = string.Empty;

            if (m.Type == MediaType.Image) { htmlContent = string.Format("<img src='http://images.climbfind.com/media/{0}' />", m.Content); }
            else if (m.Type == MediaType.Youtube) { htmlContent = GetYouTubeEmbedFromMediaData(m); }
            else if (m.Type == MediaType.Vimeo) { htmlContent = GetVimeoEmbedFromMediaData(m); }
            else { throw new NotImplementedException(m.Type.ToString() + " rendering not yet implement."); }

            return htmlContent;
        }

        /// <summary>
        /// Render the thumbnail for the given media
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string RenderThumb(this Media m)
        {
            return string.Format("<img src='{0}' alt='{1}' />", m.ThumbUrl(), m.Title);
        }

        public static string ThumbUrlRelative(this Media m)
        {
            return ThumbUrl(m).Replace(Stgs.ImgsRt, "");
        }

        public static string ThumbUrl(this Media m)
        {
            var thumbImageUrl = string.Empty;

            if (m.Type == MediaType.Image) { thumbImageUrl = string.Format("{0}/media/tm/{1}", Stgs.ImgsRt, m.Content); }
            else if (m.Type == MediaType.Youtube)
            {
                try
                {
                    YouTubeMediaData data = new JavaScriptSerializer().Deserialize<YouTubeMediaData>(m.Content);
                    thumbImageUrl = string.Format("{0}{1}{2}", Stgs.ImgsRt,
                        ImageManager.MediaPhotosTmPath, data.Thumbnail);
                }
                catch
                {
                    thumbImageUrl = string.Format("{0}/ui/youtube.png", Stgs.StaticRt);
                }
            }
            else if (m.Type == MediaType.Vimeo) {
                try
                {
                    VimeoMediaData data = new JavaScriptSerializer().Deserialize<VimeoMediaData>(m.Content);
                    thumbImageUrl = string.Format("{0}{1}{2}", Stgs.ImgsRt,
                        ImageManager.MediaPhotosTmPath, data.Thumbnail);
                }
                catch
                {
                    thumbImageUrl = string.Format("{0}/ui/vimeo.png", Stgs.StaticRt);
                }        
            }
            else
            {
                throw new NotImplementedException(m.Type.ToString() + " thumbnail rendering not yet implement.");
            }

            return thumbImageUrl;
        }

        public static UsersPersonalityMediaCollection ToUsersPersonalityMediaCollection(this IEnumerable<UserPersonalityMedia> usersMedia)
        {
            var pMedia = new UsersPersonalityMediaCollection();

            pMedia.Avatar = usersMedia.Where(m => m.Category == PersonalityCategory.Avatar).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.Headshot = usersMedia.Where(m => m.Category == PersonalityCategory.Headshot).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.BestShot = usersMedia.Where(m => m.Category == PersonalityCategory.BestShot).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.PartnerShot = usersMedia.Where(m => m.Category == PersonalityCategory.PartnerShot).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.Daredevil = usersMedia.Where(m => m.Category == PersonalityCategory.Daredevil).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.Funny = usersMedia.Where(m => m.Category == PersonalityCategory.Funny).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.Ready2Rock = usersMedia.Where(m => m.Category == PersonalityCategory.Ready2Rock).OrderByDescending(m => m.AddedUtc).FirstOrDefault();
            pMedia.Scenic = usersMedia.Where(m => m.Category == PersonalityCategory.Scenic).OrderByDescending(m => m.AddedUtc).FirstOrDefault();

            return pMedia;
        }


        public static string GetStarsString(this double val)
        {
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
            return imgString;
        }


        private static string GetVimeoEmbedFromMediaData(Media m)
        {
            try {
                VimeoMediaData data = new JavaScriptSerializer().Deserialize<VimeoMediaData>(m.Content);
                return string.Format(@"<iframe src=""http://player.vimeo.com/video/{0}?byline=0&amp;color=CB4721"" width=""640"" height=""360"" frameborder=""0""></iframe>", data.VimeoID);
            } catch { return "<p>Please delete this media item and add re-add it. It has been corrupted from cf updates</p>"; }
        }

        private static string GetYouTubeEmbedFromMediaData(Media m)
        {
            try {
                YouTubeMediaData data = new JavaScriptSerializer().Deserialize<YouTubeMediaData>(m.Content);
                return string.Format(@"<iframe title=""YouTube video player"" width=""640"" height=""510"" src=""http://www.youtube.com/embed/{0}?rel=0"" frameborder=""0"" allowfullscreen></iframe>", data.YouTubeID);
            } catch { return "<p>Please delete this media item and add re-add it. It has been corrupted from cf updates</p>"; }
        }
    }
}
