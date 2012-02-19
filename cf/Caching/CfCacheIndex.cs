using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using cf.Dtos;

namespace cf.Caching
{
    public class CfCacheIndex 
    {
        static string GetKey(Guid id) { return "ci" + id.ToString("N"); }
        static CacheItemPolicy Level1ItemPolicy { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(300) }; } } // 5 minutes (before updates/delete are reflected)
        static Level1MemoryCfCacheIndex Level1Cache { get; set; }
        static IRemoteCache<CfCacheIndexEntry> Level2Cache { get; set; }

        /// <summary>
        /// Used 2-tiered memory (only for development purposes)
        /// </summary>
        public static void Initialize()
        {
            Level1Cache = new Level1MemoryCfCacheIndex();
            Level2Cache = new Level2MemoryCfCacheIndex();
        }

        /// <summary>
        /// Used level 1 memory, plus level 2 remote (e.g. Memcached via Windows Azure Worker Role)
        /// </summary>
        public static void Initialize(IRemoteCache<CfCacheIndexEntry> level2Cache)
        {
            Level1Cache = new Level1MemoryCfCacheIndex();
            Level2Cache = level2Cache;
        }

        /// <summary>
        /// Add an item to our remote cache and if that succeeds add to local cache too
        /// </summary>
        /// <param name="entry"></param>
        public static void Add(CfCacheIndexEntry entry)
        {
            var key = GetKey(entry.ID);
            if (Level2Cache.Add(entry, key))
            {
                Level1Cache.Add(new CacheItem(key, entry), Level1ItemPolicy);
            }
        }

        /// <summary>
        /// Update item in our remote cache and if that succeeds add to local cache too
        /// </summary>
        /// <param name="entry"></param>
        public static void Update(CfCacheIndexEntry entry)
        {
            var key = GetKey(entry.ID);
            if (Level2Cache.Add(entry, key)) //-- Here we can call Add because the Memcached Store takes the Set flag which causes an update
            {
                Level1Cache.Remove(key);
                Level1Cache.Add(new CacheItem(key, entry), Level1ItemPolicy);
            }
        }

        /// <summary>
        /// Remove item from both our remote cache and if that succeeds add to local cache too
        /// </summary>
        /// <param name="entry"></param>
        public static void Remove(CfCacheIndexEntry entry)
        {
            var key = GetKey(entry.ID);
            Level2Cache.Remove(key);
            Level1Cache.Remove(key);
        }

        /// <summary>
        /// Dispose and re-create our local cash, flush our remote cash & recreate the cache index
        /// </summary>
        /// <param name="entry"></param>
        public static void Refresh()
        {
            Level1Cache.Dispose();
            Level1Cache = null;
            Level1Cache = new Level1MemoryCfCacheIndex();

            Level2Cache.Refresh();
        }

        /// <summary>
        /// Get our entry first looking in Level1 cache and if not reverting to Level2.
        /// </summary>
        /// <remarks>
        /// If the entry does not exist in level2 cache, then an "empty" entry is put into level1 which will stop
        /// the entry being looked for until it expires...
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CfCacheIndexEntry Get(Guid id)
        {
            var key = GetKey(id);
            var localObject = Level1Cache.Get(key) as CfCacheIndexEntry;
            if (localObject == null)
            {
                //-- Read the object from a repository (db, server, etc.) using our delegate
                var remoteObject = Level2Cache.Get(key);

                if (remoteObject == null)
                {
                    Level1Cache.Add(new CacheItem(key, new CfCacheIndexEntry() { ID = Guid.Empty }), Level1ItemPolicy);
                }
                else
                {
                    //-- Insert it using the cacheItemPolicy
                    Level1Cache.Add(new CacheItem(key, remoteObject), Level1ItemPolicy);
                    localObject = remoteObject;
                }
            }
            else if (localObject.ID == Guid.Empty)
            {
                //-- Here we're stopping multiple requests for no existing items going further than the local cache
                localObject = null;
            }

            return localObject;
        }
    }
}
