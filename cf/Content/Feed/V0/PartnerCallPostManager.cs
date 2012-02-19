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
    internal class PartnerCallPostManager : AbstractPostManager, IPostManager
    {
        public override int PostTypeID { get { return (int)PostType.PartnerCall; } }
        public override string TemplateKey { get { return PostTemplateLibrary.V0PartnerCall; } }
        public byte TemplateVersion { get { return 0; } }
        public string TemplateWeb { get { return @"<div class=""v0post-pc""><label>Called out to <i>{0}</i> climbers up for {1} climbing {4} {2} {3}</label><p>{5}</p></div>"; } }
        public string TemplateIphone { get { return @"Called out to {0} climbers up for {1} climbing {4} {2} {3}<split>{5}"; } }
        public string TemplateAndriod { get { return @"None"; } }
        public override string PostSlugFormat { get { return "partnercall-{0}-{1:MMdd}{2}"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Render(dynamic data)
        {
            var indoorOutdoor = "outdoor";
            if (bool.Parse(data.Indoor) && bool.Parse(data.Outdoor)) { indoorOutdoor= "indoor/outdoor"; }
            else if (bool.Parse(data.Indoor)) { indoorOutdoor = "indoor"; }

            var ataround = "anywhere around";
            if (bool.Parse(data.IsLoc)) { ataround = "at"; }

            return string.Format(TemplateWeb, data.Level.ToLower(), indoorOutdoor, ataround, data.Place, data.Start, data.Comment);
        }

        public string RenderMobile(dynamic data)
        {
            var indoorOutdoor = "outdoor";
            if (bool.Parse(data.Indoor) && bool.Parse(data.Outdoor)) { indoorOutdoor = "indoor/outdoor"; }
            else if (bool.Parse(data.Indoor)) { indoorOutdoor = "indoor"; }

            var ataround = "anywhere around";
            if (bool.Parse(data.IsLoc)) { ataround = "at"; }

            return string.Format(TemplateIphone, data.Level.ToLower(), indoorOutdoor, ataround, data.Place, data.Start, data.Comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(PartnerCall obj, CfCacheIndexEntry place)
        {
            var commentData = "";
            if (!string.IsNullOrEmpty(obj.Comment))
            {
                if (obj.Comment.Length < 255) { commentData = obj.Comment; }
                else { commentData = obj.Comment.Excerpt(255) + " ..."; }
            }

            string level = ((ClimbingLevelGeneral)obj.PreferredLevel).ToString();

            string start = obj.StartDateTime.ToString("dd MMM HH:mm");
            if (obj.StartDateTime == obj.StartDateTime.Date) { start = obj.StartDateTime.ToString("dd MMM"); }
            
            bool isLoc = (place.Type != CfType.City && place.Type != CfType.ClimbingArea);

            dynamic data = new { Place = Sanitize(place.Name), 
                Comment = Sanitize(commentData), Start = start, Level = level, Indoor = obj.ForIndoor, Outdoor = obj.ForOutdoor, IsLoc = isLoc };
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
            return "{ " + string.Format("\"Place\" : \"{0}\" , \"Comment\" : \"{1}\" , \"Start\" : \"{2}\" , \"Level\" : \"{3}\", \"Indoor\" : \"{4}\" , \"Outdoor\" : \"{5}\" , \"IsLoc\" : \"{6}\"",
                data.Place, data.Comment, data.Start, data.Level, data.Indoor, data.Outdoor, data.IsLoc) + " }";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectCollection"></param>
        /// <returns></returns>
        public dynamic CreateTemplateDynamicData(dynamic objectCollection)
        {
            PartnerCall ci = objectCollection.PartnerCall;
            CfCacheIndexEntry place = objectCollection.Place;
            return CreateTemplateDynamicData(ci, place);
        }
    }
}