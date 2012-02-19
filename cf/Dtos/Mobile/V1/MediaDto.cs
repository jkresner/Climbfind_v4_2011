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
    public class MediaDto
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public byte Type { get; set; }
        public string Added { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }
        public string Content { get; set; }
        public double? Rating { get; set; }
        public int RatingCount { get; set; }

        public MediaDto() { }

        public MediaDto(Guid id, string title, byte type, DateTime added, string by, Guid byID, string byPic,
            string content, double? rating, int ratingCount)
        {
            ID = id.ToString("N");
            Title = title;
            Type = type;
            Added = added.ToEpochTimeString();
            By = by;
            ByID = byID.ToString("N");
            ByPic = byPic;
            Content = content;
            Rating = rating;
            RatingCount = ratingCount;
        }

        public MediaDto(Media m, IUserBasicDetail by)
        {
            ID = m.ID.ToString("N");
            Title = m.Title;
            Type = m.TypeID;
            Added = m.AddedUtc.ToEpochTimeString();
            By = by.DisplayName;
            ByID = m.AddedByUserID.ToString("N");
            ByPic = by.Avatar;
            Content = m.Content;
            Rating = m.Rating;
            RatingCount = m.RatingCount;
        }
    }
}
