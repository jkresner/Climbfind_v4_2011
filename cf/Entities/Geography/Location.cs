using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    public partial class Location : Place<Guid>, IPlaceSearchable, ILocation, IRatableGeo
    {
        public byte TypeID { get; set; }

        public new SqlGeography Geo { get { return SqlGeography.Point(Latitude, Longitude, 4326); } set { {;} }  }

        public bool IsIndoorClimbing
            { get { return Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing; } }

        public bool IsOutdoorClimbing
            { get { return Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing; } }

        public override CfType Type { get { return (CfType)TypeID; } }

        public override string AvatarRelativeUrl 
        { get {
            if (IsIndoorClimbing) { return "/places/id/" + Avatar; }
            else if (IsOutdoorClimbing) { return "/places/od/" + Avatar; }
            else { throw new NotImplementedException("Place type not yet supported by AvatarRelativeUrl"); }
        }
        }
    }
}
