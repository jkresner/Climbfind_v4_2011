using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Dtos;
using cf.DataAccess.Repositories;
using cf.Entities.Enum;
using cf.Caching;

namespace cf.Content.Feed.V1
{
    /// <summary>
    /// 
    /// </summary>
    internal class CheckInPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.Visit; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V1CheckIn; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v1post-ci""><label>Visited <i>{0}</i></label><p>{1}</p>{2}{3}</div>"; } }
        public string TemplateIphone { get { return @"Visited {0}<split>{1}"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "checkin-{0}-{1:MMdd}{2}"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Render(dynamic data)
        {
            var mediaThumbItems = string.Empty;
            if (!string.IsNullOrWhiteSpace(data.Media))
            {
                mediaThumbItems += "<ul class='media'>";
                foreach (var tm in (data.Media as string).Split(',')) { mediaThumbItems += string.Format(@"<li><img src=""{0}{1}""/></li>", Stgs.ImgsRt, tm); }
                mediaThumbItems += "</ul><hr />";
            }

            var climbItems = string.Empty;
            if (!string.IsNullOrWhiteSpace(data.Climbs))
            {
                climbItems += "<ul class='climbs'>";
                foreach (var c in (data.Climbs as string).Split(','))
                {
                    var bits = c.Split('|');
                    var outcome = (ClimbOutcome)byte.Parse(bits[0]);
                    var experience = (ClimbExperience)byte.Parse(bits[1]);
                    var name = bits[2];
                    climbItems += string.Format(@"<li><img src='{0}/climbed/{1}.png'><img src='{0}/climbed/{2}.png'><b>{3}</b></li>",
                        Stgs.StaticRt, outcome, experience, name);
                }
                climbItems += "</ul><hr />";
            }

            return string.Format(TemplateWeb, data.Place, data.Comment, mediaThumbItems, climbItems);
        }

        public string RenderMobile(dynamic data) { return string.Format(TemplateIphone, data.Place, data.Comment); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(CheckIn obj, CfCacheIndexEntry place)
        {
            var commentData = "No comment";
            if (!string.IsNullOrEmpty(obj.Comment))
            {
                if (obj.Comment.Length < 255) { commentData = obj.Comment; }
                else { commentData = obj.Comment.Substring(0,254) + " ..."; }
            }

            string mediaList = " ";
            if (obj.Media.Count > 0) { mediaList = ""; foreach (var m in obj.Media) { mediaList += m.ThumbUrl().Replace(Stgs.ImgsRt, "") + ","; } }

            string climbsList = " ";
            if (obj.LoggedClimbs.Count > 0) { climbsList = ""; foreach (var m in obj.LoggedClimbs.OrderBy(l=>l.Utc)) { 
                climbsList += string.Format("{0}|{1}|{2},", m.Outcome, m.Experince, SantitizeClimbName(m.ClimbName)); 
            } }

            dynamic data = new { Place = base.Sanitize(place.Name), 
                                 Comment = base.Sanitize(commentData),
                                 Media = mediaList.Substring(0, mediaList.Length - 1),
                                 Climbs = climbsList.Substring(0, climbsList.Length - 1)
                                };
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string GetDataJson(dynamic data)
        {
            if (data == null) { throw new ArgumentNullException("Cannot create template with null dynamic data"); }
            if (data.Media == null) { throw new ArgumentNullException("Cannot create template with null media data"); }
            if (data.Climbs == null) { throw new ArgumentNullException("Cannot create template with null climbs data"); }
            return "{ " + string.Format("\"Place\" : \"{0}\" , \"Comment\" : \"{1}\" , \"Media\" : \"{2}\" , \"Climbs\" : \"{3}\"", 
                data.Place, data.Comment, data.Media, data.Climbs) + " }";
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectCollection"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(dynamic objectCollection)
        {
            CheckIn ci = objectCollection.CheckIn;
            CfCacheIndexEntry place = objectCollection.Place;
            return CreateTemplateDynamicData(ci, place);
        }

        protected string SantitizeClimbName(string rawText)
        {
            return rawText.Replace(@",", "") //-- Stop our log format from becoming malformed and unable to deserialize
                    .Replace(@"\", "") //-- Stop "Unrecognized escape sequence"
                    .Replace(@"""", "'"); //-- Stop our json becoming malformed
        }


        /// <summary>
        /// Here we add an extra overload to hook into the creation on a post and make the DateTime the same as the check in
        /// </summary>
        public virtual Post CreatePost(Guid id, Guid userID, DateTime ciDateTime, CfCacheIndexEntry place, bool isPublic, dynamic data)
        {
            if (data == null) { throw new ArgumentNullException("Cannot create template with null dynamic data"); }

            var post = new Post(id, userID, place.ID, place.TypeID, isPublic);
            post.Utc = ciDateTime;
            post.TypeID = PostTypeID;
            post.TemplateKey = TemplateKey;
            post.TemplateData = GetDataJson(data);
            post.SlugUrlPart = GetPostSlugUrlPart(id, place);
            post.LastActivityUtc = ciDateTime;

            return Create(post);
        }
    }
}
