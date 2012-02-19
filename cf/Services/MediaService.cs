using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using NetFrameworkExtensions;
using NetFrameworkExtensions.Web;
using System.IO;
using cf.Content.Images;
using System.Net;
using System.Xml.Linq;
using NetFrameworkExtensions.Net;
using cf.Dtos;

namespace cf.Services
{
    public partial class MediaService : AbstractCfService
    {
        ImageManager imgManager { get { if (_imagManager == null) { _imagManager = new ImageManager(); } return _imagManager; } } ImageManager _imagManager;
        MediaRepository medRepo { get { if (_medRepo == null) { _medRepo = new MediaRepository(); } return _medRepo; } } MediaRepository _medRepo;
        MediaOpinionRepository medRatingRepo { get { if (_medRatingRepo == null) { _medRatingRepo = new MediaOpinionRepository(); } return _medRatingRepo; } } MediaOpinionRepository _medRatingRepo;

        public MediaService() { }

        public IQueryable<Media> GetLatestMedia(int count)
        {
            return medRepo.GetAll().OrderByDescending(m => m.AddedUtc).Where(m=>m.FeedVisible).Take(count);
        }

        public IQueryable<Media> GetUsersMedia(Guid id)
        {
            return medRepo.GetAll().Where(m => m.AddedByUserID == id && m.FeedVisible).OrderByDescending(m => m.AddedUtc);
        }

        /// <summary>
        /// Used for Add media page (add from library) because we want to show which media items are already tagged to
        /// an object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Media> GetMediaByUserWithObjectRefereces(Guid id)
        {
            return medRepo.GetMediaByUserWithObjectRefereces(id).Where(m => m.FeedVisible).OrderByDescending(m => m.AddedUtc);
        }

        public IQueryable<Media> GetUsersMediaWithOpinions(Guid id)
        {
            return medRepo.GetMediaByUserWithOpinions(id).Where(m => m.FeedVisible).OrderByDescending(m => m.AddedUtc);
        }

        public IQueryable<Media> GetUsersMostRecentMedia(Guid id, int count)
        {
            return medRepo.GetAll().Where(m => m.AddedByUserID == id && m.FeedVisible).OrderByDescending(m => m.AddedUtc).Take(count);
        }

        public IQueryable<Media> GetObjectsMedia(Guid id)
        {
            return medRepo.GetObjectsMedia(id).OrderByDescending(m=>m.Rating);
        }

        public IList<Media> GetObjectsTopMedia(Guid id, int number)
        {
            return medRepo.GetObjectsMedia(id).OrderByDescending(m=>m.Rating).Take(number).ToList();
        }

        public IList<Media> GetObjectsMostRecentMedia(Guid id, int number)
        {
            return medRepo.GetObjectsMedia(id).OrderByDescending(m => m.AddedUtc).Take(number).ToList();
        }

        public Media GetMediaByID(Guid id) { return medRepo.GetByID(id); }
        public Media UpdateMedia(Media obj) { return medRepo.Update(obj); }
        public void DeleteMedia(Media obj) 
        {
            if (obj.AddedByUserID != CfIdentity.UserID & !CfPrincipal.IsGod())
            {
                throw new AccessViolationException("Cannot delete media that was not added by you");
            }

            //var comments = obj.MediaOpinion;
            medRatingRepo.Delete(medRatingRepo.GetAll().Where(r=>r.MediaID == obj.ID).Select(r=>r.ID).ToList());

            medRepo.Delete(obj.ID);
        }

        public Media CreateVimeoMedia(Media obj, Guid objID, VimeoApiResult vimeoResult)
        {
            var imgID = Guid.NewGuid().ToString().Substring(0, 12);
            string fileName = string.Format("{0}.jpg", imgID);

            using (Stream stream = new ImageDownloader().DownloadImageAsStream(vimeoResult.Thumbnail))
            {
                imgManager.SaveThumb75x75_MediumCompressed(stream, ImageManager.MediaPhotosTmPath, fileName);
            }

            obj.FeedVisible = true; //-- Movies are always visible

            obj.TypeID = (byte)MediaType.Vimeo;
            obj.Content = (new VimeoMediaData() { Thumbnail = fileName, VimeoID = vimeoResult.ID }).ToJson();
            obj.ContentType = "application/json";
            obj.Description = vimeoResult.Description;
            obj.Author = vimeoResult.Author;
            obj.TakenDate = DateTime.Parse(vimeoResult.Published).Date;
            
            return CreateMedia(obj, objID);
        }

        public Media CreateYouTubeMedia(Media obj, Guid objID, YouTubeApiResult youTubeResult)
        {
            var imgID = Guid.NewGuid().ToString().Substring(0, 12);
            string fileName = string.Format("{0}.jpg", imgID);

            using (Stream stream = new ImageDownloader().DownloadImageAsStream(youTubeResult.Thumbnail))
            {
                imgManager.SaveThumb75x75_MediumCompressed(stream, ImageManager.MediaPhotosTmPath, fileName);
            }

            obj.FeedVisible = true; //-- Movies are always visible

            obj.TypeID = (byte)MediaType.Youtube;
            obj.Content = (new YouTubeMediaData() { Thumbnail = fileName, YouTubeID = youTubeResult.ID }).ToJson();
            obj.ContentType = "application/json";
            obj.Description = youTubeResult.Description;
            obj.Author = youTubeResult.Author;
            obj.TakenDate = DateTime.Parse(youTubeResult.Published).Date;
            
            return CreateMedia(obj, objID);
        }

        public Media CreateImageMedia(Media obj, Guid objID, Stream stream)
        {
            var imgID = Guid.NewGuid().ToString().Substring(0,12);

            string fileName = string.Format("{0}.jpg", imgID);
            
            imgManager.ProcessAndSaveImageFromStream(stream, ImageManager.MediaPhotosPath, fileName, null, ImageResizeOpts.MediaImage640, ImageCrompressOpts.ImageMedia, null);
            imgManager.SaveThumb75x75_MediumCompressed(stream, ImageManager.MediaPhotosTmPath, fileName);

            obj.TypeID = (byte)MediaType.Image;
            obj.Content = fileName;

            if (string.IsNullOrWhiteSpace(obj.ContentType)) { obj.ContentType = "image/jpg"; }

            return CreateMedia(obj, objID);
        }

        public Media CreateMedia(Media obj, Guid objID) 
        { 
            Guid newId = Guid.NewGuid();
            obj.ID = newId;
            obj.AddedUtc = DateTime.UtcNow;
            obj.AddedByUserID = currentUser.UserID;
            obj.ObjectMedias.Add(new ObjectMedia { MediaID = newId, OnOjectID = objID });
            obj.NameUrlPart = obj.Title.ToUrlFriendlyString();
            return medRepo.Create(obj); 
        }

        public MediaOpinion CreateMediaOpinion(MediaOpinion obj)
        {
            if (!CfIdentity.IsAuthenticated) { throw new AccessViolationException("Cannot create media when anonymous"); }
            
            obj.ID = Guid.NewGuid();
            obj.Utc = DateTime.UtcNow;
            obj.UserID = CfIdentity.UserID;

            var rating = medRatingRepo.Create(obj);
                        
            UpdateMediaOpinionMeta(obj.MediaID);

            return rating;
        }

        public MediaOpinion GetMediaOpinion(Guid id) { return medRatingRepo.GetByID(id); }

        public void DeleteMediaOpinion(MediaOpinion obj)
        {
            if (obj.UserID != CfIdentity.UserID & !CfPrincipal.IsGod())
            {
                throw new AccessViolationException("Cannot delete opinion that was not added by you");
            }

            medRatingRepo.Delete(obj.ID);

            UpdateMediaOpinionMeta(obj.MediaID);
        }

        private void UpdateMediaOpinionMeta(Guid id)
        {
            //-- Should be a more efficient way of updating the media's total rating info
            var media = medRepo.GetByID(id);
            var mediasRatings = media.MediaOpinions;

            media.RatingCount = mediasRatings.Count();
            if (media.RatingCount == 0) { media.Rating = null; }
            else
            {
                media.Rating = mediasRatings.Average(r => r.Rating);
            }

            medRepo.Update(media);
        }

        public YouTubeApiResult GetYouTubeVideoData(string videoID)
        {
            var xmlResponse = new Uri(string.Format(@"http://gdata.youtube.com/feeds/api/videos/{0}?v=2", videoID)).ExecuteRestCall();
            return ParseYouTubeXmlResponse(xmlResponse);
        }
       
        private YouTubeApiResult ParseYouTubeXmlResponse(string xmlText)
        {
            var xml = XElement.Parse(xmlText);
            XNamespace yt = "http://gdata.youtube.com/schemas/2007";
            XNamespace media = "http://search.yahoo.com/mrss/";
            XNamespace atom = "http://www.w3.org/2005/Atom";
            var result = new YouTubeApiResult();
            result.ID = (from c in xml.Descendants(yt + "videoid") select c).First().Value;
            result.Title = (from c in xml.Descendants(atom+"title") select c).First().Value;
            result.Description = (from c in xml.Descendants(media + "description") select c).First().Value;
            result.Published = (from c in xml.Descendants(atom+"published") select c).First().Value;
            result.Thumbnail = (string)(from c in xml.Descendants(media + "thumbnail") select c).First().Attribute("url");
            result.Success = "true";
            var author = (from c in xml.Descendants(atom + "author") select c).First();
            var authorName = author.Descendants(atom + "name").First().Value;
            var authorUri = author.Descendants(atom + "uri").First().Value;
            result.Author = string.Format("<a href='{0}'>{1}</a>", authorUri, authorName);

            return result;
        }

        public VimeoApiResult GetVimeoVideoData(string videoID)
        {
            var xmlResponse = new Uri(string.Format(@"http://vimeo.com/api/v2/video/{0}.xml", videoID)).ExecuteRestCall();
            return ParseVimeoXmlResponse(xmlResponse);
        }

        private VimeoApiResult ParseVimeoXmlResponse(string xmlText)
        {
            var xml = XElement.Parse(xmlText);
            var result = new VimeoApiResult();
            result.ID = (from c in xml.Descendants("id") select c).First().Value;
            result.Title = (from c in xml.Descendants("title") select c).First().Value;
            result.Description = (from c in xml.Descendants("description") select c).First().Value;
            result.Published = (from c in xml.Descendants("upload_date") select c).First().Value;
            result.Thumbnail = (from c in xml.Descendants("thumbnail_medium") select c).First().Value;
            result.Success = "true";
            var authorName = (from c in xml.Descendants("user_name") select c).First().Value;
            var authorUri = (from c in xml.Descendants("user_url") select c).First().Value;
            result.Author = string.Format("<a href='{0}'>{1}</a>", authorUri, authorName);

            return result;
        }

        public ObjectMedia AddMediaTag(Media media, Guid onObjectID)
        {
            var alreadyTagged = media.ObjectMedias.Where(om => om.OnOjectID == onObjectID).Count() > 0;
            
            if (media.AddedByUserID != CfIdentity.UserID & !CfPrincipal.IsGod()) { throw new AccessViolationException("Cannot tag media that was not added by you"); }
            if (alreadyTagged) { throw new AccessViolationException("Cannot tag media that already has tag with objID " + onObjectID); }

            var tag = new ObjectMedia() { MediaID = media.ID, OnOjectID = onObjectID };
            medRepo.AddMediaTag(tag);
            
            return tag;
        }

        public void RemoveMediaTag(Media media, Guid onObjectID)
        {
            var tag = media.ObjectMedias.Where(om => om.OnOjectID == onObjectID).SingleOrDefault();

            if (media.AddedByUserID != CfIdentity.UserID & !CfPrincipal.IsGod()) { throw new AccessViolationException("Cannot untag media that was not added by you"); }
            if (tag == null) { throw new AccessViolationException("Cannot tag media that already has tag with objID " + onObjectID); }

            medRepo.RemoveMediaTag(tag);
        }
    }
}
