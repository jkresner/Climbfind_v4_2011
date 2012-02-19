using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cf.Web.Views.Shared
{
    public class FormOptionsViewModel
    {
        public string CancelUrl { get; set; }
        public string CancelText { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteText { get; set; }

        public FormOptionsViewModel() { CancelText = "Cancel"; DeleteText = "Delete"; }
    }
}