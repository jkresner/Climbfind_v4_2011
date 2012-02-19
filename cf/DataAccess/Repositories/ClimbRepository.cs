using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Climb = cf.Entities.Climb;
using cf.DataAccess.Interfaces;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using NetFrameworkExtensions.Data;
using System.Data.Objects;
using cf.Entities;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Climb reader / writer
    /// </summary>
    internal class ClimbRepository : AbstractCfEntitiesEf4DA<Climb, Guid>,
        IKeyEntityAccessor<Climb, Guid>, IKeyEntityWriter<Climb, Guid>
    {
        public ClimbRepository() : base() { }
        public ClimbRepository(string connectionStringKey) : base(connectionStringKey) { }
        
        public Climb UpdateCategories(Climb tEntity, List<int> categories)
        {
            foreach (var c in tEntity.ClimbTags.ToArray()) { tEntity.ClimbTags.Remove(c); Ctx.DeleteObject(c); }
            Ctx.DetectChanges();
            SaveChanges();
            foreach (var c in categories) { tEntity.ClimbTags.Add(new ClimbTag { ID = Guid.NewGuid(), ClimbID = tEntity.ID, Category = c }); }
            Ctx.DetectChanges();
            SaveChanges();
            return tEntity; 
        }


        public ClimbIndoor GetIndoorClimbByID(Guid ID)
        {
            return Ctx.Climbs.OfType<ClimbIndoor>().Include("ClimbTags").Include("Setter").Where(c=>c.ID ==ID).SingleOrDefault();
        }

        public IQueryable<ClimbIndoor> GetIndoorClimbsOfLocation(Guid ID) {
            return Ctx.Climbs.OfType<ClimbIndoor>().Where(c => c.LocationID == ID);
        }

        public ClimbOutdoor GetOutdoorClimbByID(Guid ID)
        {
            return Ctx.Climbs.OfType<ClimbOutdoor>().Include("ClimbTags").Include("Setter").Where(c => c.ID == ID).SingleOrDefault();
        }

        public IQueryable<ClimbOutdoor> GetOutdoorClimbsOfLocation(Guid ID)
        {
            return Ctx.Climbs.OfType<ClimbOutdoor>().Where(c => c.LocationID == ID);
        }

        public ClimbOutdoor UpdateOutdoor(ClimbOutdoor climb) { 
            var set = Ctx.CreateObjectSet<Climb>();
            var tEntityInDB = GetOutdoorClimbByID(climb.ID);
            set.ApplyCurrentValues(climb);
            SaveChanges();
            return climb;
        }

        public ClimbIndoor UpdateIndoor(ClimbIndoor climb) {
            var set = Ctx.CreateObjectSet<Climb>();
            var tEntityInDB = GetIndoorClimbByID(climb.ID);
            set.ApplyCurrentValues(climb);
            SaveChanges();
            return climb;
        }

        public override void Delete(Guid ID)
        {
            var climb = GetByID(ID);
            var cats = climb.ClimbTags.ToArray();
            foreach (var cat in cats) { Ctx.DeleteObject(cat); }
            base.Delete(ID);
        }
        
        public List<Climb> GetTopClimbsOfArea(Guid id, int count)
        {       
            var collection = new List<Climb>();
            using (SqlCommand cmd = new SqlCommand("geo.GetTopClimbsOfArea"))
            {
                cmd.Parameters.Add("@AreaID", SqlDbType.UniqueIdentifier).Value = id;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;
                using (SqlConnection dbCon = new SqlConnection(Stgs.DbConnectionString))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = dbCon;
                    dbCon.Open();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            collection.Add(new Climb()
                            {
                                ID = r.GetGuid(0),
                                CountryID = r.GetByte(1),
                                TypeID = r.GetByte(2),
                                LocationID = r.GetGuid(3),
                                SetterID = r.GetPossibleNullGuid(4),
                                Name = r.GetString(5),
                                NameUrlPart = r.GetString(6),
                                GradeLocal = r.GetPossibleNullString(7),
                                Avatar = r.GetPossibleNullString(8),
                                Rating = r.GetPossibleNullDouble(9),
                                RatingCount = r.GetInt32(10)
                            });
                        }
                    }
                }
            }
            return collection;
        }
    }
}
