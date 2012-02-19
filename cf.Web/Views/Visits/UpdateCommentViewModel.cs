using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.CheckIns
{
    public class UpdateCommentViewModel
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}