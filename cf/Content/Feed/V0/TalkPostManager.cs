using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Dtos;
using cf.DataAccess.Repositories;
using cf.Entities.Enum;

namespace cf.Content.Feed.V0
{
    /// <summary>
    /// 
    /// </summary>
    internal class TalkPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.Talk; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V0Talk; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v0post-talk""><label>Said to {0} climbers</label><p>{1}</p></div>"; } }
        public string TemplateIphone { get { return @"None"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "talk-{0}-{1:MMdd}{2}"; } }

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
        /// <param name="ci"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(string comment, CfCacheIndexEntry place)
        {
            dynamic data = new { Place = Sanitize(place.Name), Comment = Sanitize(comment) };
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
            string comment = objectCollection.Comment;
            CfCacheIndexEntry place = objectCollection.Place;
            return CreateTemplateDynamicData(comment, place);
        }
    }
}
