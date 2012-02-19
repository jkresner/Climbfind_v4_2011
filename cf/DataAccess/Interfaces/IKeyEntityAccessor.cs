using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.DataAccess.Interfaces
{
    /// <summary>
    /// Generic interface for repository classes to implement to declare they can read objects from a store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IKeyEntityAccessor<TEntity, KeyType>
        where TEntity : IKeyObject<KeyType>, new()
        where KeyType : IEquatable<KeyType>
    {
        /// <summary>
        /// Get all objects in the set
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Get the DTO with a specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByID(KeyType id);
    }
}
