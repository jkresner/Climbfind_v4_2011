using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Interfaces;
using cf.Entities.Interfaces;
using System.Data.Objects;
using Ef4Location = cf.Entities.LocationEF;
using cf.DataAccess.Repositories;
using System.Data.EntityClient;

namespace cf.DataAccess.EntityFramework
{
    /// <summary>
    /// An abstract data access class for all our Location entities (LocationIndoor, LocationOutdoor, LocationPOI, LocationBusiness) since
    /// all our locations belong to the one entity set (Locations)
    /// </summary>
    /// <typeparam name="TEntity">Location type</typeparam>
    public abstract class AbstractCfLocationEf4DA<TEntity> : AbstractCfEntitiesEf4DA<TEntity, Guid>
        where TEntity : class, ILocation, IKeyObject<Guid>, new()
    {
        public AbstractCfLocationEf4DA() : base() { }
        public AbstractCfLocationEf4DA(string connectionStringKey) : base(connectionStringKey) { }
        public AbstractCfLocationEf4DA(EntityConnection connection) : base(connection) { }

        public override IQueryable<TEntity> GetAll() { return Ctx.Locations.OfType<TEntity>(); }
        public override TEntity GetByID(Guid ID) { return Ctx.Locations.OfType<TEntity>().Where(e => e.ID.Equals(ID)).SingleOrDefault(); }

        /// <summary>
        /// Override base Delete because we cannot create an object set based on our TEntity, because the set is the same
        /// for all our different locations types
        /// </summary>
        /// <param name="ID"></param>
        public override void Delete(Guid ID)
        {
            ObjectSet<Ef4Location> set = Ctx.CreateObjectSet<Ef4Location>();
            set.DeleteObject(set.Where(entity => entity.ID.Equals(ID)).Single());
            SaveChanges();
        }

        /// <summary>
        /// Since all or locations can be accessed through their unique country / url part pair we can implement this get 
        /// in this base class
        /// </summary>
        /// <param name="countryID"></param>
        /// <param name="nameUrlPart"></param>
        /// <returns></returns>
        public TEntity GetByCountryAndNameUrlPart(byte countryID, string nameUrlPart)
        {
            return Ctx.Locations.OfType<TEntity>()
                .Where(c => c.CountryID == countryID && c.NameUrlPart == nameUrlPart).SingleOrDefault();
        }
    }
}
