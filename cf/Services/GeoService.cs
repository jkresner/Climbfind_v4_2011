using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using NetFrameworkExtensions;
using cf.Entities.Interfaces;
using cf.Content.Images;
using cf.Caching;
using cf.Identity;
using System.IO;
using cf.Dtos;

namespace cf.Services
{
    public partial class GeoService : AbstractCfService
    {
        ImageManager imgManager { get { if (_imagManager == null) { _imagManager = new ImageManager(); } return _imagManager; } } ImageManager _imagManager;
          
        public GeoService() { }

        //-- Mixed Stuff
        public List<CountrySummary> GetGeoSummary()
        {
            return CfPerfCache.TryGetFromCache<List<CountrySummary>>("CountriesSummary",
                () => new CountrySummaryRepository().GetAll().ToList(), CfPerfCache.SixtyMinCacheItemPolicy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="oldImageName"></param>
        /// <param name="objectNameUrlPart"></param>
        /// <param name="destPath"></param>
        /// <param name="objectDBupdateAction"></param>
        /// <returns></returns>
        private string SaveAvatar240Thumb(Stream stream, string oldImageName, string objectNameUrlPart,
            string destPath, Action<string> objectDBupdateAction, ImageCropOpts cropOpts)
        {
            //-- TODO revise naming convention
            string fileName = string.Format("{0}-{1:MMddhhmmss}.jpg", objectNameUrlPart.Substring(0, 3), DateTime.Now);

            return SaveAvatar240Thumb(stream, oldImageName, objectNameUrlPart,
                destPath, objectDBupdateAction, cropOpts, fileName);
        }

        private string SaveAvatar240Thumb(Stream stream, string oldImageName, string objectNameUrlPart,
            string destPath, Action<string> objectDBupdateAction, ImageCropOpts cropOpts, string newFilename)
        {
            stream.Seek(0, SeekOrigin.Begin);

            imgManager.ProcessAndSaveImageFromStream(stream, destPath, newFilename,
                cropOpts,
                ImageResizeOpts.ObjectAvatar240,
                ImageCrompressOpts.Avatar240Image,
                new ImageCropOpts(0, 0, 0, 300)); //-- After resize if it's too high, chop the bottom

            //-- Update the object in the database
            objectDBupdateAction(newFilename);

            return string.Format(@"{0}{1}{2}", Stgs.ImgsRt, destPath, newFilename);
        }
    }
}
