using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using cf.DataAccess.Interfaces;
using ModAction = cf.Entities.ModAction;

namespace cf.DataAccess.Repositories
{
    internal class ModActionRepository : AbstractCfEntitiesEf4DA<ModAction, Guid>, IKeyEntityAccessor<ModAction, Guid>,
        IKeyEntityWriter<ModAction, Guid>
    {
        public ModActionRepository() : base() { }
        public ModActionRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
