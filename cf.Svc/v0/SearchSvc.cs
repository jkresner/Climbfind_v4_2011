﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using cf.Content.Search;
using cf.Identity;
using cf.Caching;

namespace cf.Svc.v0
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SearchSvc : AbstractRestService
    {
        [WebGet(UriTemplate = "term/{searchTerm}")]
        public Message Term(string searchTerm)
        {
            try
            {
                var results = Global.SiteSearchEngine.Search(searchTerm, 10);

                if (results.Count() > 0) { return ReturnAsJson(results); }
                else
                {
                    return ReturnAsJson(new SearchEngineResult(
                        "No results found", 10, "Try the advanced search", "/rock-climbing-database/add-climbing-location") { Flag = "null.gif" }.AsSingleInList());
                }
            }
            catch (Exception ex) { return  HandelSearchException(ex); }
        }


        [WebGet(UriTemplate = "location/{searchTerm}")]
        public Message Location(string searchTerm)
        {
            try
            {
                var results = Global.SiteSearchEngine.Search(searchTerm, 10);
                var locationRestuls = new List<SearchEngineResult>();

                if (results.Count() > 0)
                {
                    foreach (var r in results) { if (r.TypeID > 9) { locationRestuls.Add(r); } }
                    return ReturnAsJson(locationRestuls);
                }
                else
                {
                    return ReturnAsJson(new SearchEngineResult(
                        "No locations found, <b>Add one now!</b>", 10, "Add a new location", "/rock-climbing-search-engine").AsSingleInList());
                }
            } catch (Exception ex) { return HandelSearchException(ex); }
        }


        [WebGet(UriTemplate = "province/{searchTerm}")]
        public Message Province(string searchTerm)
        {
            try
            {
                var results = Global.SiteSearchEngine.Search(searchTerm, 10);
                var locationRestuls = new List<SearchEngineResult>();

                if (results.Count() > 0)
                {
                    foreach (var r in results) { if (r.TypeID == 2) { locationRestuls.Add(r); } }
                    return ReturnAsJson(locationRestuls);
                }
                else
                {
                    return ReturnAsJson(new SearchEngineResult(
                        "No provinces found, try again...", 10, "Province not found", "/rock-climbing-search-engine").AsSingleInList());
                }
            }
            catch (Exception ex) { return HandelSearchException(ex); }
        }

        [WebGet(UriTemplate = "climbing-area/{searchTerm}")]
        public Message ClimbingArea(string searchTerm)
        {
            try
            {
                var results = Global.SiteSearchEngine.Search(searchTerm, 10);
                var locationRestuls = new List<SearchEngineResult>();

                if (results.Count() > 0)
                {
                    foreach (var r in results) { if (r.TypeID == 7) { locationRestuls.Add(r); } }
                    return ReturnAsJson(locationRestuls);
                }
                else
                {
                    return ReturnAsJson(new SearchEngineResult(
                        "No areas found, <b>Add one now!</b>", 10, "Area not found", "rock-climbing-database/choose-area-type").AsSingleInList());
                }
            }
            catch (Exception ex) { return HandelSearchException(ex); }
        }
        
        //[WebGet(UriTemplate = "refresh")]
        //public Message RefreshIndex()
        //{
        //    if (!CfIdentity.IsAuthenticated)
        //    {
        //        return ReturnAsJson(new List<object>() { new { Excerpt = "Refresh Failed - Not authenticated", Score = "0" } });
        //    }

        //    return RefreshIndexInternal();
        //}

        /// <summary>
        /// So we can control authorization to refresh index
        /// </summary>
        //private Message RefreshIndexInternal()
        //{
        //    try
        //    {
        //        AppLookups.RefreshCacheIndex();

        //        var siteSearchEngine = Global.SiteSearchEngine;
        //        siteSearchEngine.Dispose();
        //        siteSearchEngine = null;


        //        siteSearchEngine = new LuceneCfSearchEngineService();

        //        new LuceneCfIndexingService(siteSearchEngine).RebuildIndex();

        //        var successResult = new { Title = "Success", Score = float.Parse(siteSearchEngine.GetTotalIndexedEntryCount().ToString()) };
        //        return ReturnAsJson(new List<object>() { successResult });

        //    }
        //    catch (Exception ex)
        //    {
        //        CfTracer.Error(ex);

        //        var failedResult = new { Title = "Failed", Excerpt = ex.Message };
        //        return ReturnAsJson(new List<object>() { failedResult });

        //    }
        //}


        private Message HandelSearchException(Exception ex)
        {
            CfTracer.Error(ex);
            //RefreshIndex();

            var rebuildingResult = new SearchEngineResult()
            {
                Title = "Search service refreshing",
                CountryID = 10,
                Excerpt = "Try search again in a few seconds",
                Url = "#"
            };

            return ReturnAsJson(new List<SearchEngineResult>() { rebuildingResult });
        }
    }
}