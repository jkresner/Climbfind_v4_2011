using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using LoggedClimb = cf.Entities.LoggedClimb;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// LoggedClimbsRepository
    /// </summary>
    internal class LoggedClimbsRepository : AbstractCfEntitiesEf4DA<LoggedClimb, Guid>,
        IKeyEntityAccessor<LoggedClimb, Guid>, IKeyEntityWriter<LoggedClimb, Guid>
    {
        public LoggedClimbsRepository() : base() { }
        public LoggedClimbsRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IQueryable<LoggedClimb> GetAllInclude()
        {
            return Ctx.LoggedClimbs.Include("Climb");
        }
    }
}
