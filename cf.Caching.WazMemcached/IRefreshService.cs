using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace cf.Caching.WazMemcached
{
    [ServiceContract(Namespace = "urn:CfCacheIndex:2011:09:13")]
    public interface IRefreshService
    {
        [OperationContract]
        void RefreshCacheIndex(RefreshMessage sender);
    }
}
