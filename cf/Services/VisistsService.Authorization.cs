using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VisitsService
    {
        public IList<Climb> LogClimbAuthorize(CheckIn checkIn, LoggedClimb log)
        {
            //-- (JSK 2011:09.11) Don't think it makes sense to cache this:
            //var validClimbs = PerfCache.GetClimbsForCheckIn(checkIn.LocationID, checkIn.Utc);

            var validClimbs = new GeoService().GetClimbsOfLocationForLogging(checkIn.LocationID, checkIn.Utc);

            if (checkIn.UserID != CfIdentity.UserID)
            {
                var error = string.Format("Cannot log climbs for CheckIn with ID[{0}] because it does not belong to the current logged in user[{1}]", checkIn.ID, CfIdentity.UserID);
                throw new ArgumentException(error);
            }

            try
            {
                ClimbExperience experience = (ClimbExperience)log.Experince;
                ClimbGradeOpinion oppinion = (ClimbGradeOpinion)log.GradeOpinion;
                ClimbOutcome outcome = (ClimbOutcome)log.Outcome;
            }
            catch (Exception ex) { throw new ArgumentException(ex.Message); }

            if (log.Rating < 0 || log.Rating > 5) { throw new ArgumentException("Rating must be between 0 and 5."); }

            var climb = validClimbs.Where(c => c.ID == log.ClimbID).SingleOrDefault();
            if (climb == default(Climb))
            {
                var error = string.Format("Climb with ID[{0}] is not a valid climb to log @ {1} on {2}", log.ClimbID,
                    AppLookups.GetCacheIndexEntry(checkIn.LocationID).Name, checkIn.Utc);
                throw new ArgumentException(error);
            }

            return validClimbs;
        }

    }
}
