using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Dtos
{
    /// <summary>
    /// Dto used to house a template key and it's associated template
    /// </summary>
    public class FeedTemplate : IKeyObject<string>
    {
        public string ID { get; set; }
        public string TemplateString { get; set; }
    }
}
