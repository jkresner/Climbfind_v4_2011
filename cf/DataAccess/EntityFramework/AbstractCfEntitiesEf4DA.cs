using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Interfaces;
using cf.Entities.Interfaces;
using System.Data.Objects;
using System.Linq.Expressions;

namespace cf.DataAccess.EntityFramework
{
    /// <summary>
    /// Base class for interacting with our inner CF Entity Data Model (.edmx)
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="KeyType"></typeparam>
    public abstract class AbstractCfEntitiesEf4DA<TEntity, KeyType>
        where TEntity : class, IKeyObject<KeyType>, new()
        where KeyType : IEquatable<KeyType>
    {
        /// <summary>
        /// We use the same ConnectionStringKey across all child DAs
        /// </summary>
        protected string DefaultConnectionStringKey { get { return "CfEntitiesData"; } }

        /// <summary>
        /// Strongly typed ObjectsContext
        /// </summary>
        public cfEntitiesData Ctx { get; private set; }

        /// <summary>
        /// Constructor initializing our EDM connection using the DefaultConnectionStringKey
        /// </summary>
        public AbstractCfEntitiesEf4DA() { Ctx = new cfEntitiesData("name=" + DefaultConnectionStringKey); }

        /// <summary>
        /// Constructor initializing our EDM connection using the ConnectionStringKey parameter. This will throw an exception
        /// if there is no entry in .config with the matching key.
        /// </summary>
        /// <param name="connectionStringKey"></param>
        public AbstractCfEntitiesEf4DA(string connectionStringKey) { Ctx = new cfEntitiesData("name=" + connectionStringKey); }

        /// <summary>
        /// Constructor initializing our EDM connection using the connection
        /// </summary>
        /// <param name="connectionStringKey"></param>
        public AbstractCfEntitiesEf4DA(System.Data.EntityClient.EntityConnection connection) { Ctx = new cfEntitiesData(connection); }
        
        /// <summary>
        /// Call our data context to save the changes
        /// </summary>
        protected void SaveChanges()
        {
            Ctx.SaveChanges();
        }

        /// <summary>
        /// Delete an entity using it's key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ID"></param>
        public virtual void Delete(KeyType ID)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            var entityObj = set.Where(entity => entity.ID.Equals(ID)).SingleOrDefault();
            if (entityObj != default(TEntity))
            {
                set.DeleteObject(entityObj);
                SaveChanges();
            }
        }

        /// <summary>
        /// Delete an entity using it's key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ID"></param>
        public virtual void Delete(IEnumerable<KeyType> IDs)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            //-- TODO run sql trace to confirm performance impact of this code            
            foreach (var id in IDs) { 
            
                set.DeleteObject(set.Where(entity => entity.ID.Equals(id)).Single()); 
            }
            SaveChanges();
        }

        /// <summary>
        /// Get all objects from our EntitySet
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return Ctx.CreateObjectSet<TEntity>();
        }

        /// <summary>
        /// Get a specific object by it's key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(KeyType ID)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            return set.Where(entity => entity.ID.Equals(ID)).SingleOrDefault();
        }

        /// <summary>
        /// Get a collection object by foreign key selection
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetByForeignKeyID(dynamic predicate)
        {
            Expression<Func<TEntity, bool>> predicateExpression = predicate;
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            return set.Where(predicateExpression);
        }

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        public virtual TEntity Create(TEntity tEntity)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            set.AddObject(tEntity);
            SaveChanges();
            return tEntity;
        }

        /// <summary>
        /// Creates a list of entities
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        public virtual List<TEntity> Create(List<TEntity> tEntities)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            foreach (var tEntity in tEntities) { set.AddObject(tEntity); }
            SaveChanges();
            return tEntities;
        }

        /// <summary>
        /// Update all values of our entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        public virtual TEntity Update(TEntity tEntity)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            //-- We require this line because it brings the object into the EF State manager.
            TEntity tEntityInDB = GetByID(tEntity.ID);
            try
            {
                set.ApplyCurrentValues(tEntity);
                SaveChanges();
            }
            catch (Exception ex) { throw new Exception("Entity update failed: " + ex.Message); }
            return tEntity;
        }

        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> listOftEntity)
        {
            ObjectSet<TEntity> set = Ctx.CreateObjectSet<TEntity>();
            foreach (var tEntity in listOftEntity)
            {
                TEntity tEntityInDB = GetByID(tEntity.ID);
                set.ApplyCurrentValues(tEntity);
            }
            SaveChanges();
            return listOftEntity;
        }
    }
}
