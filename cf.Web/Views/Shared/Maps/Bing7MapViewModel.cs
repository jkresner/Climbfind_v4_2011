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
    //public class Bing7MapButtonModel
    //{
    //    public string ButtonText { get; set; }
    //    public string ButtonEventInitializer { get; set; }
    //}

    public class Bing7MapViewModel
    {
        public string MapID { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        string _mapTypeId = "auto";
        public virtual string MapTypeId { get { return _mapTypeId; } set { _mapTypeId = value; } }
        public string Credentials { get; set; }

        public Bing7MapViewModel() { }

        public Bing7MapViewModel(string mapId, int width, int height)
        {
            MapID = mapId.RemoveNonUtf8Characters().ToDomIdFriendlyString();
            Height = height;
            Width = width;
            Credentials = "ArIOaOmY-BqIbbf1Ueo_9McVfA9iTm_WdfX9-Boyeyg_ZuSN1dCeNQ5d1bvxsdgt";
        }
    }
}
