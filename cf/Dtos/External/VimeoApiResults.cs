using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos
{
    /// <summary>
    /// Dto representing a result from calling Vimeo api
    /// </summary>
    public class VimeoApiResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Published { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Success { get; set; }
    }
}
