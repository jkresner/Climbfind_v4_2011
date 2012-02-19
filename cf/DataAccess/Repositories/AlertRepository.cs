using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using cf.DataAccess.Interfaces;
using Alert = cf.Entities.Alert;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Alert reader / writer
    /// </summary>
    internal class AlertRepository : AbstractCfEntitiesEf4DA<Alert, Guid>,
        IKeyEntityAccessor<Alert, Guid>, IKeyEntityWriter<Alert, Guid>
    {
        public AlertRepository() : base() { }
        public AlertRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
