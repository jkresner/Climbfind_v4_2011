using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using NetFrameworkExtensions;

namespace cf.Dtos.Mobile.V1
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationOutdoorDetailDto : LocationDetailDto
    {
        public string Cautions { get; set; }
        public string Approach { get; set; }
        public bool AccessClosed { get; set; }
        public string AccessIssues { get; set; }
        public int Altitude { get; set; }
        public bool? ShadeAfternoon { get; set; }
        public bool? ShadeMidday { get; set; }
        public bool? ShadeMorning { get; set; }

        public LocationOutdoorDetailDto() { }

        public LocationOutdoorDetailDto(cf.Entities.LocationOutdoor l) : base(l)
        {
            Cautions = l.Cautions;
            Approach = l.Approach;
            Altitude = l.Altitude;
            AccessIssues = l.AccessIssues;
            AccessClosed = l.AccessClosed;
            ShadeAfternoon = l.ShadeAfternoon;
            ShadeMidday = l.ShadeMidday;
            ShadeMorning = l.ShadeMorning;
        }
    }
}
