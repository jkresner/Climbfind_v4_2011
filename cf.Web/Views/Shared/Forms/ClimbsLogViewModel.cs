using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;

namespace cf.Web.Views.Shared
{
    public class ClimbsLogViewModel
    {
        [Required]
        public Guid LogClimbsCheckInID { get; set; }

        [Required]
        public List<ClimbLogViewModel> Logs { get; set; }
    }

    public class ClimbLogViewModel
    {
        [Required]
        public Guid ClimbID { get; set; }

        [Required(ErrorMessage="Select experience")]
        public ClimbExperience Experience { get; set; }

        [Required(ErrorMessage = "Select outcome")]
        public ClimbOutcome Outcome { get; set; }

        [Required(ErrorMessage = "Give grade feedback")]
        public ClimbGradeOpinion Opinion { get; set; }

        [Required(ErrorMessage="Rate the climb")]
        [Range(1, 5, ErrorMessage="Rate the climb")]
        public byte Rating { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}