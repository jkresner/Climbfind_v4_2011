using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    public partial class PartnerCall : IGuidKeyObject
    {
        public bool HasDefaultEndDate { get { return EndDateTime == StartDateTime.AddHours(5); } }

        public string IndoorOutdoorString { get {
            var indoorOutdoor = "outdoor";
            if (ForIndoor && ForOutdoor) { indoorOutdoor = "indoor/outdoor"; }
            else if (ForIndoor) { indoorOutdoor = "indoor"; }
            return indoorOutdoor;
        } }
    }
}
