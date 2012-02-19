using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Dtos;
using System.Diagnostics;
using System.Runtime.Caching;

namespace cf.Caching
{
    public class Level1MemoryCfPerfCache : MemoryCache
    {
        public Level1MemoryCfPerfCache() : base("PerfCache") { }
    }
}
