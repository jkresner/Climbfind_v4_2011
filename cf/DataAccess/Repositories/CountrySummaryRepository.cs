using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data.SqlClient;
using cf.Entities;
using cf.Dtos;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Reader for the Country summaries (used on the places/countries page)
    /// </summary>
    internal class CountrySummaryRepository : AbstractStoredProcedureDA<CountrySummary, byte>, IKeyEntityAccessor<CountrySummary, byte>
    {
        protected override string DefaultConnectionStringKey { get { return "cfData"; } }

        public CountrySummaryRepository() : base() { }
        public CountrySummaryRepository(string connectionString) : base(connectionString) { }

        public IQueryable<CountrySummary> GetAll() { return GetAll("geo.GetGeoSummary"); }
        public CountrySummary GetByID(byte id) { throw new NotImplementedException(); }

        protected override CountrySummary InflateEntityFromReader(SqlDataReader r)
        {
            var summary = new CountrySummary() { ID = r.GetByte(0), CountryName = r.GetString(1) };

            if (r[2] != DBNull.Value) { summary.ProvinceCount = r.GetInt32(2); }
            if (r[3] != DBNull.Value) { summary.CityCount = r.GetInt32(3); }
            if (r[4] != DBNull.Value) { summary.ClimbingAreaCount = r.GetInt32(4); }
            if (r[5] != DBNull.Value) { summary.IndoorLocationCount = r.GetInt32(5); }
            if (r[6] != DBNull.Value) { summary.OutdoorLocationCount = r.GetInt32(6); }
            if (r[7] != DBNull.Value) { summary.ClimbsCount = r.GetInt32(7); }

            return summary;
        }
    }
}
