using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;

namespace cf.Dtos.Mobile.V1
{
    public class AreaDetailDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Avatar { get; set; }
        public string SlugUrl { get; set; }
        public byte CountryID { get; set; }
        public byte Type { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Nullable<double> Rating { get; set; }
        public int RatingCount { get; set; }
        public List<LocationResultDto> Locations { get; set; }

        public AreaDetailDto() { }

        public AreaDetailDto(Area o)
        {
            ID = o.ID.ToString("N");
            Name = o.Name;
            NameShort = o.NameShort; 
            Avatar = o.Avatar;
            Type = o.TypeID;
            SlugUrl = o.SlugUrl;
            CountryID = o.CountryID;
            Lat = o.Latitude;
            Lon = o.Longitude;
            Rating = o.Rating;
            RatingCount = o.RatingCount;
            Locations = new List<LocationResultDto>();
        }
    }
}
