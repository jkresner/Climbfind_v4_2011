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
using pcWorkRepository = cf.DataAccess.Repositories.PartnerCallNotificationWorkItemRepository;
using PCSubscription = cf.Entities.PartnerCallSubscription;
using PCNWorkItem = cf.Entities.PartnerCallNotificationWorkItem;
using cf.Dtos;
using cf.Instrumentation;
using cf.Mail;
using cf.DataAccess.Azure;
using Microsoft.WindowsAzure.StorageClient;
using cf.Dtos.Cloud;
using System.Net;
using System.IO;


namespace cf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AlertsService
    {
        public static string DefaultSound = "frog.caf";
        
        private static NetworkCredential GetCredentialsByClientTypeID(string clientTypeID)
        {
            if (clientTypeID == "iphone-pg") { return Stgs.PGUrbanairshipCredentials; }
            return Stgs.CFUrbanairshipCredentials;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alertMSG"></param>
        /// <param name="userEmail"></param>
        /// <param name="devicetype"></param>
        /// <iphone>{"aps": {"type":"m", "badge": 1, "alert": "New message from {0}", "sound": "{1}"}, "aliases": ["{2}"]}</iphone>
        public static void MobilePush_MessageAlert(string fromDisplayName, string toUserEmail, string devicetype)
        {
            if (devicetype.StartsWith("iphone-"))
            {
                StringBuilder sb = new StringBuilder("{\"aps\": {\"type\":\"m\", \"badge\": \"1\", \"alert\": \"New message from ");
                sb.Append(fromDisplayName);
                sb.Append("\", \"sound\": \"");
                sb.Append(DefaultSound);
                sb.Append("\"}, \"aliases\": [\"");
                sb.Append(toUserEmail);
                sb.Append("\"] }");

                PutUrbanAirshipNotification(devicetype, sb.ToString());
            }
            //--TODO implement android
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alertMSG"></param>
        /// <param name="toUserEmail"></param>
        /// <param name="devicetype"></param>
        /// <iphone>{"aps": {"type":"c", "badge": "1", "alert": "New comment by Jonathon Kresner", "sound": "frog.caf", "postID": "e180b72d64ab4a0d84eafeb17e07dd3c"}, "aliases": ["jkresner@yahoo.com.au"]}</iphone>
        public static void MobilePush_CommentAlert(Guid postID, string byDisplayName, string toUserEmail, string devicetype)
        {
            if (devicetype.StartsWith("iphone-"))
            {
                StringBuilder sb = new StringBuilder("{\"aps\": {\"type\":\"c\", \"badge\": \"1\", \"alert\": \"New comment by ");
                sb.Append(byDisplayName);
                sb.Append("\", \"sound\": \"");
                sb.Append(DefaultSound);
                sb.Append("\", \"postID\": \"");
                sb.Append(postID.ToString("N"));
                sb.Append("\"}, \"aliases\": [\"");
                sb.Append(toUserEmail);
                sb.Append("\"] }");

                PutUrbanAirshipNotification(devicetype, sb.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alertMSG"></param>
        /// <param name="toUserEmail"></param>
        /// <param name="devicetype"></param>
        /// <iphone>{"aps": {"type":"p", "badge": "1", "alert": "New Partner Call for Brooklyn Boulders", "sound": "frog.caf", "placeID": "2e1fead23b2c41d68d3df5599ad69d12", "geotype": "10"}, "aliases": ["jkresner@yahoo.com.au"]}</iphone>
        public static void MobilePush_PartnerCallAlert(Guid placeID, string placeName, byte geotype, string toUserEmail, string devicetype)
        {
            if (devicetype.StartsWith("iphone-"))
            {
                StringBuilder sb = new StringBuilder("{\"aps\": {\"type\":\"p\", \"badge\": \"1\", \"alert\": \"New Partner Call for ");
                sb.Append(placeName);
                sb.Append("\", \"sound\": \"");
                sb.Append(DefaultSound);
                sb.Append("\", \"placeID\": \"");
                sb.Append(placeID.ToString("N"));
                sb.Append("\", \"geotype\": \"");
                sb.Append(geotype);
                sb.Append("\"}, \"aliases\": [\"");
                sb.Append(toUserEmail);
                sb.Append("\"] }");

                PutUrbanAirshipNotification(devicetype, sb.ToString());
            }
        }

        private static void PutUrbanAirshipNotification(string clientTypeID, string jsonPostData)
        {
            // Create a request using a URL that can receive a post. 
            var request = WebRequest.Create("https://go.urbanairship.com/api/push/");
            request.Credentials = GetCredentialsByClientTypeID(clientTypeID);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonPostData);
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();

            // Display the status.
            //            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            //dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            //StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            //string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            // Clean up the streams.
            //reader.Close();
            //dataStream.Close();
            //var result = response.
            //response.Close();
            //return "200";
        }
    }
}
