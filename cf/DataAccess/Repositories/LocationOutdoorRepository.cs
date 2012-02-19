using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using cf.DataAccess.EntityFramework;
using cf.Entities;
using LocationOutdoor = cf.Entities.LocationOutdoor;
using Ef4Location = cf.Entities.LocationEF;
using cf.DataAccess.Interfaces;
using NetFrameworkExtensions;

namespace cf.DataAccess.Repositories
{
    internal class LocationOutdoorRepository : AbstractCfLocationEf4DA<LocationOutdoor>, IKeyEntityAccessor<LocationOutdoor, Guid>,
        IKeyEntityWriter<LocationOutdoor, Guid>
    {
        public LocationOutdoorRepository() : base() { }
        public LocationOutdoorRepository(string connectionStringKey) : base(connectionStringKey) { }

        public override LocationOutdoor Create(LocationOutdoor tEntity)
        {
            return new LocationRepository().CreateLocationOutdoor(tEntity);
        }

        public override LocationOutdoor Update(LocationOutdoor tEntity)
        {
            tEntity.SetEmptyIfNull(e => e.Description, (e, s) => e.Description = s);
            tEntity.SetEmptyIfNull(e => e.NameShort, (e, s) => e.NameShort = s);
            tEntity.SetEmptyIfNull(e => e.SearchSupportString, (e, s) => e.SearchSupportString = s);
            tEntity.SetEmptyIfNull(e => e.Avatar, (e, s) => e.Avatar = s);
            
            //-- Use E.f. for most of the update
            ObjectSet<Ef4Location> set = Ctx.CreateObjectSet<Ef4Location>();
            LocationOutdoor tEntityInDB = GetByID(tEntity.ID);
            set.ApplyCurrentValues(tEntity);
            SaveChanges();

            //-- Finish of with the geography type from the LocationRepository
            new LocationRepository().UpdateLocationGeography(tEntity.ID, tEntity.Geo);

            return tEntity;
        }
    }
}
