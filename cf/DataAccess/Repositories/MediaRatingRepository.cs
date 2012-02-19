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
using MediaOpinion = cf.Entities.MediaOpinion;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Media Repository
    /// </summary>
    internal class MediaOpinionRepository : AbstractCfEntitiesEf4DA<MediaOpinion, Guid>,
        IKeyEntityAccessor<MediaOpinion, Guid>, IKeyEntityWriter<MediaOpinion, Guid>
    {
        public MediaOpinionRepository() : base() { }
        public MediaOpinionRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
