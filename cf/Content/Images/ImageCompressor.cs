using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace cf.Content.Images
{
    /// <summary>
    /// Conceptual collection of different compression options like codec (JPG, PNG etc) and compression level
    /// </summary>
    internal static class ImageCompressor
    {
        public static Stream CompressImage(Image original, ImageCrompressOpts o)
        {
            Stream compressedStream = new MemoryStream();
            
            //-- Get first codec match for image format
            ImageCodecInfo encoder = GetEncoder(o.Format);
  
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, o.CompressionWeight);

            new Bitmap(original).Save(compressedStream, encoder, encoderParams);
            
            return compressedStream;
        }

        /// <summary>
        /// Get the encoder for the give image format, though only returns the first match?
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
