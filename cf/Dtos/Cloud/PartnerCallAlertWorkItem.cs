using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Dtos.Cloud
{
    public class PartnerCallAlertWorkItem
    {
        public Guid ByID { get; set; }
        public Guid PlaceID { get; set; }
        public Guid PartnerCallID { get; set; }
    }
}
