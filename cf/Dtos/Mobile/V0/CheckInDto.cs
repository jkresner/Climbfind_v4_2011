using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos.Mobile.V0
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckInDto
    {
        public Guid ID { get; set; }
        public Guid LocationID { get; set; }
        public DateTime DateTime { get; set; }
        public string By { get; set; }
        public Guid ByID { get; set; }
        public string ByPic { get; set; }
        public string Comment { get; set; }

        public CheckInDto() { }

        public CheckInDto(Guid id, Guid locationID, DateTime dateTime, string by, Guid byID, string byPic,
            string comment)
        {
            ID = id;
            LocationID = locationID;
            DateTime = dateTime;
            By = by;
            ByID = byID;
            ByPic = byPic;
            Comment = comment;
        }
    }
}
