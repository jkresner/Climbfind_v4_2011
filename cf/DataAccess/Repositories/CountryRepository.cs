using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using cf.Entities;
using System.Data;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using Microsoft.SqlServer.Types;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Reader / writer repository for Country entities
    /// </summary>
    internal class CountryRepository : AbstractStoredProcedureDA<Country, byte>,
                    IKeyEntityAccessor<Country, byte>, IKeyEntityWriter<Country, byte>
    {
        protected override string DefaultConnectionStringKey { get { return "cfData"; } }

        public CountryRepository() : base() { }
        public CountryRepository(string connectionString) : base(connectionString) { }

        public IQueryable<Country> GetAll() { return GetAll("geo.GetAllCountries").OrderBy(a => a.Name); }
        public Country GetByID(byte id) { return GetByID("geo.GetCountryByID", id, SqlDbType.TinyInt, i => InflateCountryWithGeo(i) ); }

        public Country Update(Country ot)
        {
            using (SqlCommand cmd = new SqlCommand("geo.UpdateCountry"))
            {
                //-- Here we do not let users alter anything but the polygon and the threshold (for improving performance)
                cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = ot.ID;
                cmd.Parameters.Add("@GeoReduceThreshold", SqlDbType.Int).Value = ot.GeoReduceThreshold;
                cmd.Parameters.Add(new SqlParameter("@Geo", ot.Geo) { UdtTypeName = "Geography" });
                ExecuteUpdate(cmd);
            }

            return ot;
        }

        public Country Create(Country ot) { throw new Exception("Countries cannot be created!"); }
        public void Delete(Country ot) { throw new Exception("Countries cannot be deleted!"); }
        public void Delete(byte id) { throw new Exception("Countries cannot be deleted!"); }

        /// By default we query the database without the geo column because the data is very large and we do not
        /// want to hold it in things like the AppLookups cache, or read it for displaying related areas etc.
        protected override Country InflateEntityFromReader(SqlDataReader r)
        {
            return new Country()
            {
                ID = r.GetByte(0),
                Name = r.GetString(1),
                NameShort = r.GetString(2),
                NameUrlPart = r.GetString(3),
                Flag = r.GetString(4),
                Iso2 = r.GetString(5),
                Iso3 = r.GetString(6)
            };
        }

        protected Country InflateCountryWithGeo(SqlDataReader r)
        {
            return new Country()
            {
                ID = r.GetByte(0),
                Name = r.GetString(1),
                NameShort = r.GetString(2),
                NameUrlPart = r.GetString(3),
                Flag = r.GetString(4),
                Iso2 = r.GetString(5),
                Iso3 = r.GetString(6),
                Geo = r[7] as SqlGeography,
                GeoReduceThreshold = r.GetInt32(8)
            };
        }
    }

}
