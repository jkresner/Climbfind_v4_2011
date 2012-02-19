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
using cf.Entities.Interfaces;
using NetFrameworkExtensions.Data;
using NetFrameworkExtensions;

namespace cf.DataAccess.Repositories
{
    internal class LocationRepository : AbstractStoredProcedureDA<Location, Guid>, IKeyEntityAccessor<Location, Guid>
    //-- Note there is no need to have write to this Repository because the objects are always edited by their child objects
    {
        protected override string DefaultConnectionStringKey { get { return "cfData"; } }

        public LocationRepository() : base() { }
        public LocationRepository(string connectionString) : base(connectionString) { }

        public IQueryable<Location> GetAll() { return GetAll("geo.GetAllLocations"); }

        public IQueryable<Location> GetTopLocationsOfCountry(byte id)
        {
            //**** TODO make this work for areas that span multiple countries
            using (SqlCommand cmd = new SqlCommand("geo.GetTopLocationsOfCountry"))
            {
                cmd.Parameters.Add("@ID", SqlDbType.SmallInt).Value = id;
                return GetCollection(cmd);
            }
        }

        public Location GetByID(Guid id) { return GetByID("geo.GetLocationByID", id, SqlDbType.UniqueIdentifier); }

        public List<Location> GetLocationsOfArea(Guid id)
        {
            //**** TODO make this work for areas that span multiple countries
            using (SqlCommand cmd = new SqlCommand("geo.GetLocationsOfArea"))
            {
                cmd.Parameters.Add("@AreaID", SqlDbType.UniqueIdentifier).Value = id;
                return GetCollection(cmd).OrderBy(a => a.Name).ToList();
            }
        }

        public List<Location> GetUsersIndoorPlaces(Guid id)
        {
            //**** TODO make this work for areas that span multiple countries
            using (SqlCommand cmd = new SqlCommand("usr.GetUsersIndoorPlaces"))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = id;
                return GetCollection(cmd, r => InflateIndoorForProfileFromReader(r) ).ToList();
            }
        }

        public IQueryable<Location> GetClosestLocationsOfLocation(Guid id)
        {
            var param = new SqlParameter("@LocationID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetClosestLocationsOfLocation", param);
        }

        public IQueryable<Location> GetClosestLocationsOfLocation(double latitude, double longitude)
        {
            using (SqlCommand cmd = new SqlCommand("geo.GetClosestLocationsOfPoint"))
            {
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = latitude;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = longitude;
                return GetCollection(cmd);
            }
        }

        public LocationOutdoor CreateLocationOutdoor(LocationOutdoor ot)
        {
            using (SqlCommand cmd = new SqlCommand("geo.InsertLocationOutdoor"))
            {
                AddLocationBaseParamters(cmd, ot, false);
                cmd.Parameters.Add("@Avatar", SqlDbType.NVarChar).Value = ot.Avatar.GetEmptyIfNull();
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = ot.Latitude;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = ot.Longitude;

                var newIDFromDB = ExecuteInsert(cmd);
                ot.ID = new Guid(newIDFromDB);
            }

            return ot;
        }

        public LocationIndoor CreateLocationIndoor(LocationIndoor ot)
        {
            using (SqlCommand cmd = new SqlCommand("geo.InsertLocationIndoor"))
            {
                AddLocationBaseParamters(cmd, ot, false);
                cmd.Parameters.Add("@Avatar", SqlDbType.NVarChar).Value = ot.Avatar.GetEmptyIfNull();
                cmd.Parameters.Add("@MapAddress", SqlDbType.NVarChar).Value = ot.MapAddress;
                cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = ot.Address;
                cmd.Parameters.Add("@Website", SqlDbType.NVarChar).Value = ot.Website;
                cmd.Parameters.Add("@IsPrivate", SqlDbType.Bit).Value = ot.IsPrivate;
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = ot.Latitude;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = ot.Longitude;
                
                var newIDFromDB = ExecuteInsert(cmd);
                ot.ID = new Guid(newIDFromDB);
            }

            return ot;
        }

        public void UpdateLocationGeography(Guid id, SqlGeography geo)
        {
            using (SqlCommand cmd = new SqlCommand("geo.UpdateLocationGeography"))
            {
                cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = id;
                cmd.Parameters.Add(new SqlParameter("@Geo", geo) { UdtTypeName = "Geography" });
                ExecuteUpdate(cmd);
            }
        }

        public void UpdateLocationAvatar(Guid id, string avatar)
        {
            using (SqlCommand cmd = new SqlCommand("geo.UpdateLocationAvatar"))
            {
                cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = id;
                cmd.Parameters.Add("@Avatar", SqlDbType.NVarChar).Value = avatar;
                ExecuteUpdate(cmd);
            }
        }

        private void AddLocationBaseParamters(SqlCommand cmd, ILocation ot, bool withID)
        {
            if (withID) { cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = ot.ID; }
            cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = ot.CountryID;
            cmd.Parameters.Add("@TypeID", SqlDbType.TinyInt).Value = (byte)ot.Type;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = ot.Name;
            cmd.Parameters.Add("@NameShort", SqlDbType.NVarChar).Value = ot.NameShort.GetEmptyIfNull();
            cmd.Parameters.Add("@NameUrlPart", SqlDbType.NVarChar).Value = ot.NameUrlPart;
            cmd.Parameters.Add("@SearchSupportString", SqlDbType.NVarChar).Value = ot.SearchSupportString.GetEmptyIfNull();
            cmd.Parameters.Add(new SqlParameter("@Geo", ot.Geo) { UdtTypeName = "Geography" });
        }


        protected override Location InflateEntityFromReader(SqlDataReader r)
        {
            return new Location()
            {
                ID = r.GetGuid(0),
                TypeID = r.GetByte(1),
                CountryID = r.GetByte(2),
                Name = r.GetString(3),
                NameShort = r.GetPossibleNullString(4),
                NameUrlPart = r.GetString(5),
                SearchSupportString = r.GetPossibleNullString(6),
                Description = r.GetPossibleNullString(7),
                Avatar = r.GetPossibleNullString(8),
                Latitude = r.GetDouble(9),
                Longitude = r.GetDouble(10),
                Rating = r.GetPossibleNullDouble(12),
                RatingCount = r.GetInt32(13)
            };
        }


        protected Location InflateIndoorForProfileFromReader(SqlDataReader r)
        {
            return new Location()
            {
                ID = r.GetGuid(0),
                TypeID = r.GetByte(1),
                CountryID = r.GetByte(2),
                Name = r.GetString(3),
                NameShort = r.GetPossibleNullString(4),
                NameUrlPart = r.GetString(5),
                Avatar = r.GetPossibleNullString(6)
            };
        } 
    }
}
