using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using PartnerCall = cf.Entities.PartnerCall;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Partner Call reader / writer
    /// </summary>
    internal class PartnerCallRepository : AbstractCfEntitiesEf4DA<PartnerCall, Guid>,
        IKeyEntityAccessor<PartnerCall, Guid>, IKeyEntityWriter<PartnerCall, Guid>
    {
        public PartnerCallRepository() : base() { }
        public PartnerCallRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
