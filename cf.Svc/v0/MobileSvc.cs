using System;
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
using System.Net;
using cf.Services;
using cf.Entities;
using cf.Entities.Enum;
using cf.Dtos;
using cf.Dtos.Mobile.V0;
using Message = System.ServiceModel.Channels.Message;
using System.Text;

namespace cf.Svc.v0
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MobileSvc : AbstractRestService
    {      
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MappingService mappingSvc { get { if (_mappingSvc == null) { _mappingSvc = new MappingService(); } return _mappingSvc; } } MappingService _mappingSvc;
        MediaService medSvc { get { if (_medSvc == null) { _medSvc = new MediaService(); } return _medSvc; } } MediaService _medSvc;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "nearest-locations")]
        public Message GetNearestLocations()
        {
            return Gone("This version of the Climbfind app is no longer supported, please delete the app and re-download");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "locations-of-area/{id}")]
        public Message GetLocationsOfArea(string id)
        {
            return Gone("This version of the Climbfind app is no longer supported, please delete the app and re-download");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "highest-rated-media/{id}")]
        public Message GetObjectsBestMedia(string id)
        {
            return Gone("This version of the Climbfind app is no longer supported, please delete the app and re-download");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "recently-submitted-media/{id}")]
        public Message GetObjectsRecentMedia(string id)
        {
            return Gone("This version of the Climbfind app is no longer supported, please delete the app and re-download");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "recent-checkins/{id}")]
        public Message GetLastestCheckIns(string id)
        {
            return Gone("This version of the Climbfind app is no longer supported, please delete the app and re-download");
        }
    }
}