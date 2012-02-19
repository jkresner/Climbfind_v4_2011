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
    /// Class providing cropping logic for images
    /// </summary>
    /// <remarks>
    /// Used by the ImageManger class
    /// </remarks>
    internal static class ImageCropper
    {
        /// <summary>
        /// Crop an image based on start x, start y, width & height
        /// </summary>
        /// <param name="img"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        /// <remarks>
        /// Stangely for large images the x/y dimensions don't always come in right (e.g.
        /// http://upload.wikimedia.org/wikipedia/commons/a/a8/Vista_de_Madrid_desde_Callao_01.jpg
        /// In these cases we just check and fix the crop dimensions to be the image width & height
        /// </remarks>
        public static Image Crop(Image img, ImageCropOpts o)
        {
            var error = string.Empty;

            if (o == null) { error = "Cannot crop image with null crop options..."; }
            else if (o.W == 0 || o.H == 0) { error = string.Format("Cannot crop image with w[{0}],h[{1}]. Cropping with a 0px dimension is not possible.", o.W, o.H); }
            else if (img.Width < (o.X + o.W) || img.Height < (o.Y + o.H))
            {
                var errorMsg = string.Format("Cannot crop image {0}x{1} with crop options x[{2}],w[{3}] and y[{4}],h[5] as crop dimensions exceed the image dimensions.", img.Width, img.Height, o.X, o.W, o.Y, o.H);
            }

            if (!string.IsNullOrEmpty(error)) { throw new ArgumentException(error); }

            //-- Stop stack overflow exception as per remark above
            if (o.W > img.Width) { o.W = img.Width; }
            if (o.H > img.Height) { o.H = img.Height; }

            var cropArea = new System.Drawing.Rectangle(o.X, o.Y, o.W, o.H);
            return Crop(img, cropArea);
        }

        /// <summary>
        /// Crop an image based on a Rectangle
        /// </summary>
        /// <param name="img"></param>
        /// <param name="cropArea"></param>
        /// <returns></returns>
        private static Image Crop(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }
    }
}
