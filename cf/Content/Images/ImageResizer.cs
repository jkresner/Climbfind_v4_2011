using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace cf.Content.Images
{
    /// <summary>
    /// Class providing image resizing function and validation (without compression!)
    /// </summary>
    public static class ImageResizer
    {
        private static object syncLock = new object();

        /// <summary>
        /// Resize a given image based on resize options with checks that the resizing options are valid and do not cause Overflow exceptions etc.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Image ResizeImage(Image img, ImageResizeOpts o)
        {
            var error = string.Empty;
            if (o == null) { error = "Cannot resize image with null resize options..."; }
            else if (o.Mode == ResizeMode.ExactWidth && o.Width == 0) { error = string.Format("Cannot resize image with w[{0}] on width only resize. Resize with a 0px dimension is not possible.", o.Width); }
            else if (o.Mode == ResizeMode.ExactHeight && o.Height == 0) { error = string.Format("Cannot resize image with h[{0}]. Resize with a 0px dimension is not possible.", o.Height); }
            else if (o.Mode == ResizeMode.ShrinkOnly && (o.Height == 0 && o.Width ==0)) { error = string.Format("Cannot resize image with w[{0}]h[{1}]. Resize with a 0px dimension is not possible.", o.Width, o.Height); }

            if (!string.IsNullOrEmpty(error)) { throw new ArgumentException(error); }

            //-- If we're only wanting to shrink the image if it's too big & it's already bigger than the desired dimensions, we don't need to do anything
            if (o.Mode == ResizeMode.ShrinkOnly && (img.Width < o.Width && img.Height < o.Height)) { return img; }
            else if (o.Mode == ResizeMode.ExactWidth) { return GetResizedImage(img, o.Width, 0); }
            else if (o.Mode == ResizeMode.ExactHeight) { return GetResizedImage(img, 0, o.Height); }
            else
            {
                return GetResizedImage(img, o.Width, o.Height);
            }
        }

        //--------------------------------------------------------------------------------//
        /// <summary>
        /// Takes in a temporary image and width and height dimensions. If the temp image
        /// exceeds either the width or height, then the method rescales the image and
        /// saves a new copy into saveTo
        /// </summary>
        /// <param name="originalImgName">Name of the image that the user supplied
        /// i.e. the one that has not been re-sized</param>
        /// <param name="newImgDest">The new img dest.</param>
        /// <param name="desiredWidth">Width of the desired.</param>
        /// <param name="desiredHeight">Height of the desired.</param>
        /// <returns>Float which represents the resizing</returns>
        /// <note>* Firefox takes just the name of the image where as IE takes the whole path</note>
        //--------------------------------------------------------------------------------//
        private static Image GetResizedImage(Image originalImage, int desiredWidth, int desiredHeight)
        {
            Image resized = null;
            float wPercent = 0, hPercent = 0, resize = 1;

            lock (syncLock)
            {
                using (Bitmap originalBitmap = new Bitmap(originalImage))
                {
                    float imgWidth = originalBitmap.Width, imgHeight = originalBitmap.Height;

                    //If width needs to be scaled down, get scaling percentage
                    if (imgWidth > desiredWidth) { wPercent = desiredWidth / imgWidth; }

                    //If height needs to be scales down, get scaling percentage
                    if (imgHeight > desiredHeight) { hPercent = desiredHeight / imgHeight; }

                    //Get the smaller scale down percentage of the two
                    if (wPercent != 0) { resize = wPercent; }
                    if (hPercent != 0)
                    {
                        if (wPercent == 0) { resize = hPercent; }
                        else if (hPercent < wPercent) { resize = hPercent; }
                    }

                    //If no need to resize return the original image    
                    if (resize == 1) { return originalImage; }

                    //Else resize and save the new image
                    else { resized = PreformResize(originalBitmap, resize); }

                    //-- Attempt to Stop generic GDI+ exception
                    GC.Collect();
                }
            }

            return (resized);
        }

        /// <summary>
        /// Resizes the image based on the scale up/down resize percentage without compression
        /// </summary>
        /// <param name="originalBitmap"></param>
        /// <param name="resize"></param>
        /// <returns></returns>
        private static Image PreformResize(Bitmap originalBitmap, float resize)
        {
            int newWidth = (int)(originalBitmap.Width * resize);
            int newHeight = (int)(originalBitmap.Height * resize);

            Bitmap resizedImage = null;

            using (Bitmap resizedBitmap = new Bitmap(newWidth, newHeight))
            {
                //Image thumbnail = resizedBitmap;
                using (Graphics graphic = Graphics.FromImage(resizedBitmap))
                {
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;

                    graphic.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);

                    //-- Note we save it as a memory bitmap without compression as we leave compression to the ImageCompressor class
                    using (Stream stream = new MemoryStream())
                    {
                        //Use high quality for resize and leave compress for ImageCompressorStep
                        ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                        using (EncoderParameters Params = new EncoderParameters(1))
                        {
                            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            resizedBitmap.Save(stream, Info[1], Params);
                        }
                        
                        resizedImage = new Bitmap(resizedBitmap);
                    }
                }
            }

            return resizedImage;    
        }
    }
}
