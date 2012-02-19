using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using cf.DataAccess.Interfaces;
using ModProfile = cf.Entities.ModProfile;

namespace cf.DataAccess.Repositories
{
    internal class ModProfileRepository : AbstractCfEntitiesEf4DA<ModProfile, Guid>, IKeyEntityAccessor<ModProfile, Guid>,
        IKeyEntityWriter<ModProfile, Guid>
    {
        public ModProfileRepository() : base() { }
        public ModProfileRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IList<ModProfile> GetObjectsModerators(Guid objID)
        {
            IEnumerable<Guid> moderatorIDs = Ctx.ModClaims.Where(p => p.ObjectID == objID).Select(p => p.UserID);

            return Ctx.ModProfiles.Where(p => moderatorIDs.Contains(p.ID)).ToList();
        }
    }
}
