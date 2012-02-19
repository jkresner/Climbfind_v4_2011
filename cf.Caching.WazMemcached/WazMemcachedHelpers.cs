using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Protocol.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace cf.Caching.WazMemcached
{
    public static partial class WazMemcachedHelpers
    {
        /// <summary>
        /// http://marcgravell.blogspot.com/2010/01/distributed-caching-with-protobuf-net.html
        /// http://code.google.com/p/protobuf-net/
        /// </summary>
        /// <param name="memcachedRoleName"></param>
        /// <param name="memcachedEndpointName"></param>
        /// <returns></returns>
        public static MemcachedClient CreateProtobufferClient(string memcachedRoleName, string memcachedEndpointName)
        {
            //-- Here we inject our custom Proto-buffer "transcoder"
            var transcoder = new NetTranscoder();
            return new MemcachedClient(new WindowsAzureServerPool(memcachedRoleName, memcachedEndpointName), new DefaultKeyTransformer(), transcoder);
        }

        /// <summary>
        /// http://blog.smarx.com/posts/memcached-in-windows-azure (thank you SMARX!)
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="maxMemory"></param>
        /// <returns></returns>
        public static Process StartMemcachedServer(string endpointName, int maxMemory)
        {
            var port = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName].IPEndpoint.Port;

            var path = Path.Combine(
                Directory.Exists(Environment.ExpandEnvironmentVariables(@"%RoleRoot%\approot\bin"))
                    ? Environment.ExpandEnvironmentVariables(@"%RoleRoot%\approot\bin\memcached") // web role
                    : Environment.ExpandEnvironmentVariables(@"%RoleRoot%\approot\memcached"),    // worker role
                Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            return Process.Start(new ProcessStartInfo(
                Path.Combine(path, "memcached.exe"),
                string.Format("-p {0} -m {1}", port, maxMemory))
            {
                WorkingDirectory = path
            });
        }
    }
}