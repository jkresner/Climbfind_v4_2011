using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.DataAccess.Interfaces
{
    /// <summary>
    /// Generic interface for repository classes to implement to declare they can write objects to a store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IKeyEntityWriter<TEntity, KeyType> : IKeyEntityAccessor<TEntity, KeyType>
        where TEntity : IKeyObject<KeyType>, new()
        where KeyType : IEquatable<KeyType>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        TEntity Update(TEntity tEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        TEntity Create(TEntity tEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tEntity"></param>
        void Delete(KeyType key);
    }
}
