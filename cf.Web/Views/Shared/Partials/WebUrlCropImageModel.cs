using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using NetFrameworkExtensions;
using cf.Web.Mvc.Helpers;

namespace cf.Web.Models
{
    public class WebUrlCropImageModel
    {
        public string ObjID { get; set; }
        public string PostActionUrl { get; set; }
        public bool AllowUpload { get; set; }
        public string JavascriptSuccessCallback { get; set; }
        public string JavascriptFailCallback { get; set; }
        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public int BoxMaxWidth { get; set; }
        public int BoxMaxHeight { get; set; }

        public WebUrlCropImageModel()
        {
            BoxMaxWidth = 640;
            BoxMaxHeight = 600;
        }
    }
}
