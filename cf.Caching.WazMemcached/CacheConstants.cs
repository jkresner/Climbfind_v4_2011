using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Caching.WazMemcached
{
    public static class CacheConstants
    {
        public const string CacheRole = "cf.CacheServer";
        public const string CacheEndpoint = "Memcached";
        public const string RefreshEndpointName = "Refresh";
        public const string RefreshEndpointAddressFormat = "net.tcp://{0}/RefreshService";
    }
}
