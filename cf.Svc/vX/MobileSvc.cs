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

namespace cf.Svc.vX
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MobileSvc : AbstractRestService
    {      
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "supported-versions")]
        public Message GetSupportedVersions()
        {
            object obj = new { Versions = new[] { "V0", "V1" } };
            return ReturnAsJson(obj);
        }

        [WebGet(UriTemplate = "lastest-roadmap")]
        public Message GetRoadMapHtml()
        {
            object obj = new { Info = "Html info coming here" };
            return ReturnAsJson(obj);
        }
    }
}