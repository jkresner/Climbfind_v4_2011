using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities;
using cf.Entities.Interfaces;

namespace cf.Web.Views.Profiles
{
    public abstract class ProfileWebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        //public Country country { get { return ViewBag.Country as Country;} }
        public Profile user { get { return ViewBag.Current as Profile; } }
    }

    public abstract class ProfileWebViewPage : System.Web.Mvc.WebViewPage<dynamic>
    {
        //public Country country { get { return ViewBag.Country as Country;} }
        public Profile user { get { return ViewBag.Current as Profile; } }
    }
}