using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Dtos.Mobile.V0
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationResult : IKeyObject<Guid>
    {
        public Guid ID { get; set; }
        public byte Type { get; set; }
        public byte Country { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Avatar { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Distance { get; set; }

        public LocationResult() { }

        public LocationResult(Guid id, byte type, byte country, string name, string nameShort, string avatar,
            double lat, double lon, double distance)
        {
            ID = id;
            Type = type;
            Country = country;
            Name = name;
            NameShort = nameShort;
            Avatar = avatar;
            Lat = lat;
            Lon = lon;
            Distance = distance;
        }
    }
}
