using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Entities.Enum;



namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MobileService : AbstractCfService
    {
        public MobileService() { }

        public IList<cf.Dtos.Mobile.V0.LocationResult> GetNearestLocationsV0(double lat, double lon)
        {
            return new MobileSvcRepository().GetNearestLocationsV0(lat, lon, 20);
        }

        public IList<cf.Dtos.Mobile.V1.LocationResultDto> GetNearestLocationsV1(double lat, double lon, int count)
        {
            return new MobileSvcRepository().GetNearestLocationsV1(lat, lon, count);
        }
    }
}
