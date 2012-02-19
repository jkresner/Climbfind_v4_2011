using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;

namespace cf.Dtos
{
    /// <summary>
    /// Data Transfer Object that contains 
    /// </summary>
    public class FeedResultSet
    {
        public List<CfCacheIndexEntry> Places { get; set; }
        public List<PostRendered> Posts { get; set; }

        public FeedResultSet()
        {
            Places = new List<CfCacheIndexEntry>();
            Posts = new List<PostRendered>();
        }
    }
}
