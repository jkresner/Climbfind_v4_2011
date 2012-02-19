using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Configuration;
using System.Data.Services.Client;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using NetFrameworkExtensions.Identity;
using NetFrameworkExtensions;
using Climb = cf.Entities.Climb;
using cf.Entities;
using Microsoft.IdentityModel.Claims;

namespace cf.Mobile.Client
{
    partial class Program
    {
        static string token;
        static string uploadSvcUrl, odataSvcUrl, mobileSvcUrl, searchSvcUrl, usernameEmail, idSvrUrl;
        static double lat, lon;
        public static WebClient mobClient { get; set; }

        static void Initialize()
        {
            idSvrUrl = ConfigurationManager.AppSettings["StsUrl"].ToString(); 
            searchSvcUrl = ConfigurationManager.AppSettings["SearchServiceUrl"].ToString(); 
            uploadSvcUrl = ConfigurationManager.AppSettings["UploadServiceUrl"].ToString();
            mobileSvcUrl = ConfigurationManager.AppSettings["MobileServiceUrl"].ToString();
            usernameEmail = ConfigurationManager.AppSettings["UserEmail"].ToString();

            Console.WriteLine("Running as: " + usernameEmail);
            Console.WriteLine("Running against: ");
            Console.WriteLine("idsvr: " + idSvrUrl);
            Console.WriteLine("odata: " + odataSvcUrl);
            Console.WriteLine("mobile: " + mobileSvcUrl);
            Console.WriteLine("upload: " + uploadSvcUrl);
            
            lat = double.Parse(ConfigurationManager.AppSettings["Latitude"].ToString());
            lon = double.Parse(ConfigurationManager.AppSettings["Longitude"].ToString());

            token = RequestSamlTokenByRestCall();
            
            mobClient = new WebClient();
            mobClient.Headers.Add("Latlon", string.Format("{0},{1}", lat, lon));
            mobClient.Headers.Add("cf-Authorization", "cfST="+token);
        }
        
        public static T GetResultFromMobileService<T>(string relativeCallUrl)
        {
            var fullUrl = mobileSvcUrl + relativeCallUrl;
            var jsonResult = mobClient.DownloadString(fullUrl);
            return new JavaScriptSerializer().Deserialize<T>(jsonResult);
        }


        public static T GetResultFromSearcService<T>(string relativeCallUrl)
        {
            var fullUrl = searchSvcUrl + relativeCallUrl;
            var jsonResult = mobClient.DownloadString(fullUrl);
            return new JavaScriptSerializer().Deserialize<T>(jsonResult);
        }

        static void SendUploadRequest(string url, byte[] byteData)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "image/jpg";
            request.ContentLength = byteData.Length;
            request.AllowWriteStreamBuffering = true;
            request.Headers.Add("cf-Authorization", String.Format("cfST={0}", token));
            using (var stream = request.GetRequestStream()) { stream.Write(byteData, 0, byteData.Length); }

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                Console.WriteLine(reader.ReadToEnd());
            }
        }


        private static string RequestSamlTokenByRestCall()
        {
            //-- Trust certificate even though the name and the origin don't match to avoid trust exception.
            if (Environment.MachineName.ToLower() == "jonathon-pc")
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };
            }

            var client = new WebClient();
            client.BaseAddress = idSvrUrl;

            //-- collect email/password login details
            usernameEmail = ConfigurationManager.AppSettings["UserEmail"].ToString();
            var userPass = ConfigurationManager.AppSettings["Password"].ToString();

            //-- stuff {email}:{password} into a string then encode as UTf
            var emailAndPassword = string.Format("{0}:{1}", usernameEmail, userPass);

            Console.WriteLine(emailAndPassword);

            var utf8EncodedBytes = Encoding.UTF8.GetBytes(emailAndPassword);

            //Console.WriteLine("Encoded creds {0}", utf8EncodedBytes);

            var base64EncodedCredentials = Convert.ToBase64String(utf8EncodedBytes);

            Console.WriteLine("Encoded creds {0}", base64EncodedCredentials);

            var authorizationHeaderBody = string.Format("Basic {0}", base64EncodedCredentials);

            //-- Add the authorization header
            client.Headers.Add("Authorization", authorizationHeaderBody);

            //&tokenType=SAML

            //-- Format the relative url
            var relativeGetUrl = string.Format("issue/simple?realm={0}", //&tokenType={1}
                HttpUtility.UrlEncode("http://mobile.climbfind.com/")); //,"http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0"

            //-- Make the call on the whole url: https://accounts.climbfind.com/users/httpIssue.svc/?realm=http://iphone.climbfind.com/
            var tokenResultString = client.DownloadString(relativeGetUrl);

            //-- Extract the token out of stupid .net serialized xml (probably will be fixed in a later release of the security service)
            //var xmlResultString = XElement.Parse(tokenResultString).Value;
            //-- Remove the xml declaration from the token
            //token = xmlResultString.Substring(39, xmlResultString.Length - 39);

            var tokenString = tokenResultString.Replace("wrap_access_token=", "");

            //-- Save the token in memory for future service calls
            return tokenString;
        }
    }
}
