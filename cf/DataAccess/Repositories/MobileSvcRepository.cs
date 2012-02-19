using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data.SqlClient;
using cf.Entities;
using System.Data;
using System.Configuration;
using cf.Dtos;
using NetFrameworkExtensions.Data;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Custom lightweight super fast data access
    /// </summary>
    internal class MobileSvcRepository
    {
        protected string ConnectionString { get; set; }

        public MobileSvcRepository() { ConnectionString = ConfigurationManager.ConnectionStrings["cfData"].ConnectionString; }

        public IList<cf.Dtos.Mobile.V0.LocationResult> GetNearestLocationsV0(double lat, double lon, int count) 
        {
            var collection = new List<cf.Dtos.Mobile.V0.LocationResult>();
            using (SqlCommand cmd = new SqlCommand("mob.GetNearestLocations"))
            {
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = lat;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = lon;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;
                using (SqlConnection dbCon = new SqlConnection(ConnectionString))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = dbCon;
                    dbCon.Open();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            collection.Add(new cf.Dtos.Mobile.V0.LocationResult() {
                                ID = r.GetGuid(0),
                                Type = r.GetByte(1),
                                Country = r.GetByte(2),
                                Name = r.GetString(3),
                                NameShort = r.GetString(4),
                                Avatar = r.GetString(5),
                                Lat = r.GetDouble(6),
                                Lon = r.GetDouble(7),
                                Distance = Math.Round(r.GetDouble(8), 1)
                            });
                        }
                    }
                }
            }   
            return collection; 
        }

        public IList<cf.Dtos.Mobile.V1.LocationResultDto> GetNearestLocationsV1(double lat, double lon, int count)
        {
            var collection = new List<cf.Dtos.Mobile.V1.LocationResultDto>();
            using (SqlCommand cmd = new SqlCommand("mob.GetNearestLocations"))
            {
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = lat;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = lon;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;
                using (SqlConnection dbCon = new SqlConnection(ConnectionString))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = dbCon;
                    dbCon.Open();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            collection.Add(new cf.Dtos.Mobile.V1.LocationResultDto()
                            {
                                ID = r.GetGuid(0).ToString("N"),
                                Type = r.GetByte(1),
                                Country = r.GetByte(2),
                                Name = r.GetString(3),
                                NameShort = r.GetString(4),
                                Avatar = r.GetString(5),
                                Lat = r.GetDouble(6),
                                Lon = r.GetDouble(7),
                                Distance = Math.Round(r.GetDouble(8), 1),
                                Rating = r.GetPossibleNullDouble(9),
                                RatingCount = r.GetInt32(10),
                            });
                        }
                    }
                }
            }
            return collection;
        }
    }
}
