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
    public class Bing7GeoJsonMapViewModel : Bing7MapViewModel
    {
        public string GeoJsonUrl { get; set; }
        public string DefaultPolygonColor { get; set; }
        public string DefaultPolygonStrokeColor { get; set; }
        public Bing7MapViewOptionsViewModel ViewOptions { get; set; }
        public override string MapTypeId
        {
            get
            {
                if (ViewOptions == default(Bing7MapViewOptionsViewModel) || string.IsNullOrWhiteSpace(ViewOptions.MapTypeId)) { return base.MapTypeId; }
                else { return ViewOptions.MapTypeId; }
            }
            set
            {
                base.MapTypeId = value;
            }
        } 


        public Bing7GeoJsonMapViewModel() : base() { }
        
        public Bing7GeoJsonMapViewModel(string mapId, int width, int height, string geoRssRelativeUrl) :
            base (mapId, width, height)
        {
            Height = height;
            Width = width;
            GeoJsonUrl = Stgs.SvcRt + geoRssRelativeUrl;
            DefaultPolygonColor = "Color(60, 255, 165, 0)";
            DefaultPolygonStrokeColor = "Color(0, 0, 255, 0)";
        }

        public Bing7GeoJsonMapViewModel(string mapId, int width, int height, string geoRssRelativeUrl, bool hidePolygon) :
            this(mapId, width, height, geoRssRelativeUrl)
        {
            if (hidePolygon)
            {
                SetInvisiblePolygons();
            }
        }

        public void SetInvisiblePolygons()
        {
            DefaultPolygonColor = "Color(0, 0, 0, 0)";
            DefaultPolygonStrokeColor = "Color(0, 0, 0, 0)";
        }
    }
}
