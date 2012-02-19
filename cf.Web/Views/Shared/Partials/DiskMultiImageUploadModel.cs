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
    public class DiskMultiImageUploadModel
    {
        public string ObjID { get; set; }
        public string PostActionUrl { get; set; }
        public string JavascriptSuccessCallback { get; set; }
        public string JavascriptFailCallback { get; set; }

        public DiskMultiImageUploadModel() { }
    }
}
