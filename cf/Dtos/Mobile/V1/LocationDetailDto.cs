using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;

namespace cf.Dtos.Mobile.V1
{
    public abstract class LocationDetailDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public byte CountryID { get; set; }
        public byte Type { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Nullable<double> Rating { get; set; }
        public int RatingCount { get; set; }

        public LocationDetailDto() { }

        public LocationDetailDto(LocationEF l)
        {
            ID = l.ID.ToString("N");
            Name = l.Name;
            NameShort = l.NameShort; 
            Avatar = l.Avatar;
            Type = l.TypeID;
            Description = l.Description;
            CountryID = l.CountryID;
            Lat = l.Latitude;
            Lon = l.Longitude;
            Rating = l.Rating;
            RatingCount = l.RatingCount;
        }
    }
}
