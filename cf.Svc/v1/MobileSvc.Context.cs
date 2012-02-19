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
using Microsoft.IdentityModel.Claims;
using NetFrameworkExtensions.Identity;

namespace cf.Svc.v1
{
    class SvcContext 
    {
        public bool Invalid { get; set; }
        public Message ContextMessage { get; set; }
        public LatLon ClientGeolocation { get; set; } 
        public double Lat { get { return ClientGeolocation.Lat; } }
        public double Lon { get { return ClientGeolocation.Lon; } }
    }

    public partial class MobileSvc : AbstractRestService
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;
        MappingService mappingSvc { get { if (_mappingSvc == null) { _mappingSvc = new MappingService(); } return _mappingSvc; } } MappingService _mappingSvc;
        MediaService medSvc { get { if (_medSvc == null) { _medSvc = new MediaService(); } return _medSvc; } } MediaService _medSvc;
        PostService postSvc { get { if (_postSvc == null) { _postSvc = new PostService(); } return _postSvc; } } PostService _postSvc;
        VisitsService visitsSvc { get { if (_visitsSvc == null) { _visitsSvc = new VisitsService(); } return _visitsSvc; } } VisitsService _visitsSvc;
        PartnerCallService pcSvc { get { if (_pcSvc == null) { _pcSvc = new PartnerCallService(); } return _pcSvc; } } PartnerCallService _pcSvc;
        UserService usrSvc { get { if (_usrSvc == null) { _usrSvc = new UserService(); } return _usrSvc; } } UserService _usrSvc;

        /// <summary>
        /// Reads the headers to determine client location & identity
        /// </summary>
        /// <returns>Service execution context</returns>
        private SvcContext InflateContext()
        {
            var ctx = new SvcContext();
                        
            var latLon = new LatLon();

            if (latLon.Inflate()) { ctx.ClientGeolocation = latLon; }
            //else { ctx.Invalid = true; ctx.ContextMessage = Forbidden("Operation not allowed"); return ctx; }

            SimpleWebToken swttoken = null;
            if (!IsAuthenticated(HttpContext.Current, out swttoken)) 
            { 
                ctx.Invalid = true;
                if (swttoken == null) { ctx.ContextMessage = Unauthorized("Token required"); }
                else if (swttoken.IsExpired) { ctx.ContextMessage = Unauthorized("Token expired"); }
                else { ctx.ContextMessage = Unauthorized("Token invalid"); }
            }

            return ctx;
        }

        /// <summary>
        /// Check if the user is authenticated with a valid token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool IsAuthenticated(HttpContext context, out SimpleWebToken swttoken)
        {
            IClaimsIdentity currentIdentiy = context.User.Identity as IClaimsIdentity;
            IClaimsPrincipal incomingPrincipal = context.User as IClaimsPrincipal;

            //if (!incomingPrincipal.Identity.IsAuthenticated)
            //{
            if (new cf.Identity.CfIdentityInflater().TryGetSwtClaimsIdentity(out currentIdentiy, out swttoken))
            {
                incomingPrincipal.Identities[0] = currentIdentiy;
            }
            //}

            return incomingPrincipal.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string GetObjectName(Guid id)
        {
            return AppLookups.GetCacheIndexEntry(id).Name;
        }
    }
}