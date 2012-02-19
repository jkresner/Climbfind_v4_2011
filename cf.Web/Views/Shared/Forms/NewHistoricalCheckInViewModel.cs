using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Views.Shared
{
    public class NewHistoricalCheckInViewModel
    {
        [Required(ErrorMessage="Date required")]
        [RegularExpression("^[A-Z]{1}[a-z]{2}[ ]{1}[0-9]{2}[ ]{1}[A-Z]{1}[a-z]{2}[ ]{1}[2]{1}[0]{1}[0-9]{2}$", ErrorMessage = "Select date")]
        public string CheckInDate { get; set; }

        [RegularExpression("^[0|1|2]{1}[0-9]{1}[:][0-5]{1}[0-9]{1}$", ErrorMessage = "HH:mm required")]
        public string CheckInTime { get; set; }

        [Required(ErrorMessage = "Location required")]
        public Guid CheckLocationID { get; set; }

        public string CheckInComment { get; set; }
    }
}