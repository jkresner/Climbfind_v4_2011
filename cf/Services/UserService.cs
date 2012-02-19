using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Content.Images;
using cf.Identity;
using cf.Entities.Enum;
using cf.Dtos;
using NetFrameworkExtensions.Net;
using System.Web.Security;
using cf.Instrumentation;
using cf.Caching;

namespace cf.Services
{
    public partial class UserService : AbstractCfService
    {
        ImageManager imgManager { get { if (_imagManager == null) { _imagManager = new ImageManager(); } return _imagManager; } } ImageManager _imagManager;
        ProfileRepository profileRepo { get { if (_profileRepo == null) { _profileRepo = new ProfileRepository(); } return _profileRepo; } } ProfileRepository _profileRepo;
        
        public UserService() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgStream"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> GetHomePageProfileThumbs()
        {
            var profileThumbs = CfPerfCache.SessionGetFromCache<List<string>>("home-profiles-thumbs");
            if (profileThumbs == null)
            {
                profileThumbs = profileRepo.GetAll().Where(p => !string.IsNullOrEmpty(p.Avatar)).Select(p => p.Avatar).ToList();
                CfPerfCache.SessionAddToCache("home-profiles-thumbs", profileThumbs, CfPerfCache.SixtyMinTimeSpan);
            }

            return profileThumbs;
        }


        /// <summary>
        /// User for Cf3 and facebook pics
        /// </summary>
        /// <param name="imgStream"></param>
        /// <param name="cropOpts"></param>
        /// <returns></returns>
        public string SaveProfileAvatarPicFrom3rdPartSource(Stream imgStream, Profile user)
        {
            var imgID = Guid.NewGuid().ToString().Substring(0, 12);
            string fileName = string.Format("{0}.jpg", imgID);
            
            {
                imgManager.ProcessAndSaveImageFromStream(imgStream, ImageManager.UserMainPic240Path, fileName, null, ImageResizeOpts.ObjectAvatar240, new ImageCrompressOpts(90, ImageConstants.Kb50), new ImageCropOpts(0, 0, 0, 320));
                imgManager.SaveThumb40x40_HighCompressed(imgStream, ImageManager.UserMainPicTmPath, fileName, null);

                user.Avatar = fileName;
                profileRepo.Update(user);
            }

            return user.Avatar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImageUrl"></param>
        /// <param name="cropOpts"></param>
        /// <returns></returns>
        /// <remarks>Overload was added for creating/migrating cf3 accounts while the user isn't yet on the thread</remarks>
        public string SaveProfileAvatarPic(Stream imgStream, ImageCropOpts cropOpts)
        {
            var user = profileRepo.GetByID(CfIdentity.UserID);
      
            //-- Save (4) different version of our avatar (2) inside of personality media
            string fileName = SavePersonalityMediaImage(user.ID, imgStream, user.DisplayName + " Avatar", PersonalityCategory.Avatar, cropOpts);

            imgManager.ProcessAndSaveImageFromStream(imgStream, ImageManager.UserMainPic240Path, fileName, null, ImageResizeOpts.ObjectAvatar240, new ImageCrompressOpts(90, ImageConstants.Kb50), new ImageCropOpts(0,0,0,320));
            imgManager.SaveThumb40x40_HighCompressed(imgStream, ImageManager.UserMainPicTmPath, fileName, cropOpts);

            user.Avatar = fileName;
            profileRepo.Update(user);
            
            return user.Avatar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        public string SavePersonalityMediaImage(Guid userID, Stream imgStream, string name, PersonalityCategory category, ImageCropOpts cropOpts)
        {
            var media = new Media() { FeedVisible = true, Title = name, ContentType = "image/jpg" };
            new MediaService().CreateImageMedia(media, userID, imgStream);

            new UserPersonalityMediaRepository().Create(
                new UserPersonalityMedia() { ID = Guid.NewGuid(), CategoryID = (byte)category, MediaID = media.ID, UserID = userID });

            return media.Content;
        }

        public string SavePersonalityMediaYouTube(string name, PersonalityCategory category, YouTubeApiResult youTubeData)
        {
            var media = new Media() { Title = name };
            new MediaService().CreateYouTubeMedia(media, currentUser.UserID, youTubeData);

            new UserPersonalityMediaRepository().Create(
                new UserPersonalityMedia() { ID = Guid.NewGuid(), CategoryID = (byte)category, MediaID = media.ID, UserID = currentUserID });

            return media.Content;
        }

        public string SavePersonalityMediaVimeo(string name, PersonalityCategory category, VimeoApiResult youTubeData)
        {
            var media = new Media() { Title = name };
            new MediaService().CreateVimeoMedia(media, currentUser.UserID, youTubeData);

            new UserPersonalityMediaRepository().Create(
                new UserPersonalityMedia() { ID = Guid.NewGuid(), CategoryID = (byte)category, MediaID = media.ID, UserID = currentUserID });

            return media.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        public IQueryable<UserPersonalityMedia> GetAllPersonalityMediaOfUser(Guid userID)
        {
            return new UserPersonalityMediaRepository().GetUsersMedia(userID);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        public UsersPersonalityMediaCollection GetPersonalityMediaCollectionOfUser(Guid userID)
        {
            var personality = new UserPersonalityMediaRepository().GetUsersMedia(userID).ToUsersPersonalityMediaCollection();
            personality.UserID = userID;
            return personality;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        public UserPersonalityMedia GetPersonalityMediaByID(Guid ID)
        {
            return new UserPersonalityMediaRepository().GetByID(ID);
        }
    }
}

