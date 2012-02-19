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
    internal class OpinionPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.Opinion; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V0Opinion; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v0post-opinion""><label>Thinks {0}/5 about {1}</label><p>{2}</p></div>"; } }
        public string TemplateIphone { get { return @"Thinks {0}/5 about {1}<split>{2}"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "opinion-{0}-{1:MMdd}{2}"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Render(dynamic data)
        {
            var val = byte.Parse(data.Score);
            var imgString = string.Empty;
            if (val == 5) { imgString = "five"; }
            else if (val == 4) { imgString = "four"; }
            else if (val == 3) { imgString = "three"; }
            else if (val >= 2) { imgString = "two"; }
            else if (val >= 1) { imgString = "one"; }
            else { imgString = "zero"; }

            var starImg = string.Format(@"<img src=""{0}/ratings/{1}.bmp"" class=""stars"" />", Stgs.StaticRt, imgString);

            return string.Format(TemplateWeb, starImg, data.Name, data.Comment);
        }

        public string RenderMobile(dynamic data)
        {
            return string.Format(TemplateIphone, data.Score, data.Name, data.Comment);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(Opinion o, CfCacheIndexEntry obj)
        {
            var commentData = "No comment";
            if (!string.IsNullOrEmpty(o.Comment))
            {
                if (o.Comment.Length < 255) { commentData = o.Comment; }
                else { commentData = o.Comment.Excerpt(255) + " ..."; }
            }

            dynamic data = new { Name = Sanitize(obj.Name), Comment = Sanitize(commentData), Score = o.Rating };
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
            return "{ " + string.Format("\"Name\" : \"{0}\" , \"Comment\" : \"{1}\" , \"Score\" : \"{2}\"", data.Name, data.Comment, data.Score) + " }";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectCollection"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(dynamic objectCollection)
        {
            Opinion opinion = objectCollection.Opinion;
            CfCacheIndexEntry place = objectCollection.Place;
            return CreateTemplateDynamicData(opinion, place);
        }
    }
}