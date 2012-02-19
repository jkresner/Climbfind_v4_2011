using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Entities.Interfaces;
using cf.Content.Images;
using cf.Caching;
using cf.Identity;
using cf.Entities.Enum;
using System.IO;
using NetFrameworkExtensions;
using NetFrameworkExtensions.SqlServer.Types;

namespace cf.Services
{
    public partial class GeoService
    {
        /// <summary>
        /// Only god users can edit countries
        /// </summary>
        void UpdateCountryAuthorization(Country obj)
        {
            //-- Would be good to move this out of the method? W.I.F.??
            if (!currentUser.IsGodUser) { throw new AccessViolationException("UpdateCountry: Only god users can update countries"); }     
        }
        
        /// <summary>
        /// Only god & the system can do a read of all areas
        /// </summary>
        void GetAreasAllAuthorization() 
        {
            //-- Would be good to move this out of the method? W.I.F.??
            if (!currentUser.IsGodOrSystemUser) { throw new AccessViolationException("GetAreasAllAuthorization: User is not god or system."); }     
        }
        
        /// <summary>
        /// Only authenticated users can create areas
        /// </summary>
        /// <param name="obj"></param>
        void CreateAreaAuthorization(Area obj)
        {
            SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        ObjectModMeta UpdateAreaAuthorization(Area original, Area updated) 
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(original);
            
            if ((original.ID != updated.ID)  || (original.CountryID != updated.CountryID))
            {
                throw new ArgumentException(string.Format("Original area {0}[{1}][{2}] is not the same area as updated area {3}[{4}][{5}]", 
                    original.Name, original.CountryID, original.ID, updated.Name, updated.CountryID, updated.ID));
            }

            if ((original.Latitude != updated.Latitude) || (original.Longitude != updated.Longitude))
            {
                if (!currentUser.IsInRole("ModAdmin"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Admin Moderators can change the area center latitude & longitude.");
                }
            }

            if ((original.TypeID != updated.TypeID) && (original.Type == CfType.Province || updated.Type == CfType.Province))
            {
                if (!currentUser.IsInRole("ModAdmin"))
                {
                    throw new AccessViolationException("UpdateArea["+original.ID+"]: Only Admin Moderators can change the area type to & from province.");
                }
            }

            if ((original.Geo.GetWkt() != updated.Geo.GetWkt()))
            {
                if ((updated.Type == CfType.Province) && !currentUser.IsInRole("ModAdmin"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Admin Moderators can change the boundaries of provinces.");
                }
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the boundaries of areas that have already been verified.");
                }
            }

            if (original.GeoReduceThreshold != updated.GeoReduceThreshold)
            {
                if (updated.Type != CfType.Province)
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: you can only change the geo reduce threshold of provinces, because the basic boxes will reduce into a coordinate, which is bad.");
                }
            }

            if ((original.Name != updated.Name) && meta.HasBeenVerified)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the name of an area that have already been verified.");
                }
            }

            if ((original.NameUrlPart != updated.NameUrlPart) && meta.HasBeenVerified)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the url part for an area that has already been verified.");
                }
            }

            if ((original.Description != updated.Description) && meta.HasBeenVerified && meta.CQR > 6)
            {
                if (!currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateArea[" + original.ID + "]: Only Senior Moderators can change the description of an area that has been verified and has a high CQR.");
                }
            }

            return meta;
        }

        ObjectModMeta DeleteAreaAuthorization(Area obj) 
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
            
            if (obj.Type == CfType.Province)
            {
                //-- Would be good to move this out of the method? W.I.F.??
                if (!currentUser.IsGodUser) { throw new AccessViolationException("DeleteArea: Only god users can delete provinces."); }     
            }

            if (meta.CQR > 7 && !currentUser.IsInRole("ModAdmin"))
            {
                throw new AccessViolationException("DeleteArea: Only Admin Moderators can delete places with CQR higher than 7");
            }
            else if (meta.CQR > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("DeleteArea: Only Senior Moderators can delete places with CQR higher than 1");
            }
            else if (!currentUser.IsInRole("ModAdmin,ModSenior,ModCommunity"))
            {
                throw new AccessViolationException("DeleteArea: You must be Moderator to delete places");
            }

            return meta;
        }


        ObjectModMeta SaveAreaAvatarAuthorization(Area obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.VerifiedAvatar > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("SaveAreaProfileImage: Only Senior Moderators can change images that have already been verified by other users"); 
            }
            
            return meta;
        }
    }
}
