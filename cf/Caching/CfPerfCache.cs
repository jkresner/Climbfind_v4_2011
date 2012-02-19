using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using cf.Dtos;
using cf.Entities;
using cf.Services;
using cf.Dtos.Cache;

namespace cf.Caching
{
    /// <summary>
    /// A cache to throw things in that expire both locally (on custom timing) and remotely after 5 minutes
    /// </summary>
    /// <remarks>If multiple client require the same data then it will be cached an made available for 5 minutes in level 2, but might be cached locally
    /// for say 60 minutes (e.g. geo deducible places)</remarks>
    public class CfPerfCache
    {
        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan FiveMinTimeSpan { get { return new TimeSpan(0, 5, 0); } }
        public static TimeSpan SixtyMinTimeSpan { get { return new TimeSpan(0, 60, 0); } }
        public static CacheItemPolicy FiveMinCacheItemPolicy { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(300) }; } }
        public static CacheItemPolicy SixtyMinCacheItemPolicy { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60) }; } }

        static Level1MemoryCfPerfCache Level1Cache { get; set; }
        static IRemoteCache Level2Cache { get; set; }

        /// <summary>
        /// Used 2-tiered memory (only for development purposes)
        /// </summary>
        public static void Initialize()
        {
            Level1Cache = new Level1MemoryCfPerfCache();
            Level2Cache = new Level2MemoryCfPerfCache();
        }

        /// <summary>
        /// Used level 1 memory, plus level 2 remote (e.g. Memcached via Windows Azure Worker Role)
        /// </summary>
        public static void Initialize(IRemoteCache level2Cache)
        {
            Level1Cache = new Level1MemoryCfPerfCache();
            Level2Cache = level2Cache;
        }

        /// <summary>
        /// Add an item to our remote cache and if that succeeds add to local cache too
        /// </summary>
        /// <param name="entry"></param>
        public static void Add<T>(T entry, string key, CacheItemPolicy cacheItemPolicy)
        {
            if (Level2Cache.Add(entry, key, FiveMinTimeSpan))
            {
                Level1Cache.Add(new CacheItem(key, entry), cacheItemPolicy);
            }
        }

        /// <summary>
        /// Dispose and re-create our local cash, flush our remote cash & recreate the cache index
        /// </summary>
        /// <param name="entry"></param>
        public static void Refresh()
        {
            Level1Cache.Dispose();
            Level1Cache = null;
            Level1Cache = new Level1MemoryCfPerfCache();

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
        public static T TryGetFromCache<T>(string key, Func<T> delegateToGetItemIfNotInCache, CacheItemPolicy cacheItemPolicy)
        {
            T localObject = (T)Level1Cache.Get(key);
            if (localObject == null)
            {
                //-- Read the object from a repository (db, server, etc.) using our delegate
                T remoteObject = default(T);

                //-- After redeployments we're having troubles ....
                try { remoteObject = (T)Level2Cache.Get(key); } catch (Exception ex) { 
                    var msg = string.Format("Failed to deserialize [{0}] resetting cache value", key);
                    Mail.MailMan.SendAppEvent(Instrumentation.TraceCode.AppBuildCache, msg, "", Stgs.SystemID, "jkresner@yahoo.com.au", false); } 

                if (remoteObject != null)
                {
                    //-- Insert it using the cacheItemPolicy
                    Level1Cache.Add(new CacheItem(key, remoteObject), cacheItemPolicy);
                    localObject = remoteObject;
                }
            }

            if (localObject == null)
            {
                //-- Read the object from a repository (db, server, etc.) using our delegate
                localObject = delegateToGetItemIfNotInCache();

                if (localObject != null)
                {
                    Add(localObject, key, cacheItemPolicy);
                }
            }

            return localObject;
        }

        public static void Flush(string key)
        {
            Level1Cache.Remove(key);
            Level2Cache.Remove(key);
        }

        /// <summary>
        /// Get our entry if it exists and if not returns null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T SessionGetFromCache<T>(string key)
        {
            //-- Read the object from a repository (db, server, etc.) using our delegate
            T remoteObject = (T)Level2Cache.Get(key);

            return remoteObject;
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
        public static T SessionAddToCache<T>(string key, T t, TimeSpan timeSpan)
        {
            Level2Cache.Add(t, key, timeSpan);
            
            return t;
        }


        /// <summary>
        /// Used everywhere to minimize DB hits for user data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CachedProfileDetails GetClimber(Guid id)
        {
            string key = "climber-" + id.ToString();
            return TryGetFromCache(key, () => new CachedProfileDetails(new UserService().GetProfileByID(id)), FiveMinCacheItemPolicy);
        }


        /// <summary>
        /// Used for mobile service with 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>The value of having a cached object rather than a join means the join won't occur on each climb 
        /// (time x over multiple consumers)</remarks>
        public static CachedLocationDetails GetLocation(Guid id)
        {
            string key = "loc-" + id.ToString();
            return TryGetFromCache(key, () => new CachedLocationDetails(new GeoService().GetLocationByID(id)), FiveMinCacheItemPolicy);
        }

        /// <summary>
        /// Used for mobile service with all the logged climb stuff needing to pass back grade / avatar etc.... (I think)!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public static Climb GetClimb(Guid id)
        //{
        //    string key = "climb-" + id.ToString();
        //    return TryGetFromCache(key, () => new GeoService().GetClimbByID(id), FiveMinCacheItemPolicy);
        //}

        /// <summary>
        /// User for partner calls event-ing
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public static List<cf.Dtos.CfCacheIndexEntry> GetGeoDeduciblePlaces(cf.Dtos.CfCacheIndexEntry place)
        {
            string key = "deduc-ofplace-" + place.ID.ToString();
            return TryGetFromCache(key, () => new GeoService().GetGeoDeduciblePlaces(place), SixtyMinCacheItemPolicy);
        }
    }
}
