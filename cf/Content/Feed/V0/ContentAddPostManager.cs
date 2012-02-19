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
    internal class ContentAddPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.ContentAdd; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V0ContentAdd; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v0post-ca""><label>Added {0} <i>{1}</i></label></div>"; } }
        public string TemplateIphone { get { return @"Added {0} {1}"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "contentadded-{0}-{1:MMdd}{2}"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Render(dynamic data)
        {
            return string.Format(TemplateWeb, data.Category, data.Content);
        }

        public string RenderMobile(dynamic data)
        {
            return string.Format(TemplateIphone, data.Category, data.Content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(CfCacheIndexEntry content)
        {
            dynamic data = new { Content = Sanitize(content.Name), Category = content.Type.ToPlaceCateogry() };
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
            return "{ " + string.Format("\"Content\" : \"{0}\" , \"Category\" : \"{1}\"", data.Content, data.Category) + " }";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectCollection"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(dynamic objectCollection)
        {
            CfCacheIndexEntry content = objectCollection.Content;
            return CreateTemplateDynamicData(content);
        }
    }
}
