using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using cf.DataAccess.Interfaces;
using cf.Entities.Interfaces;
using ObjectModMeta = cf.Entities.ObjectModMeta;

namespace cf.DataAccess.Repositories
{
    internal class ObjectModMetaRepository : AbstractCfEntitiesEf4DA<ObjectModMeta, Guid>, IKeyEntityAccessor<ObjectModMeta, Guid>,
        IKeyEntityWriter<ObjectModMeta, Guid>
    {
        public ObjectModMetaRepository() : base() { }
        public ObjectModMetaRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IList<ObjectModMeta> GetModeratorsObjects(Guid modUserID)
        {
            List<Guid> placeIDs = Ctx.ModClaims.Where(p => p.UserID == modUserID).Select(p=>p.ObjectID).ToList();

            return Ctx.ObjectModMetas.Where(p => placeIDs.Contains(p.ID)).ToList();
        }

        /// <summary>
        /// Claim an area / indoor loc / outdoor loc / climb / cPOI etc.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modUserID"></param>
        /// <returns></returns>
        public ObjectModMeta ClaimObject(ISearchable obj, Guid modUserID)
        {
            Guid objID = new Guid(obj.IDstring);

            Ctx.ModClaims.AddObject(new Entities.ModClaim() { ID = Guid.NewGuid(), ObjectID = objID, UserID = modUserID });
            Ctx.SaveChanges();

            return Ctx.ObjectModMetas.Where(p => p.ID == objID).Single();
        }

        /// <summary>
        /// Un claim an area / indoor loc / outdoor loc / climb / cPOI etc.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modUserID"></param>
        public void UnclaimObject(ISearchable obj, Guid modUserID)
        {
            Guid objID = new Guid(obj.IDstring);

            Ctx.ModClaims.DeleteObject(Ctx.ModClaims.Where(p => p.ObjectID == objID && p.UserID == modUserID).Single());
            Ctx.SaveChanges();
        }
    }
}
