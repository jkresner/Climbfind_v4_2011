using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using cf.DataAccess.EntityFramework;
using LocationIndoor = cf.Entities.LocationIndoor;
using Ef4Location = cf.Entities.LocationEF;
using cf.DataAccess.Interfaces;
using NetFrameworkExtensions;
using cf.Entities;

namespace cf.DataAccess.Repositories
{
    internal class LocationIndoorRepository : AbstractCfLocationEf4DA<LocationIndoor>, IKeyEntityAccessor<LocationIndoor, Guid>,
        IKeyEntityWriter<LocationIndoor, Guid>
    {
        public LocationIndoorRepository() : base() { }
        public LocationIndoorRepository(string connectionStringKey) : base(connectionStringKey) { }

        public override LocationIndoor Create(LocationIndoor tEntity)
        {
            return new LocationRepository().CreateLocationIndoor(tEntity);
        }

        public void AddSetter(Guid locID, Guid setterID)
        {
            Ctx.Locations.OfType<LocationIndoor>().Where(l => l.ID == locID).Single().Setters.Add(
                Ctx.Setters.Where(l => l.ID == setterID).Single());
            SaveChanges();
        }

        public void RemoveSetter(Guid locID, Guid setterID)
        {
            Ctx.Locations.OfType<LocationIndoor>().Where(l => l.ID == locID).Single().Setters.Remove(
                Ctx.Setters.Where(l => l.ID == setterID).Single());
            SaveChanges();
        }

        public override LocationIndoor Update(LocationIndoor tEntity)
        {
            tEntity.SetEmptyIfNull(e => e.Description, (e, s) => e.Description = s);
            tEntity.SetEmptyIfNull(e => e.NameShort, (e, s) => e.NameShort = s);
            tEntity.SetEmptyIfNull(e => e.SearchSupportString, (e, s) => e.SearchSupportString = s);
            tEntity.SetEmptyIfNull(e => e.Avatar, (e, s) => e.Avatar = s);
            
            //-- Use E.f. for most of the update
            ObjectSet<Ef4Location> set = Ctx.CreateObjectSet<Ef4Location>();
            LocationIndoor tEntityInDB = GetByID(tEntity.ID);
            set.ApplyCurrentValues(tEntity);
            SaveChanges();

            //-- Finish of with the geography type from the LocationRepository
            new LocationRepository().UpdateLocationGeography(tEntity.ID, tEntity.Geo);

            return tEntity;
        }
    }
}
