using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Shared
{
    public class NewRatingViewModel
    {
        [Required]
        public Guid RateObjectID { get; set; }

        [Required]
        [Range(1,5,ErrorMessage="Choose between 1 & 5 stars")]
        public byte RateScore { get; set; }

        [Required(ErrorMessage="Provide a comment")]
        public string RateComment { get; set; }
    }
}