using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Web.Mvc.ActionFilters;

namespace cf.Web.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Index() { return View(); }
        public ActionResult Faq() { return View(); }
    }
}
