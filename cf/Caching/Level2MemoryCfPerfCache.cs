using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Dtos;
using System.Diagnostics;
using System.Runtime.Caching;

namespace cf.Caching
{
    public class Level2MemoryCfPerfCache : IRemoteCache
    {
        protected CacheItemPolicy FiveMinCacheItemPolicy { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(300) }; } }
        public MemoryCache mClient { get; set; }

        public Level2MemoryCfPerfCache()
        {
            mClient = new MemoryCache("Level2CfPerfCache");
        }

        public object Get(string key) { return mClient.Get(key); }
        public bool Add<T>(T entry, string key) { mClient.Set(new CacheItem(key, entry), FiveMinCacheItemPolicy); return true; }
        public bool Add<T>(T entry, string key, TimeSpan timespan) { mClient.Set(new CacheItem(key, entry), FiveMinCacheItemPolicy); return true; } // NOTE 5 minute is hardcoded....
        public bool Remove(string key) { return mClient.Remove(key) != null; }

        public bool Refresh() 
        {
            mClient.Dispose();
            mClient = null;
            mClient = new MemoryCache("Level2CfPerfCache");

            return true;
        }
    }
}
