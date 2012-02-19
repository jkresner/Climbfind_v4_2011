using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using cf.DataAccess.Interfaces;
using System.Data.EntityClient;

namespace cf.DataAccess.AdoNet2
{
    /// <summary>
    /// Shared base functionality for executing stored procedures via .net2 / ado .net 2 style code
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the Data Access Class</typeparam>
    /// <typeparam name="KeyType">The key type of the the entity type</typeparam>
    public abstract class AbstractStoredProcedureDA<TEntity, KeyType> where TEntity : new()
    {
        protected abstract string DefaultConnectionStringKey { get; }
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Default constructor setting the connection string to be the one specified from .config with the Default Connection
        /// String in the child class
        /// </summary>
        public AbstractStoredProcedureDA()
        {
            ConnectionString = Stgs.DbConnectionString;
        }

        /// <summary>
        /// Constructor allowing the connection string to be injected
        /// </summary>
        /// <param name="connectionString"></param>
        public AbstractStoredProcedureDA(string connectionString) { ConnectionString = connectionString; }

        /// <summary>
        /// Get by ID, with optional inflate (build object from DataReader) method
        /// </summary>
        /// <param name="storedProceduceName"></param>
        /// <param name="id"></param>
        /// <param name="sqlKeyType"></param>
        /// <param name="inflateDelegate"></param>
        /// <returns></returns>
        protected TEntity GetByID(string storedProceduceName, KeyType id, SqlDbType sqlKeyType, Func<SqlDataReader, TEntity> inflateDelegate)
        {
            using (SqlCommand cmd = new SqlCommand(storedProceduceName))
            {
                cmd.Parameters.Add("@ID", sqlKeyType).Value = id;
                return GetObject(cmd, inflateDelegate);
            }
        }

        /// <summary>
        /// Get by ID with default object inflate method
        /// </summary>
        /// <param name="storedProceduceName"></param>
        /// <param name="id"></param>
        /// <param name="sqlKeyType"></param>
        /// <returns></returns>
        protected TEntity GetByID(string storedProceduceName, KeyType id, SqlDbType sqlKeyType)
        {
            return GetByID(storedProceduceName, id, sqlKeyType, i => InflateEntityFromReader(i));
        }

        /// <summary>
        /// Get a single object using the specified stored procedure
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="inflateDelegate"></param>
        /// <returns></returns>
        protected TEntity GetObject(SqlCommand cmd, Func<SqlDataReader, TEntity> inflateDelegate)
        {
            //-- If the reader does not have a row, we are passing back default(TEntity);
            var obj = default(TEntity);
            
            using (SqlConnection dbCon = new SqlConnection(ConnectionString))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = dbCon;
                dbCon.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read()) { obj = inflateDelegate(r); }
                }
            }
            return obj;
        }

        /// <summary>
        /// Get a collection by a foreign key, or related (spatial intersecting key)
        /// </summary>
        /// <param name="storedProceduceName"></param>
        /// <param name="parameterName"></param>
        /// <param name="id"></param>
        /// <param name="sqlKeyType"></param>
        /// <returns></returns>
        protected IQueryable<TEntity> GetByForeignKey(string storedProceduceName, SqlParameter parameter)
        {
            using (SqlCommand cmd = new SqlCommand(storedProceduceName))
            {
                cmd.Parameters.Add(parameter);
                return GetCollection(cmd);
            }
        }

        protected IQueryable<TEntity> GetByForeignKey(string storedProceduceName, SqlParameter parameter, Func<SqlDataReader, TEntity> inflateDelegate)
        {
            using (SqlCommand cmd = new SqlCommand(storedProceduceName))
            {
                cmd.Parameters.Add(parameter);
                return GetCollection(cmd, inflateDelegate);
            }
        }

        /// <summary>
        /// Get All objects of type TEntity with the specified stored procedure
        /// </summary>
        /// <param name="storedProceduceName"></param>
        /// <returns></returns>
        protected IQueryable<TEntity> GetAll(string storedProceduceName)
        {
            using (SqlCommand cmd = new SqlCommand(storedProceduceName))
            {
                return GetCollection(cmd);
            }
        }

        /// <summary>
        /// Return a list of objects using the specified stored procedure
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected IQueryable<TEntity> GetCollection(SqlCommand cmd)
        {
            return GetCollection(cmd, i => InflateEntityFromReader(i)); 
        }
        
        protected IQueryable<TEntity> GetCollection(SqlCommand cmd, Func<SqlDataReader, TEntity> inflateDelegate)
        {
            var collection = new List<TEntity>();
            using (SqlConnection dbCon = new SqlConnection(ConnectionString))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = dbCon;
                dbCon.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) { collection.Add(inflateDelegate(r)); }
                }
            }
            return collection.AsQueryable();
        }

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="cmd"></param>
        protected void ExecuteUpdate(SqlCommand cmd)
        {
            using (SqlConnection dbCon = new SqlConnection(ConnectionString))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = dbCon;
                dbCon.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) { throw new Exception("Update failed for cmd : " + cmd.CommandText); }
            }
        }

        /// <summary>
        /// Here we want to force inserts to get an id back from the database as confirmation that the insert was successful.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        /// <remarks>We pass back the ID as a string so that we can use this code with child data access classes that have different
        /// key types</remarks>
        protected string ExecuteInsert(SqlCommand cmd)
        {
            string badID = default(KeyType).ToString();

            using (SqlConnection dbCon = new SqlConnection(ConnectionString))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = dbCon;
                dbCon.Open();

                string newID = cmd.ExecuteScalar().ToString();
                if (newID == badID) { throw new Exception("INSERT FAILED: " + cmd.CommandText); }

                return newID;
            }
        }

        /// <summary>
        /// Abstract declaration of how we create a TEntity from a data reader
        /// </summary>
        /// <param name="sqlDataReader"></param>
        /// <returns></returns>
        protected abstract TEntity InflateEntityFromReader(SqlDataReader sqlDataReader);
        
    }
}
