using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationResultDto
    {
        public string ID { get; set; }
        public byte Type { get; set; }
        public byte Country { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Avatar { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Distance { get; set; }
        public Nullable<double> Rating { get; set; }
        public int RatingCount { get; set; }

        public LocationResultDto() { }

        public LocationResultDto(Guid id, byte type, byte country, string name, string nameShort, string avatar,
            double lat, double lon, double distance, double? rating, int ratingCount)
        {
            ID = id.ToString("N");
            Type = type;
            Country = country;
            Name = name;
            NameShort = nameShort;
            Avatar = avatar;
            Lat = lat;
            Lon = lon;
            Distance = distance;
            Rating = rating;
            RatingCount = ratingCount;
        }

        public LocationResultDto(LocationEF l)
        {
            ID = l.ID.ToString("N");
            Type = l.TypeID;
            Country = l.CountryID;
            Name = l.Name;
            NameShort = l.NameShort;
            Avatar = l.Avatar;
            Lat = l.Latitude;
            Lon = l.Longitude;
            Rating = l.Rating;
            RatingCount = l.RatingCount;
        }
    }
}
