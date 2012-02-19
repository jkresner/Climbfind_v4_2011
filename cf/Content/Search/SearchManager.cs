using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using cf.Instrumentation;
using System.Diagnostics;
using cf.DataAccess.Repositories;
using cf.Entities.Enum;

namespace cf.Content.Search
{
    public class SearchManager
    {
        CountryRepository countryRepo { get { if (_countryRepo == null) { _countryRepo = new CountryRepository(); } return _countryRepo; } } CountryRepository _countryRepo;
        AreaRepository areaRepo { get { if (_areaRepo == null) { _areaRepo = new AreaRepository(); } return _areaRepo; } } AreaRepository _areaRepo;
        LocationRepository locationRepo { get { if (_locationRepo == null) { _locationRepo = new LocationRepository(); } return _locationRepo; } } LocationRepository _locationRepo;
        ClimbRepository climbRepo { get { if (_climbRepo == null) { _climbRepo = new ClimbRepository(); } return _climbRepo; } } ClimbRepository _climbRepo;
        ProfileRepository profileRepo { get { if (_profileRepo == null) { _profileRepo = new ProfileRepository(); } return _profileRepo; } } ProfileRepository _profileRepo;
        
        public void BuildIndex(Directory directory) 
        {
            CfTrace.Current.Information(TraceCode.AppBuildSearchIndex, "Create Search Index");

            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (directory == null) {
                var directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";
                directory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(directoryPath));
            }
                
            var indexBuilder = new CfLuceneIndexBuilder(directory);
            var allEntries = GetAllEntries();
            indexBuilder.AddEntries(allEntries);
            
            stopwatch.Stop();
           
            //-- These lines have to be before Dispose() otherwise an exception will be thrown
            CfTrace.Current.Performance(TraceCode.AppBuildSearchIndex, startTime, stopwatch.Elapsed, "Build Search Index", "Total entries: " + indexBuilder.GetTotalIndexedEntryCount());
            CfTrace.Current.Information(TraceCode.AppBuildSearchIndex, "End Create Search Index after {0}ms", stopwatch.ElapsedMilliseconds);

            indexBuilder.Dispose();
         }

        //public void PersistIndex(T directory) { }
        //public Directory GetIndex(Directory directory) { return null; }
        
        private IEnumerable<ILuceneSearchEngineEntry> GetAllEntries()
        {
            var entries = new List<SearchEngineEntryPlace>();
            byte indoorClimbTypeID = (byte)CfType.ClimbIndoor;

            foreach (var e in countryRepo.GetAll()) { entries.Add(new SearchEngineEntryPlace(e)); }
            foreach (var e in areaRepo.GetAll()) { entries.Add(new SearchEngineEntryPlace(e)); }
            foreach (var e in locationRepo.GetAll()) { entries.Add(new SearchEngineEntryPlace(e)); }
            foreach (var e in climbRepo.GetAll().Where(e => e.TypeID != indoorClimbTypeID)) { entries.Add(new SearchEngineEntryPlace(e)); }
            foreach (var e in profileRepo.GetAll().Where( e=>e.PrivacyShowInSearch) ) { entries.Add(new SearchEngineEntryPlace(e)); }

            return entries;
        }
    }
}
