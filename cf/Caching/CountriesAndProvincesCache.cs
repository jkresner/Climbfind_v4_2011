using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using cf.Entities;
using cf.Services;
using cf.DataAccess.Repositories;

namespace cf.Caching
{
    /// <summary>
    /// Memory cache of data required by the AppLookups class
    /// </summary>  
    public sealed class CountriesAndProvincesCache : MemoryLookupsCache
    {
        /// <summary>
        /// Constructor forcing our MemCache
        /// </summary>
        /// <param name="appLookupsCacheKey"></param>
        public CountriesAndProvincesCache() : base("CountriesAndProvinces", 4) { }

        /// <summary>
        /// Additional Cache Item Policy (Add as many as you like)
        /// </summary>
        CacheItemPolicy OneHourCacheItemPolicy
        { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }; } }

        CacheItemPolicy SixHourCacheItemPolicy
        { get { return new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddHours(6) }; } }

        /// <summary>
        /// Method to flush and refresh all data in the cache
        /// </summary>
        /// <remarks>
        /// If you add data to the cache you need to add the Refresh method for that data set to this method
        /// </remarks>
        public void RefreshEntireCache()
        {
            RefreshCountries();
        }

        /// <summary>
        /// Cached Provinces
        /// </summary>
        const string ProvincesKey = "Provinces_{0}";

        Func<byte, List<Area>> GetProvincesDelegate = id => new AreaRepository().GetProvincesOfCountry(id).OrderBy(c => c.Name).ToList();
        public List<Area> GetProvinces(byte countryID)
        {
            var key = string.Format(ProvincesKey, countryID);
            return TryGetFromCache(key, () => GetProvincesDelegate(countryID), SixHourCacheItemPolicy);
        }

        /// <summary>
        /// Cached Countries
        /// </summary>
        const string CountriesKey = "Countries";
        Func<List<Country>> GetCountriesDelegate = () => new CountryRepository().GetAll().OrderBy(c => c.Name).ToList();
        public List<Country> Countries { get { return TryGetFromCache(CountriesKey, GetCountriesDelegate, OneHourCacheItemPolicy); } }
        public void RefreshCountries() { RefreshCacheItem(CountriesKey, GetCountriesDelegate, OneHourCacheItemPolicy); }
    }
}
