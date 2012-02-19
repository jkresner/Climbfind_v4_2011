using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data;
using System.Data.SqlClient;
using cf.Entities.Enum;
using Microsoft.SqlServer.Types;
using cf.DataAccess.EntityFramework;
using UserPersonalityMedia = cf.Entities.UserPersonalityMedia;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Personality Media
    /// </summary>
    internal class UserPersonalityMediaRepository : AbstractCfEntitiesEf4DA<UserPersonalityMedia, Guid>,
        IKeyEntityAccessor<UserPersonalityMedia, Guid>, IKeyEntityWriter<UserPersonalityMedia, Guid>
    {
        public UserPersonalityMediaRepository() : base() { }
        public UserPersonalityMediaRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IQueryable<UserPersonalityMedia> GetUsersMedia(Guid userID)
        {
            var medias = (from m in Ctx.Medias.Include("UserPersonalityMedias") 
                          where m.UserPersonalityMedias.Any(um => um.UserID == userID) select m);

            List<UserPersonalityMedia> pMedias = new List<UserPersonalityMedia>();

            foreach (var m in medias)
            {
                var us = m.UserPersonalityMedias.Single();
                us.Media = m;
                pMedias.Add(us);
            }

            return pMedias.AsQueryable();
        }
    }
}
