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
using cf.Entities.Enum;
using System.IO;
using cf.Identity;

namespace cf.Services
{
    public partial class GeoService
    {
        /// <summary>
        /// No fancy authorization logic, just create the mod profile it it's the users first time & check they're not a bad egg (negative rep)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="categories"></param>
        void CreateClimbAuthorization(Climb obj, List<int> categories)
        {
            SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        ObjectModMeta UpdateClimbAuthorization(Climb original, Climb updated, List<int> categories)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(original);

            if ((original.ID != updated.ID) || (original.CountryID != updated.CountryID))
            {
                throw new ArgumentException(string.Format("Original climb {0}[{1}][{2}] is not the same climb as updated climb {3}[{4}][{5}]",
                    original.Name, original.CountryID, original.ID, updated.Name, updated.CountryID, updated.ID));
            }

            if ((original.Name != updated.Name) && meta.HasBeenVerified)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the name of an climb that has already been verified.");
                }
            }

            if ((original.NameUrlPart != updated.NameUrlPart) && meta.HasBeenVerified)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the url part for a climb that has already been verified.");
                }
            }

            if ((original.Description != updated.Description) && meta.HasBeenVerified && meta.CQR > 6)
            {
                if (!currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the description of a climb that has been verified and has a high CQR.");
                }
            }

            return meta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ObjectModMeta DeleteClimbOutdoorAuthorization(Climb obj)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
            
            var contentRank = meta.CQR;

            if (contentRank > 7 && !currentUser.IsInRole("ModAdmin"))
            {
                throw new AccessViolationException("DeleteArea: Only Admin Moderators can delete climbs with CQR higher than 7");
            }
            else if (contentRank > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("DeleteArea: Only Senior Moderators can delete climbs with CQR higher than 1");
            }
            else if (!currentUser.IsInRole("ModAdmin,ModSenior,ModCommunity"))
            {
                throw new AccessViolationException("DeleteArea: You must be Moderator to delete climbs");
            }

            return meta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="originalImageUrl"></param>
        /// <param name="cropOptions"></param>
        /// <returns></returns>
        public ObjectModMeta SaveClimbAvatarAuthorization(Climb obj, Stream stream, ImageCropOpts cropOptions)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.VerifiedAvatar > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("SaveAreaProfileImage: Only Senior Moderators can change avatar images that have already been verified by other users");
            }

            return meta;
        }
    }
}
