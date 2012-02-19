using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using CheckIn = cf.Entities.CheckIn;
using Media = cf.Entities.Media;
using cf.DataAccess.Interfaces;
using System.Data.Objects;
using cf.Entities;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// CheckIn reader / writer
    /// </summary>
    internal class CheckInRepository : AbstractCfEntitiesEf4DA<CheckIn, Guid>,
        IKeyEntityAccessor<CheckIn, Guid>, IKeyEntityWriter<CheckIn, Guid>
    {
        public CheckInRepository() : base() { }
        public CheckInRepository(string connectionStringKey) : base(connectionStringKey) { }

        public CheckIn GetCheckInByID(Guid id)
        {
            return Ctx.CheckIns.Include("Media").Include("LoggedClimbs").Where(ci => ci.ID == id).SingleOrDefault();
        }

        public IQueryable<CheckIn> GetClosestToDate(Guid locationID, DateTime checkInUtc)
        {
            var parameters = new ObjectParameter[] { new ObjectParameter("Date", checkInUtc), new ObjectParameter("LocationID", locationID) };
            return (from c in Ctx.ExecuteFunction<CheckIn>("GetClosestCheckIns", parameters) orderby c.Utc select c).AsQueryable();
        }

        public IQueryable<CheckIn> GetUsersHistory(Guid userID)
        {
            return Ctx.CheckIns
                .Include("Media")
                .Include("LoggedClimbs")
                    .Where(ci => ci.UserID == userID)
                        .OrderByDescending(ci => ci.Utc);
        }

        public void DeleteCheckIn(CheckIn checkIn)
        {
            foreach (var l in checkIn.LoggedClimbs.ToArray()) { checkIn.LoggedClimbs.Remove(l); Ctx.DeleteObject(l); }
            foreach (var l in checkIn.Media.ToArray()) { checkIn.Media.Remove(l); Ctx.DeleteObject(l); }
            
            Ctx.DeleteObject(checkIn);
            SaveChanges();
        }

        public void AddMedia(CheckIn checkIn, Media media)
        {
            Ctx.CheckIns.Where(ci => ci.ID == checkIn.ID).Single().Media.Add(Ctx.Medias.Where(m => m.ID == media.ID).Single());
            SaveChanges();
        }

        public void RemoveMedia(CheckIn checkIn, Guid mediaID)
        {
            var media = Ctx.Medias.Where(m => m.ID == mediaID).Single();
            Ctx.CheckIns.Where(ci => ci.ID == checkIn.ID).Single().Media.Remove(media);
            SaveChanges();
        }
    }
}
