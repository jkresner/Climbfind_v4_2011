using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetFrameworkExtensions;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using cf.DataAccess.Azure;

namespace cf.Mail
{
    internal static class HtmlBodyGenerator
    {
        public const string Master = "master", Message = "message", Comment = "comment", PartnerCall = "partnercall";
        private static readonly Dictionary<string, string> templateLibrary = new Dictionary<string, string>();
        static string HeaderFooterTemplate;
        private const string BlobContainerName = "cf-email-templates";

        static HtmlBodyGenerator()
        {
            HeaderFooterTemplate = GetTemplateFromfile("master.htm");
            templateLibrary.Add(Message, GetTemplateAsString(Message));
            templateLibrary.Add(Comment, GetTemplateAsString(Comment));
            templateLibrary.Add(PartnerCall, GetTemplateAsString(PartnerCall));
        }

        public static string GetMessageBody(Guid fromID, string fromLink, string fromFullName, string fromThumbPic,
            string message)
        {
            if (string.IsNullOrWhiteSpace(fromThumbPic)) { fromThumbPic = "empty.jpg";  }
            
            //-- note: all links are relative coming in
            return string.Format(templateLibrary[Message], Stgs.WebRt, fromLink, fromFullName, fromThumbPic, message,
                message.GetHtmlParagraph(), fromID);
        }

        public static string GetCommentBody(Guid postID, Guid byID, string byLink, string byFullName, string byThumbPic,
            string comment)
        {
            if (string.IsNullOrWhiteSpace(byThumbPic)) { byThumbPic = "empty.jpg"; }

            //-- note: all links are relative coming in
            return string.Format(templateLibrary[Comment], Stgs.WebRt, byLink, byFullName, postID, byThumbPic, comment,
                comment.GetHtmlParagraph());
        }

        public static string GetPartnerCallBody(Guid pcID, Guid byID, string byLink, string byFullName, 
            string pcPlaceLink, string pcPlaceName, DateTime pcDateTime, string byThumbPic, string comment, string matchingPlaces)
        {
            if (string.IsNullOrWhiteSpace(byThumbPic)) { byThumbPic = "empty.jpg"; }
            var dateTimeString = pcDateTime.AppointmentDateTimeString();
            
            //-- note: all links are relative coming in
            return string.Format(templateLibrary[PartnerCall], Stgs.WebRt, byLink, byFullName,
                byThumbPic, comment, pcPlaceLink, pcPlaceName, dateTimeString, matchingPlaces, comment.GetHtmlParagraph(), pcID);
        }

        /// <summary>
        /// Return the template as a string which we can insert values into using string.format
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private static string GetTemplateAsString(string templateName)
        {
            return string.Format(HeaderFooterTemplate, GetTemplateFromfile(templateName + ".htm"));
        }

        /// <summary>
        /// Private helper method for retrieving the template from a datasource e.g. file system for debugging of azure blob storage when live
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetTemplateFromfile(string fileName)
        {
            if (Stgs.IsDevelopmentEnvironment)
            {
                return File.ReadAllText(Stgs.DebugMailDir + fileName);
            }
            else 
            {
                return new BlobRepository().DownloadBlogAsText(BlobContainerName, fileName)
                    .Remove(0, 1); //-- Try hack to remove BOM character screwing up emails 
            }
        }
    }
}