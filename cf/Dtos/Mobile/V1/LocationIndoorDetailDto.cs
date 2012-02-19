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
    public class LocationIndoorDetailDto : LocationDetailDto
    {
        public string Address { get; set; }
        public string Logo { get; set; }
        public bool TopRope { get; set; }
        public bool Boulder { get; set; }
        public bool Lead { get; set; }
        
        public LocationIndoorDetailDto() { }

        public LocationIndoorDetailDto(cf.Entities.LocationIndoor l) : base(l)
        {
            Address = l.Address;
            Logo = l.Logo;
            TopRope = l.HasTopRope;
            Boulder = l.HasBoulder;
            Lead = l.HasLead;      
        }
    }
}
