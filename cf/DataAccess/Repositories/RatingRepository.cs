using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Opinion = cf.Entities.Opinion;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// CheckIn reader / writer
    /// </summary>
    internal class OpinionRepository : AbstractCfEntitiesEf4DA<Opinion, Guid>,
        IKeyEntityAccessor<Opinion, Guid>, IKeyEntityWriter<Opinion, Guid>
    {
        public OpinionRepository() : base() { }
        public OpinionRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
