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

namespace cf.Services
{
    public partial class GeoService
    {
        /// <summary>
        /// No fancy authorization logic, just create the mod profile it it's the users first time & check they're not a bad egg (negative rep)
        /// </summary>
        void CreateLocationIndoorAuthorization(LocationIndoor obj)
        {
            SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
        }

        /// <summary>
        /// No fancy authorization logic, just create the mod profile it it's the users first time & check they're not a bad egg (negative rep)
        /// </summary>
        void CreateLocationOutdoorAuthorization(LocationOutdoor obj)
        {
            if (obj.TypeID < 21 || obj.TypeID > 60) 
            {
                var error = string.Format("Cannot create outdoor location {0} with type set to {1} as it's not a valid outdoor location type", obj.Name, obj.Type);
                throw new ArgumentException(error); 
            }
                        
            SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        ObjectModMeta UpdateLocationIndoorAuthorization(LocationIndoor original, LocationIndoor updated)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(original);

            if (original.ID != updated.ID || original.CountryID != updated.CountryID)
            {
                throw new ArgumentException(string.Format("Original indoor location {0}[{1}][{2}] is not the same as updated {3}[{4}][{5}]",
                    original.Name, original.CountryID, original.ID, updated.Name, updated.CountryID, updated.ID));
            }

            if ((original.Latitude != updated.Latitude) || (original.Longitude != updated.Longitude))
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the position of a verified outdoor location.");
                }
            }

            if ((original.Name != updated.Name))
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the name of an outdoor location that has already been verified.");
                }
            }

            if ((original.NameUrlPart != updated.NameUrlPart))
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the url.");
                }
            }

            if ((original.Description != updated.Description) && meta.CQR > 6)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the description of an outdoor location that has been verified and has a high CQR.");
                }
            }

            if (original.Address != updated.Address)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the address.");
                }
            }

            if (original.Website != updated.Website)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationIndoor[" + original.ID + "]: Only Senior Moderators can change the website.");
                }
            }

            return meta;
        }

        ObjectModMeta UpdateLocationOutdoorAuthorization(LocationOutdoor original, LocationOutdoor updated)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(original);

            if ((original.ID != updated.ID) || (original.CountryID != updated.CountryID))
            {
                throw new ArgumentException(string.Format("Original outdoor location {0}[{1}][{2}] is not the same as updated {3}[{4}][{5}]",
                    original.Name, original.CountryID, original.ID, updated.Name, updated.CountryID, updated.ID));
            }

            if ((original.Latitude != updated.Latitude) || (original.Longitude != updated.Longitude))
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationOutdoor[" + original.ID + "]: Only Senior Moderators can change the position of a verified outdoor location.");
                }
            }
                        
            if (original.Name != updated.Name)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationOutdoor[" + original.ID + "]: Only Senior Moderators can change the name of an outdoor location that has already been verified.");
                }
            }

            if ((original.Description != updated.Description) && meta.CQR > 6)
            {
                if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
                {
                    throw new AccessViolationException("UpdateLocationOutdoor[" + original.ID + "]: Only Senior Moderators can change the description of an outdoor location that has been verified and has a high CQR.");
                }
            }

            return meta;
        }


        ObjectModMeta SaveLocationIndoorLogoAuthorization(LocationIndoor obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.VerifiedDetails > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("SaveLocationIndoorLogoImage: Only Senior Moderators can change images that have already been verified by other users");
            }

            return meta;
        }


        ObjectModMeta SaveLocationAvatarAuthorization(Location obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.VerifiedAvatar > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("SaveLocationIndoorClimbingImage: Only Senior Moderators can change images that have already been verified by other users");
            }

            return meta;
        }

        ObjectModMeta SaveLocationOutdoorAvatarAuthorization(LocationOutdoor obj, Stream stream, ImageCropOpts cropOpts)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.VerifiedAvatar > 1 && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("SaveLocationOutdoorClimbingImage: Only Senior Moderators can change images that have already been verified by other users");
            }

            return meta;
        }

        ObjectModMeta DeleteLocationOutdoorAuthorization(LocationOutdoor obj)
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.CQR > 7 && !currentUser.IsInRole("ModAdmin"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: Only Admin Moderators can delete places with CQR higher than 7");
            }
            else if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: Only Senior Moderators can delete places with CQR higher than 1");
            }
            else if (!currentUser.IsInRole("ModAdmin,ModSenior,ModCommunity"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: You must be Moderator to delete places");
            }

            return meta;
        }

        ObjectModMeta DeleteLocationIndoorAuthorization(LocationIndoor obj) 
        {
            var meta = SetModDetailsOnPrincipalAndStopModIfNegativeReputationAndReturnObjectModMeta(obj);

            if (meta.CQR > 7 && !currentUser.IsInRole("ModAdmin"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: Only Admin Moderators can delete places with CQR higher than 7");
            }
            else if (meta.HasBeenVerified && !currentUser.IsInRole("ModAdmin,ModSenior"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: Only Senior Moderators can delete places with CQR higher than 1");
            }
            else if (!currentUser.IsInRole("ModAdmin,ModSenior,ModCommunity"))
            {
                throw new AccessViolationException("DeleteLocationIndoor: You must be Moderator to delete places");
            }

            return meta;
        }
    }
}
