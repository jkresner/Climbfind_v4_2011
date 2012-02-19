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
    public class OpinionDto
    {
        public string ID { get; set; }
        public byte Rating { get; set; }
        public string Utc { get; set; }
        public string By { get; set; }
        public string ByID { get; set; }
        public string ByPic { get; set; }
        public string Comment { get; set; }
        public string OnName { get; set; }

        public OpinionDto() { }

        public OpinionDto(Guid id, byte rating, DateTime utc, string by, Guid byID, string byPic, string comment)
        {
            ID = id.ToString("N");
            Rating = rating;
            Utc = utc.ToEpochTimeString();
            By = by;
            ByID = byID.ToString("N");
            ByPic = byPic;
            Comment = comment;
        }

        public OpinionDto(Opinion o, IUserBasicDetail by)
        {
            ID = o.ID.ToString("N");
            Rating = o.Rating;
            Utc = o.Utc.ToEpochTimeString();
            By = by.DisplayName;
            ByID = by.ID.ToString("N");
            ByPic = by.Avatar;
            Comment = o.Comment;
        }
    }
}
