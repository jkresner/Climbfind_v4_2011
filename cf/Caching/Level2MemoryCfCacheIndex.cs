using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Dtos;
using System.Diagnostics;
using System.Runtime.Caching;
using cf.DataAccess.Repositories;

namespace cf.Caching
{
    public class Level2MemoryCfCacheIndex : IRemoteCache<CfCacheIndexEntry>
    {
        public CacheItemPolicy LongTimePolicy { get; set; }
        public MemoryCache mClient { get; set; }

        public Level2MemoryCfCacheIndex()
        {
            mClient = new MemoryCache("Level2CfCacheIndex");
            LongTimePolicy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddDays(1) };

            var index = new CFCacheIndexEntryRepository().GetAll().ToList();
            foreach (var entry in index) { 
                var key = "ci" + entry.ID.ToString("N");
                mClient.Add(new CacheItem(key, entry), LongTimePolicy);
            }
        }

        public CfCacheIndexEntry Get(string key) { return mClient.Get(key) as CfCacheIndexEntry; }
        public bool Add(CfCacheIndexEntry entry, string key) { return mClient.Add(new CacheItem(key, entry), LongTimePolicy); }
        public bool Remove(string key) { return mClient.Remove(key) != null; }

        public bool Refresh() 
        {
            mClient.Dispose();
            mClient = null;
            mClient = new MemoryCache("Level2CfCacheIndex");

            var index = new CFCacheIndexEntryRepository().GetAll().ToList();
            foreach (var entry in index)
            {
                var key = "ci" + entry.ID.ToString("N");
                mClient.Add(new CacheItem(key, entry), LongTimePolicy);
            }
            return true;
        }
    }
}
