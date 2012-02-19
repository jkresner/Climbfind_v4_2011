using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos.Mobile.V0
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// NOTE IF YOU CHANGE THIS CLASS YOU WILL PROBABLY BREAK THE IPHONE APPLICATION!!!!!!!
    /// </remarks>
    public class MediaDto
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public byte Type { get; set; }
        public DateTime Added { get; set; }
        public string By { get; set; }
        public Guid ByID { get; set; }
        public string ByPic { get; set; }
        public string Content { get; set; }
        public double? Rating { get; set; }
        public int RatingCount { get; set; }

        public MediaDto() { }

        public MediaDto(Guid id, string title, byte type, DateTime added, string by, Guid byID, string byPic,
            string content, double? rating, int ratingCount)
        {
            ID = id;
            Title = title;
            Type = type;
            Added = added;
            By = by;
            ByID = byID;
            ByPic = byPic;
            Content = content;
            Rating = rating;
            RatingCount = ratingCount;
        }
    }
}
