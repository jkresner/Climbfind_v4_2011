using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using cf.DataAccess.Interfaces;
using UserSiteSettings = cf.Entities.UserSiteSettings;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Alert reader / writer
    /// </summary>
    internal class UserSiteSettingsRepository : AbstractCfEntitiesEf4DA<UserSiteSettings, Guid>,
        IKeyEntityAccessor<UserSiteSettings, Guid>, IKeyEntityWriter<UserSiteSettings, Guid>
    {
        public UserSiteSettingsRepository() : base() { }
        public UserSiteSettingsRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
