using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos.Cloud
{
    public class MessageAlertWorkItem
    {
        public Guid FromID { get; set; }
        public Guid ToID { get; set; }
        public string Content { get; set; }
    }
}
