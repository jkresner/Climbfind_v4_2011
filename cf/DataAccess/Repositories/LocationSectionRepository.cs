using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data;
using System.Data.SqlClient;
using cf.Entities.Enum;
using Microsoft.SqlServer.Types;
using cf.Entities.Interfaces;
using NetFrameworkExtensions.Data;
using NetFrameworkExtensions;
using cf.DataAccess.EntityFramework;

namespace cf.DataAccess.Repositories
{
    internal class LocationSectionRepository : AbstractCfEntitiesEf4DA<LocationSection, Guid>,
    IKeyEntityAccessor<LocationSection, Guid>, IKeyEntityWriter<LocationSection, Guid>
    {
        public LocationSectionRepository() : base() { }
        public LocationSectionRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
