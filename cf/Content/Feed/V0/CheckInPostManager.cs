using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Dtos;
using cf.DataAccess.Repositories;
using cf.Entities.Enum;
using NetFrameworkExtensions;

namespace cf.Content.Feed.V0
{
    /// <summary>
    /// 
    /// </summary>
    internal class CheckInPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.Visit; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V0CheckIn; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v0post-ci""><label>Checked in to <i>{0}</i></label><p>{1}</p></div>"; } }
        public string TemplateIphone { get { return @"None"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "checkin-{0}-{1:MMdd}{2}"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Render(dynamic data)
        {
            return string.Format(TemplateWeb, data.Place, data.Comment);
        }

        public string RenderMobile(dynamic data) { return ""; }

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
                else { commentData = obj.Comment.Excerpt(255) + " ..."; }
            }

            dynamic data = new { Place = place.Name, Comment = commentData };
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
            return "{ " + string.Format("\"Place\" : \"{0}\" , \"Comment\" : \"{1}\"", data.Place, data.Comment) + " }";
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
    }
}
