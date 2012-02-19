using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;
using cf.Dtos;
using Enyim.Caching.Memcached;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.ServiceModel;
using System.Diagnostics;

namespace cf.Caching.WazMemcached
{
    public class Level2MemcachedCacheIndex : IRemoteCache<CfCacheIndexEntry>
    {
        public MemcachedClient mClient { get; set; }

        public Level2MemcachedCacheIndex()
        {
            mClient = WazMemcachedHelpers.CreateProtobufferClient(CacheConstants.CacheRole, CacheConstants.CacheEndpoint);
        }

        public CfCacheIndexEntry Get(string key) { return mClient.Get(key) as CfCacheIndexEntry; }
        public bool Add(CfCacheIndexEntry entry, string key) { return mClient.Store(StoreMode.Set, key, entry); }
        public bool Remove(string key) { return mClient.Remove(key); }

        public bool Refresh() 
        {
            var role = RoleEnvironment.Roles[CacheConstants.CacheRole];

            //-- We only want the rebuild to happen once (or at least called once & leave the details to memcached)
            var instance = role.Instances[0];
            var refreshEndpoint = instance.InstanceEndpoints[CacheConstants.RefreshEndpointName];
            var binding = new NetTcpBinding(SecurityMode.None, false);
            var endpointAddress = new EndpointAddress(String.Format(CacheConstants.RefreshEndpointAddressFormat, refreshEndpoint.IPEndpoint));

            try
            {
                var myChanFac = new ChannelFactory<IRefreshService>(binding, endpointAddress);
                var refreshClient = myChanFac.CreateChannel();
                refreshClient.RefreshCacheIndex(new RefreshMessage() { RoleName = instance.Role.Name });
            }
            catch (Exception e)
            {
                Trace.WriteLine("An error occured trying to notify the instances: " + e.Message, "Warning");
                return false;
            }

            return true;
        }
    }
}
