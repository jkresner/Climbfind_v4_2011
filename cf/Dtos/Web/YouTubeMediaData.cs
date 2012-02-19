using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos
{
    /// <summary>
    /// Dto representing a result from calling YouTube api
    /// </summary>
    public class YouTubeMediaData
    {
        public string YouTubeID { get; set; }
        public string Thumbnail { get; set; }
    }
}
