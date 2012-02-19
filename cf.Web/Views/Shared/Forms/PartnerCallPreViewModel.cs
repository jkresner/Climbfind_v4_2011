using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Views.Shared
{
    public class PartnerCallPreViewModel
    {
        [Required]
        public Guid id { get; set; }
    }
}