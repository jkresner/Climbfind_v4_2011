using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Entities.Interfaces;

namespace cf.Web.Views.Places
{
    public abstract class PlacesWebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>, IPlaceView
    {
        public Country country { get { return ViewBag.Country as Country;} }
        public IPlaceWithGeo current { get { return ViewBag.Current as IPlaceWithGeo; } }
        
        public List<Area> intersectingCities { get { return ViewBag.Cities as List<Area>; } }
        public List<Area> intersectingProvinces { get { return ViewBag.Provinces as List<Area>; } }
        public List<Area> intersectingClimbingAreas { get { return ViewBag.ClimbingAreas as List<Area>; } }

        public IPlaceWithGeo parentProvince { get {
            if (current.Type == Entities.Enum.CfType.Province) { return current; }
            else if (intersectingProvinces.Count == 1) { return intersectingProvinces.First(); }
            else { return null; } } }

        public ObjectModMeta objModMeta { get { return ViewBag.ObjectModMeta as ObjectModMeta; } }

        public bool HasProvinceContext { get { return parentProvince != null; } }
    }

    public abstract class PlacesWebViewPage : System.Web.Mvc.WebViewPage<dynamic>, IPlaceView
    {
        public Country country { get { return ViewBag.Country as Country; } }
        public IPlaceWithGeo current { get { return ViewBag.Current as IPlaceWithGeo; } }

        public List<Area> intersectingCities { get { return ViewBag.Cities as List<Area>; } }
        public List<Area> intersectingProvinces { get { return ViewBag.Provinces as List<Area>; } }
        public List<Area> intersectingClimbingAreas { get { return ViewBag.ClimbingAreas as List<Area>; } }
        
        public IPlaceWithGeo parentProvince
        {
            get
            {
                if (current.Type == Entities.Enum.CfType.Province) { return current; }
                else if (intersectingProvinces.Count == 1) { return intersectingProvinces.First(); }
                else { return null; }
            }
        }

        public bool HasProvinceContext { get { return intersectingProvinces.Count > 0; } }
    }

    public interface IPlaceView
    {
        Country country { get; }
        IPlaceWithGeo current { get; }
        List<Area> intersectingProvinces { get; }
        IPlaceWithGeo parentProvince { get; }
        bool HasProvinceContext { get; }
        List<Area> intersectingCities { get; }
    }
}