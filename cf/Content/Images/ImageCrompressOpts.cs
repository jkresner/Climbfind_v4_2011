using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace cf.Content.Images
{
    /// <summary>
    /// Conceptual collection of different compression options like codec (JPG, PNG etc) and compression level
    /// </summary>
    public class ImageCrompressOpts
    {
        public ImageFormat Format { get; set; }
        public long CompressionWeight { get; set; }
        public long OriginalByteLengthThreshold { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ImageCrompressOpts() { }

        /// <summary>
        /// Constructor using Jpeg Compression with supplied weight & original image size threshold
        /// </summary>
        public ImageCrompressOpts(long compressionWeight, long originalByteLengthThreshold)
        {
            Format = ImageFormat.Jpeg;
            CompressionWeight = compressionWeight;
            OriginalByteLengthThreshold = originalByteLengthThreshold;
        }

        /// <summary>
        /// User full static properties for easily getting common settings
        /// </summary>
        public static ImageCrompressOpts PngNone { get { return new ImageCrompressOpts() { Format = ImageFormat.Png }; } }
        public static ImageCrompressOpts ImageMedia { get { return new ImageCrompressOpts(60, ImageConstants.Kb105); } }
        public static ImageCrompressOpts TempImage { get { return new ImageCrompressOpts(88, ImageConstants.Kb200); } }
        public static ImageCrompressOpts Avatar240Image { get { return new ImageCrompressOpts(90, ImageConstants.Kb200); } }
    }
}
