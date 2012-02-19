using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using cf.Entities;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;
using cf.Entities.Enum;
using NetFrameworkExtensions;
using NetFrameworkExtensions.Data;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Reader / writer repository for Area entities
    /// </summary>
    internal class AreaRepository : AbstractStoredProcedureDA<Area, Guid>, 
        IKeyEntityAccessor<Area, Guid>, IKeyEntityWriter<Area, Guid>
    {
        protected override string DefaultConnectionStringKey { get { return "cfData"; } }

        public AreaRepository() : base() { }
        public AreaRepository(string connectionString) : base(connectionString) { }

        public IQueryable<Area> GetAll() { return GetAll("geo.GetAllAreas").OrderBy(a => a.Name); }

        public Area GetByID(Guid id) { return GetByID("geo.GetAreaByID", id, SqlDbType.UniqueIdentifier, i => InflateAreaWithGeo(i)); }

        public Area Update(Area ot)
        {
            using (SqlCommand cmd = new SqlCommand("geo.UpdateArea"))
            {
                cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = ot.ID;
                cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = ot.CountryID;
                cmd.Parameters.Add("@TypeID", SqlDbType.TinyInt).Value = (byte)ot.Type;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = ot.Name;
                cmd.Parameters.Add("@NameShort", SqlDbType.NVarChar).Value = ot.NameShort.GetEmptyIfNull();
                cmd.Parameters.Add("@NameUrlPart", SqlDbType.NVarChar).Value = ot.NameUrlPart;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = ot.Description.GetEmptyIfNull();
                cmd.Parameters.Add("@Avatar", SqlDbType.NVarChar).Value = ot.Avatar.GetEmptyIfNull();
                cmd.Parameters.Add("@SearchSupportString", SqlDbType.NVarChar).Value = ot.SearchSupportString.GetEmptyIfNull();
                cmd.Parameters.Add("@GeoReduceThreshold", SqlDbType.Int).Value = ot.GeoReduceThreshold;
                cmd.Parameters.Add("@RatingCount", SqlDbType.Int).Value = ot.RatingCount;
                cmd.Parameters.Add("@NoIndoorConfirmed", SqlDbType.Bit).Value = ot.NoIndoorConfirmed;
                cmd.Parameters.Add("@DisallowPartnerCalls", SqlDbType.Bit).Value = ot.DisallowPartnerCalls;
                cmd.Parameters.Add(new SqlParameter("@Geo", ot.Geo) { UdtTypeName = "Geography" });

                if (ot.Rating.HasValue) { cmd.Parameters.Add("@Rating", SqlDbType.Float).Value = ot.Rating; }
                else { cmd.Parameters.Add("@Rating", SqlDbType.Float).Value = DBNull.Value; }

                ExecuteUpdate(cmd);
            }

            return ot;
        }

        public void Delete(Area ot) { Delete(ot.ID); }
        public void Delete(Guid id)
        {
            using (SqlCommand cmd = new SqlCommand("geo.DeleteArea"))
            {
                cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = id;
                ExecuteUpdate(cmd);
            }
        }

        public Area Create(Area ot)
        {
            using (SqlCommand cmd = new SqlCommand("geo.InsertArea"))
            {
                cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = ot.CountryID;
                cmd.Parameters.Add("@TypeID", SqlDbType.TinyInt).Value = (byte)ot.Type;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = ot.Name;
                cmd.Parameters.Add("@NameShort", SqlDbType.NVarChar).Value = ot.NameShort.GetEmptyIfNull();
                cmd.Parameters.Add("@NameUrlPart", SqlDbType.NVarChar).Value = ot.NameUrlPart;
                cmd.Parameters.Add("@SearchSupportString", SqlDbType.NVarChar).Value = ot.SearchSupportString.GetEmptyIfNull();
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = ot.Latitude;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = ot.Longitude;
                cmd.Parameters.Add(new SqlParameter("@Geo", ot.Geo) { UdtTypeName = "Geography" });
                string newID = ExecuteInsert(cmd);
                ot.ID = new Guid(newID);
            }

            return ot;
        }


        public IList<Area> GetAreasOfCountry(byte id)
        {
            //**** TODO make this work for areas that span multiple countries
            var param = new SqlParameter("@CountryID", SqlDbType.TinyInt) { Value = id };
            return GetByForeignKey("geo.GetAreasOfCountry", param).OrderBy(a => a.Name).ToList();
        }

        public IList<Area> GetCitiesAndMajorClimbingAreasOfCountry(byte id)
        {
            //**** TODO make this work for areas that span multiple countries
            var param = new SqlParameter("@CountryID", SqlDbType.TinyInt) { Value = id };
            return GetByForeignKey("geo.GetCitiesAndMajorClimbingAreasOfCountry", param, i => InflateAreaWithGeo(i)).OrderBy(a => a.Name).ToList();
        }

        public IList<Area> GetProvincesOfCountry(byte id)
        {
            //**** TODO make this work for areas that span multiple countries
            var param = new SqlParameter("@CountryID", SqlDbType.TinyInt) { Value = id };
            return GetByForeignKey("geo.GetProvincesOfCountry", param).OrderBy(a => a.Name).ToList();
        }

        public IList<Area> GetIntersectingAreas(Guid id)
        {
            var param = new SqlParameter("@AreaID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetIntersectingAreas", param).ToList();
        }

        /// <summary>
        /// 2011.09.25 Although this method has a funny name (which should be ignored) under the hood the difference between this one
        /// and "GetIntersectingAreas" is that this one returns GEO (sqlGeograph) plus shapeArea/lengh 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<Area> GetIntersectingAreasWithGeoInflate(Guid id)
        {
            //**** TODO make this work for areas that span multiple countries
            var param = new SqlParameter("@AreaID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetProvincesIntersectingAreas", param, i => InflateAreaWithGeo(i)).OrderBy(a => a.Name).ToList();
        }

        public IList<Area> GetIntersectingAreasOfLocation(Guid id)
        {
            var param = new SqlParameter("@LocationID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetIntersectingAreasOfLocation", param).ToList();
        }

        public IQueryable<Area> GetIntersectingAreasOfPoint(double latitude, double longitude)
        {
            using (SqlCommand cmd = new SqlCommand("geo.GetIntersectingAreasOfPoint"))
            {
                cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = latitude;
                cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = longitude;
                return GetCollection(cmd);
            }
        }

        

        public IList<Area> GetRelatedAreas(Guid id)
        {
            var param = new SqlParameter("@AreaID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetRelatedAreas", param, i => InflateAreaWithGeo(i)).ToList();
        }

        public IList<Area> GetRelatedAreasOfLocation(Guid id)
        {
            var param = new SqlParameter("@LocationID", SqlDbType.UniqueIdentifier) { Value = id };
            return GetByForeignKey("geo.GetRelatedAreasOfLocation", param, i => InflateAreaWithGeo(i)).ToList();
        }

        public Area GetByCountryAndNamePartUrl(byte id, string nameUrlPart)
        {
            //**** TODO make this work for areas that span multiple countries
            //**** Make sure a unique index exists
            using (SqlCommand cmd = new SqlCommand("geo.GetArea"))
            {
                cmd.Parameters.Add("@NameUrlPart", SqlDbType.NChar).Value = nameUrlPart;
                //-- Area spans multiple countries
                if (id == 0) { cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = DBNull.Value; }
                else { cmd.Parameters.Add("@CountryID", SqlDbType.TinyInt).Value = id; }

                return GetObject(cmd, i => InflateAreaWithGeo(i));
            }
        }


        //public IQueryable<Dictionary<byte, List<Area>>> GetReducedProvincesOfCountries() 
        //{
        //    var dic = new Dictionary<byte, List<Area>>();
        //    var reduce = 2000;

        //    using (SqlConnection dbCon = new SqlConnection(ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("geo.GetAllReducedProvinces"))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Connection = dbCon;
        //            dbCon.Open();

        //            using (SqlDataReader r = cmd.ExecuteReader())
        //            {
        //                while (r.Read()) 
        //                { 

        //                    collection.Add(inflateDelegate(r)); 
        //                }
        //            }
        //        }
        //    }

        //    return dic; 
        //}

        /// <summary>
        /// By default we query the database without the geo column because the data is very large and we do not
        /// want to hold it in things like the AppLookups cache, or read it for displaying related areas etc.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected override Area InflateEntityFromReader(SqlDataReader r)
        {
            return new Area
            {
                ID = r.GetGuid(0),
                TypeID = r.GetByte(1),
                CountryID = r.GetByte(2),
                Name = r.GetString(3),
                NameShort = r.GetString(4),
                NameUrlPart = r.GetString(5),
                SearchSupportString = r.GetString(6),
                Description = r.GetPossibleNullString(7),
                Avatar = r.GetPossibleNullString(8),
                Latitude = r.GetDouble(9),
                Longitude = r.GetDouble(10),
                Rating = r.GetPossibleNullDouble(11),
                RatingCount = r.GetInt32(12),
                NoIndoorConfirmed = r.GetBoolean(13),
                DisallowPartnerCalls = r.GetBoolean(14)
            };
        }

        /// <summary>
        /// Additional inflate with geo column
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected Area InflateAreaWithGeo(SqlDataReader r)
        {
            return new Area
            {
                ID = r.GetGuid(0),
                TypeID = r.GetByte(1),
                CountryID = r.GetByte(2),
                Name = r.GetString(3),
                NameShort = r.GetString(4),
                NameUrlPart = r.GetString(5),
                SearchSupportString = r.GetString(6),
                Description = r.GetPossibleNullString(7),
                Avatar = r.GetPossibleNullString(8),
                Latitude = r.GetDouble(9),
                Longitude = r.GetDouble(10),
                Geo = r[11] as SqlGeography,
                GeoReduceThreshold = r.GetInt32(12),
                ShapeArea = r.GetDouble(14),
                Rating = r.GetPossibleNullDouble(15),
                RatingCount = r.GetInt32(16),
                NoIndoorConfirmed = r.GetBoolean(17),
                DisallowPartnerCalls = r.GetBoolean(18)
            };
        }
    }
}
