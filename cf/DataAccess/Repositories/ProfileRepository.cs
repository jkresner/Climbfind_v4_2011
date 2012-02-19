using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Profile = cf.Entities.Profile;
using cf.DataAccess.Interfaces;
using System.Data.Objects;

namespace cf.DataAccess.Repositories
{
    internal class ProfileRepository : AbstractCfEntitiesEf4DA<Profile, Guid>, IKeyEntityAccessor<Profile, Guid>,
        IKeyEntityWriter<Profile, Guid>
    {
        public ProfileRepository() : base() { }
        public ProfileRepository(string connectionStringKey) : base(connectionStringKey) { }

        internal Profile GetProfileByEmail(string email) { return Ctx.Profiles.Where(c => string.Compare(c.Email, email, true) == 0 ).SingleOrDefault(); }
        internal Profile GetProfileBySlugUrlPart(string slugUrlPart) { return Ctx.Profiles.Where(c => string.Compare(c.SlugUrlPart, slugUrlPart, true) == 0 ).SingleOrDefault(); }

        internal IEnumerable<Guid> GetFeedPlacePreferences(Guid id) {
            var parameters = new ObjectParameter[] { new ObjectParameter("UserID", id) };
            return Ctx.ExecuteFunction<Guid>("GetFeedPlacePreferences", parameters).AsQueryable(); }
    }
}
 