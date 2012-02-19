//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using cf.DataAccess.EntityFramework;
//using cf.DataAccess.Interfaces;
//using LogEvent = cf.Entities.Event;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Data;

//namespace cf.DataAccess.Repositories
//{
//    /// <summary>
//    /// LogEvent reader / writer
//    /// </summary>
//    internal class LogEventRepository : AbstractCfEntitiesEf4DA<LogEvent, int>,
//        IKeyEntityAccessor<LogEvent, int>, IKeyEntityWriter<LogEvent, int>
//    {
//        public LogEventRepository() : base() { }
//        public LogEventRepository(string connectionStringKey) : base(connectionStringKey) { }

//        public void EfficientCreate(int typeID, Guid userID, string msg)
//        {
//            string ConnectionString = ConfigurationManager.ConnectionStrings["cfData"].ConnectionString;

//            using (SqlCommand cmd = new SqlCommand("log.InsertEvent"))
//            {
//                cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = typeID;
//                cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = userID;
//                cmd.Parameters.Add("@Utc", SqlDbType.DateTime).Value = DateTime.UtcNow;
//                cmd.Parameters.Add("@Message", SqlDbType.NVarChar).Value = msg;
//                using (SqlConnection dbCon = new SqlConnection(ConnectionString))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Connection = dbCon;
//                    dbCon.Open();

//                    string newID = cmd.ExecuteScalar().ToString();
//                    if (string.IsNullOrWhiteSpace(newID)) { throw new Exception("log.Event INSERT FAILED: " + cmd.CommandText); }
//                }
//            }
//        }
//    }
//}
