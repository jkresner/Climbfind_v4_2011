using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Net;

namespace NetFrameworkExtensions.Net
{
    /// <summary>
    /// Grabs images from the web and returns them in forms (Image, Stream etc.) useful for manipulation in .net code)
    /// </summary>
    public class ImageDownloader
    {
        public Image DownloadImageAsImageObject(string url)
        {
            Image _tmpImage = null;

            using (var webStream = DownloadImageAsStream(url))
            {
                // convert web stream to image
                _tmpImage = Image.FromStream(webStream);
            }

            return _tmpImage;
        }
        
        /// <summary>
        /// Function to download Image from website
        /// </summary>
        /// <param name="url">URL address to download image</param>
        /// <returns>Image</returns>
        public Stream DownloadImageAsStream(string url)
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                // Open a connection
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referrer: (Optional)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                request.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                request.Timeout = 20000;

                // Request response
                using (WebResponse response = request.GetResponse())
                {
                    response.GetResponseStream().CopyTo(ms);
                }
                
                // Open data stream
                return ms;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }
        }
    }
}
;