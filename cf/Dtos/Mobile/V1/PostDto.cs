using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetFrameworkExtensions;
using cf.Entities;
using cf.Entities.Interfaces;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class PostDto
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public string PlaceID { get; set; }
        public string PlaceName { get; set; }
        public byte CountryID { get; set; }
        public string Utc { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }
        public string Meta { get; set; }
        public string Comment { get; set; }
        public List<PostCommentDto> Comments { get; set; }
        
        public PostDto() { }

        public PostDto(PostRendered p, CfCacheIndexEntry place, string by,string byPic)
        {
            ID = p.ID.ToString("N");
            PlaceID = p.PlaceID.ToString("N");
            Type = p.TypeID;
            PlaceName = place.Name;
            CountryID = place.CountryID;
            Utc = p.Utc.ToEpochTimeString();
            By = by;
            ByID = p.UserID.ToString("N");
            ByPic = byPic;
            if (p.Content.Contains("<split>"))
            {
                var split = p.Content.IndexOf("<split>");
                Meta = p.Content.Substring(0, split);
                Comment = p.Content.Substring(split + 7, p.Content.Length - (split + 7));
            }
            
            //Content = p.Content;
            Comments = new List<PostCommentDto>();
        }
    }

    public class PostCommentDto 
    { 
        public string ID { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }
        public string Msg { get; set; }
        public string Utc { get; set; }

        public PostCommentDto() { }
        public PostCommentDto(PostComment c, IUserBasicDetail p)
        {
            ID = c.ID.ToString("N");
            By = p.DisplayName;
            ByID = c.UserID.ToString("N");
            ByPic = p.Avatar;
            Msg = c.Message;
            Utc = c.Utc.ToEpochTimeString();
        }
    }
}
