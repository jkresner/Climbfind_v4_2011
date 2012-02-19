using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos.Cloud
{
    public class CommentAlertWorkItem
    {
        public Guid ByID { get; set; }
        public Guid PostID { get; set; }
        public string Content { get; set; }
    }
}
