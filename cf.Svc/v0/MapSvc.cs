using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using cf.Services;
using System.ServiceModel;
using System.ServiceModel.Activation;
using cf.Instrumentation;
using cf.Caching;
using System.Web.Script.Serialization;
using cf.Entities;
using MappingSvc = cf.Services.MappingService;
using Message = System.ServiceModel.Channels.Message;

namespace cf.Svc.v0
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MapSvc : AbstractRestService
    {
        public const string ServiceCacheProfileName = "MapCacheFor180Seconds";

        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MappingSvc mapSvc { get { if (_mapSvc == null) { _mapSvc = new MappingSvc(); } return _mapSvc; } } MappingSvc _mapSvc;

        [WebGet(UriTemplate = "country/{id}"), AspNetCacheProfile("MapCacheFor20Minutes")]
        public Message GetCountry(string id)
        {
            //-- TODO: check country and pass back friendly error if not found page
            var countryWithOutGeo = AppLookups.Country(id);
            var country = geoSvc.GetCountryByID(countryWithOutGeo.ID);

            var mapItemCollection = mapSvc.GetCountryMapItems(country);

            return ReturnMapItemCollectionAsJson(mapItemCollection);
        }

        [WebGet(UriTemplate = "province/{id}"), AspNetCacheProfile("MapCacheFor10Minutes")]
        public Message GetProvince(string id)
        {
            Guid gId = new Guid(id);

            //-- TODO: check area and pass back not found page
            var area = geoSvc.GetAreaByID(gId);

            var mapItems = mapSvc.GetProvinceMapItems(area);

            var jsonMessage = ReturnMapItemCollectionAsJson(mapItems);

            return jsonMessage;
        }

        [WebGet(UriTemplate = "city/{id}"), AspNetCacheProfile(ServiceCacheProfileName)]
        public Message GetCity(string id)
        {
            Guid gId = new Guid(id);

            //-- TODO: check area and pass back not found page
            var area = geoSvc.GetAreaByID(gId);

            var mapItems = mapSvc.GetCityMapItems(area);

            return ReturnMapItemCollectionAsJson(mapItems);
        }

        [WebGet(UriTemplate = "area/{id}"), AspNetCacheProfile(ServiceCacheProfileName)]
        public Message GetArea(string id)
        {
            Guid gId = new Guid(id);

            //-- TODO: check area and pass back not found page
            var area = geoSvc.GetAreaByID(gId);
            //var rssTitle = string.Format("Climbing in {0}, {1}", area.Name, AppLookups.CountryName(area.CountryID));

            var mapItemCollection = mapSvc.GetAreaMapItems(area);

            return ReturnMapItemCollectionAsJson(mapItemCollection);
        }

        [WebGet(UriTemplate = "area-for-new-outdoor-location/{id}"), AspNetCacheProfile(ServiceCacheProfileName)]
        public Message GetAreaForNewOutdoorLocation(string id)
        {
            Guid gId = new Guid(id);

            //-- TODO: check area and pass back not found page
            var area = geoSvc.GetAreaByID(gId);

            var mapItemCollection = mapSvc.GetAreaForNewOutdoorClimbingLocationMapItems(area);

            return ReturnMapItemCollectionAsJson(mapItemCollection);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapItemCollection"></param>
        /// <returns></returns>
        private Message ReturnMapItemCollectionAsJson(MapItemCollection mapItemCollection)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            //-- remove this line!
            serializer.MaxJsonLength = 4097152;

            var output = serializer.Serialize(mapItemCollection);

            return WebOperationContext.Current.CreateTextResponse(output);
        }
    }
}