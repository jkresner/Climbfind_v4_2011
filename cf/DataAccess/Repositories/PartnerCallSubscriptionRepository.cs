using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using PCSubscription = cf.Entities.PartnerCallSubscription;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// PartnerCallSubscription reader / writer
    /// </summary>
    internal class PartnerCallSubscriptionRepository : AbstractCfEntitiesEf4DA<PCSubscription, Guid>,
        IKeyEntityAccessor<PCSubscription, Guid>, IKeyEntityWriter<PCSubscription, Guid>
    {
        public PartnerCallSubscriptionRepository() : base() { }
        public PartnerCallSubscriptionRepository(string connectionStringKey) : base(connectionStringKey) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="placeIDs"></param>
        public IQueryable<PCSubscription> GetSubscriptionsForPlaces(List<Guid> placeIDs)
        {
            //-- Todo: see if there is a way we can optimize this
            return GetAll().Where( s=> placeIDs.Contains(s.PlaceID) );
        }
    }
}
