using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using NetFrameworkExtensions.Web.Mvc;
using NetFrameworkExtensions.Threading;
using cf.Instrumentation;
using cf.Dtos;
using cf.DataAccess.Repositories;

namespace cf.Caching
{
    /// <summary>
    /// Utility-like class giving us easy programmatic access to common data that is worthy of caching
    /// </summary>
    /// <remarks>
    /// This stuff should blow up if CfCacheIndex hasn't been initialized
    /// </remarks>
    public partial class AppLookups
    {
        protected static CountriesAndProvincesCache CPC = new CountriesAndProvincesCache();
        //protected static CfCacheIndex CI { get; set; }
        
        /// <summary>
        /// Static constructor to force some caching to occur on application load
        /// </summary>
        //public static void InitializeAllCacheItems()
        //{
        //    CPC
        //}

        public static void AddIndexEntryToCache(CfCacheIndexEntry entry) { CfCacheIndex.Add(entry); }
        public static void UpdateIndexEntryInCache(CfCacheIndexEntry entry) { CfCacheIndex.Update(entry); }
        public static CfCacheIndexEntry GetCacheIndexEntry(Guid id) { return CfCacheIndex.Get(id); }
        public static void RemoveCacheIndexEntry(Guid id) {
            var entry = GetCacheIndexEntry(id);
            if (entry != null) { CfCacheIndex.Remove(entry); }
        }

        /// <summary>
        /// Called when we want to manually force a cache refresh
        /// </summary>
        public static void RefreshCacheIndex() { CfCacheIndex.Refresh(); }
    }
}
