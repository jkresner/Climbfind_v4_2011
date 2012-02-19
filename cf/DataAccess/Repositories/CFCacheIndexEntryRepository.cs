using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data.SqlClient;
using cf.Dtos;

namespace cf.DataAccess.Repositories
{
    public class CFCacheIndexEntryRepository : AbstractStoredProcedureDA<CfCacheIndexEntry, Guid>, IKeyEntityAccessor<CfCacheIndexEntry, Guid>
    {
        protected override string DefaultConnectionStringKey { get { return "cfData"; } }

        public CFCacheIndexEntryRepository() : base() { }
        public CFCacheIndexEntryRepository(string connectionString) : base(connectionString) { }

        public IQueryable<CfCacheIndexEntry> GetAll() { return GetAll("geo.GetCfCacheIndex"); }
        public CfCacheIndexEntry GetByID(Guid id) { throw new NotImplementedException("We only read the whole place index at a time"); }

        protected override CfCacheIndexEntry InflateEntityFromReader(SqlDataReader r)
        {
            return new CfCacheIndexEntry() 
            { 
                ID = r.GetGuid(0), 
                Name = r.GetString(1),
                NameShort = r.GetString(2),
                NameUrlPart = r.GetString(3),
                CountryID = r.GetByte(4),
                TypeID = r.GetByte(5),
            };
        }
    }
}
