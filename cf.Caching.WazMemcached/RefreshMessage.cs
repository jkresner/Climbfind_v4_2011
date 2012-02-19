using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace cf.Caching.WazMemcached
{
    [DataContract(Namespace = "urn:CfCacheIndex:2011:09:13")]
    public class RefreshMessage
    {
        [DataMember]
        public string RoleName { get; set; }
    }
}
