using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using PartnerCallNotificationWorkItem = cf.Entities.PartnerCallNotificationWorkItem;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Partner Call reader / writer
    /// </summary>
    internal class PartnerCallNotificationWorkItemRepository : AbstractCfEntitiesEf4DA<PartnerCallNotificationWorkItem, Guid>,
        IKeyEntityAccessor<PartnerCallNotificationWorkItem, Guid>, IKeyEntityWriter<PartnerCallNotificationWorkItem, Guid>
    {
        public PartnerCallNotificationWorkItemRepository() : base() { }
        public PartnerCallNotificationWorkItemRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
