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
using NetFrameworkExtensions.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Caching;
using NetFrameworkExtensions.Net;
using cf.Services;
using cf.Instrumentation;
using cf.Identity;
using Microsoft.IdentityModel.Claims;
using cf.Entities.Enum;
using cf.Content.Images;
using NetFrameworkExtensions.Identity;

namespace cf.Web.Controllers
{
    public class UploadController : Controller
    {
        GeoService geoSvc { get { if (_geoSvc == null) { _geoSvc = new GeoService(); } return _geoSvc; } } GeoService _geoSvc;

        public Stream CurrentInputStream
        {
            get
            {
                HttpPostedFileBase file = ControllerContext.HttpContext.Request.Files["media"]; //android work around
                if (file != null) { return file.InputStream; ; }
                else { return ControllerContext.HttpContext.Request.InputStream; }
            }
        }

        private Media SaveMediaFromStream(bool mediaFeedVisible, string mediaName, Guid objectID)
        {
            HttpPostedFileBase file = ControllerContext.HttpContext.Request.Files["media"]; //android work around
            string contentType = string.Empty;
            if (file != null) { contentType = file.ContentType; }
            else { contentType = ControllerContext.HttpContext.Request.ContentType; }

            Media media = new MediaService().CreateImageMedia(new Media
            {
                FeedVisible = true,
                Title = mediaName,
                TypeID = (byte)MediaType.Image,
                ContentType = contentType
            }, objectID, CurrentInputStream);

            return media;
        }

        [HttpPost]
        public ActionResult AddVisitPhotoMedia(Guid id, string mediaName)
        {
            if (!IsAuthenticated(ControllerContext.HttpContext)) { return new HttpStatusCodeResult(401, "Operation requires authenticated access"); }

            var visit = new VisitsService().GetCheckInById(id);
            if (visit == default(CheckIn)) { return new HttpStatusCodeResult(400, "Cannot find visit"); }

            var media = SaveMediaFromStream(true, mediaName, visit.LocationID);

            new VisitsService().AddMedia(visit, media);
            var by = CfPerfCache.GetClimber(CfIdentity.UserID);

            return Json(new cf.Dtos.Mobile.V1.MediaDto(media, by));
        }

        [HttpPost]
        public ActionResult AddLocationPhotoMedia(Guid id, string mediaName)
        {
            if (!IsAuthenticated(ControllerContext.HttpContext)) { return new HttpStatusCodeResult(401, "Operation requires authenticated access"); }

            var l = new GeoService().GetLocationByID(id);
            if (l == default(Location)) { return new HttpStatusCodeResult(400, "Cannot add location media: Unrecognized location."); }

            var media = new Media();

            //-- If it already just 
            if (l.HasAvatar)
            {
                media = SaveMediaFromStream(true, mediaName, l.ID);
            }
            else
            {
                media = new GeoService().SaveLocationAvatar(l, CurrentInputStream, null);
            }

            var by = CfPerfCache.GetClimber(CfIdentity.UserID);

            return Json(new cf.Dtos.Mobile.V1.MediaDto(media, by));
        }

        [HttpPost]
        public ActionResult AddClimbPhotoMedia(Guid id, string mediaName)
        {
            if (!IsAuthenticated(ControllerContext.HttpContext)) { return new HttpStatusCodeResult(401, "Operation requires authenticated access"); }

            var c = new GeoService().GetClimbByID(id);
            if (c == default(Climb)) { return new HttpStatusCodeResult(400, "Cannot add climb media: Unrecognized climb."); }

            var contentType = ControllerContext.HttpContext.Request.ContentType;

            var media = new Media();
            if (c.HasAvatar) { media = SaveMediaFromStream(c.Type != CfType.ClimbIndoor, mediaName, c.ID); }
            else
            {
                media = new GeoService().SaveClimbAvatar(c, CurrentInputStream, null);
            }

            var by = CfPerfCache.GetClimber(CfIdentity.UserID);

            return Json(new cf.Dtos.Mobile.V1.MediaDto(media, by));
        }

        public class TempImageUploadJsonItem
        {
            public string FileGuid;
            public string FileName;
            public int FileSize;
            public bool Exists;
            public string Error;
        }

        //[HttpPost, CfAuthorize]
        //public ActionResult SaveClimbImages(Guid id, string guidlist)
        //{
        //    var items = new List<TempImageUploadJsonItem>();
        //    var medSvc = new MediaService();
        //    var climb = geoSvc.GetClimbByID(id);

        //    //you can use the MvcUploader.GetUploadedFile in any where
        //    using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(System.Web.HttpContext.Current))
        //    {
        //        foreach (string strguid in guidlist.Split('/'))
        //        {
        //            var item = new TempImageUploadJsonItem();
        //            item.FileGuid = strguid;
        //            CuteWebUI.MvcUploadFile file = uploader.GetUploadedFile(new Guid(strguid));
        //            if (file == null)
        //            {
        //                item.Exists = false;
        //                item.Error = "File not exists";
        //                continue;
        //            }
        //            item.FileName = file.FileName;
        //            item.FileSize = file.FileSize;
        //            //process this item..

        //            using (var stream = file.OpenStream())
        //            {
        //                //you should validate it here:
        //                var media = new cf.Entities.Media() { Title = item.FileName };
        //                if (climb.HasAvatar)
        //                {
        //                    medSvc.CreateImageMedia(media, climb.ID, stream);
        //                }
        //                else
        //                {
        //                    geoSvc.SaveClimbAvatar(climb, stream, null);
        //                }
        //            }

        //            items.Add(item);
        //        }
        //    }

        //    return new JsonResult() { Data = items };
        //}

        protected static bool IsAuthenticated(HttpContextBase context)
        {
            //-- If the WIF pipeline has already authenticated the user just return
            if (context.User.Identity.IsAuthenticated) { return true; }

            //-- Otherwise try to authenticate by swt token in header
            SimpleWebToken swttoken = null;
            return SwtAuthenticate(context, out swttoken).Identity.IsAuthenticated;
        }

        protected static IClaimsPrincipal SwtAuthenticate(HttpContextBase context, out SimpleWebToken swttoken)
        {
            IClaimsIdentity currentIdentiy = context.User.Identity as IClaimsIdentity;
            IClaimsPrincipal incomingPrincipal = context.User as IClaimsPrincipal;

            //if (!incomingPrincipal.Identity.IsAuthenticated)
            //{
            if (new cf.Identity.CfIdentityInflater().TryGetSwtClaimsIdentity(out currentIdentiy, out swttoken))
            {
                incomingPrincipal.Identities[0] = currentIdentiy;
            }
            //}

            return incomingPrincipal;
        }
    }
}
