using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Entities
{
    public class Country : Place<byte>, IArea, IPlaceSearchable
    {
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Flag { get; set; }
        public int GeoReduceThreshold { get; set; }
        
        public override CfType Type { get { return CfType.Country; } }

        public override string AvatarRelativeUrl { get { throw new NotImplementedException(); } }

        public new byte CountryID { get { return ID; } set { ;} }
        
        /// <summary>
        /// It's very important we override this property as otherwise the child class is used which does not pick up on our overridden CountryID
        /// </summary>
        public new bool InitializedForSlug
        {
            get
            {
                return !string.IsNullOrWhiteSpace(NameUrlPart) && CountryID != 0;
            }
        }
    }
}
