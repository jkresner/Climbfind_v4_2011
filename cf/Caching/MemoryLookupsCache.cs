using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Collections.Specialized;

namespace cf.Caching
{
    /// <summary>
    /// Abstract class for delegate style cache item lookup, insertion & invalidation
    /// </summary>
    public abstract class MemoryLookupsCache
    {
        /// <summary>
        /// Our .net 4 memCache (concrete implementation of object cache)
        /// </summary>
        private MemoryCache Cache { get; set; }

        /// <summary>
        /// Static constructor fire the first time any static property on this class is requested
        /// </summary>
        protected MemoryLookupsCache(string cacheSetName, long cacheLimitInMegabytes)
        {
            NameValueCollection config = new NameValueCollection();
            config.Add("CacheMemoryLimitInMegabytes", cacheLimitInMegabytes.ToString());
            
            //-- Create an instance of MemoryCache
            Cache = new MemoryCache(cacheSetName, config);
        }

        /// <summary>
        /// Basic base method for retrieving and saving entries into our cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="delegateToGetItemIfNotInCache"></param>
        /// <param name="cacheItemPolicy"></param>
        /// <returns></returns>
        protected T TryGetFromCache<T>(string cacheKey, Func<T> delegateToGetItemIfNotInCache, CacheItemPolicy cacheItemPolicy)
        {
            T cachedObject = (T)Cache.Get(cacheKey);
            if (cachedObject == null)
            {
                lock (this)
                {
                    //-- Read the object from a repository (db, server, etc.) using our delegate
                    cachedObject = delegateToGetItemIfNotInCache();

                    if (cachedObject != null)
                    {
                        //-- Insert it using the cacheItemPolicy
                        Cache.Add(new CacheItem(cacheKey, cachedObject), cacheItemPolicy);
                    }
                }
            }
            return cachedObject;
        }

        /// <summary>
        /// Method to refresh an item in the cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="delegateToGetItem"></param>
        /// <param name="cacheItemPolicy"></param>
        /// <returns></returns>
        protected T RefreshCacheItem<T>(string cacheKey, Func<T> delegateToGetItem, CacheItemPolicy cacheItemPolicy)
        {
            T cachedObject;
            lock (this)
            {
                //-- Read the object from a repository (db, server, etc.) using our delegate
                cachedObject = delegateToGetItem();

                //-- Insert it using the cacheItemPolicy
                Cache.Remove(cacheKey);
                Cache.Add(new CacheItem(cacheKey, cachedObject), cacheItemPolicy);
            }
            
            return cachedObject;
        }
    }
}
