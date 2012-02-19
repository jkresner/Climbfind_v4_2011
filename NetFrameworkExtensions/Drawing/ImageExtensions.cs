using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace NetFrameworkExtensions.Drawing
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Often we have an Image object after some form of .net manipulation (sizing, cropping etc.) and we then need to send that image
        /// somewhere (like file, or cloud storage) as a stream. 
        /// </summary>
        /// <param name="image">The image as raw bitmap in memory</param>
        /// <param name="format">Format e.g. ImageFormat.Jpeg, ImageFormat.Png. This parameter has HUGE effect on the size and quality of the image</param>
        /// <returns></returns>
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
