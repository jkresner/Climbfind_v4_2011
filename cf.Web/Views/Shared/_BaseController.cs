using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using cf.Entities;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using NetFrameworkExtensions.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Caching;
using NetFrameworkExtensions.Net;
using cf.Services;
using cf.Instrumentation;
using System.Web.Security;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Protocols.WSFederation;
using cf.Identity;
using cf.Content.Search;
using cf.Content.Images;
using NetFrameworkExtensions.Threading;
using System.Threading.Tasks;

namespace cf.Web.Controllers
{
    public class BaseCfController : Controller
    {
        protected JsonResult CropAndSaveImageFromUrlAndDeleteOriginalAtUrl(string originalImgUrl, Func<Stream, string> saveImageFunc)
        {
            try
            {
                var url = string.Empty;

                using (Stream imgStream = new ImageDownloader().DownloadImageAsStream(originalImgUrl))
                {
                    url = saveImageFunc(imgStream);
                }

                //-- Delete the image in the background
                //new ThreadHelper(CfTrace.AppCurrent.trace).FireAndForget(o => , 
                //    "Failed to delete temp image " + originalImgUrl );
                new ContentService().DeleteTempImage(originalImgUrl);

                return Json(new { Success = true, SavedImgUrl = url });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }
    }
}
 