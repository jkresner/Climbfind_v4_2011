using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace cf.Content.Images
{
    /// <summary>
    /// Interface useful if eventually swapping out the ImagePersister implementation
    /// </summary>
    /// <remarks>
    /// E.g. moving from AmazonS3 to Azure Blog Storage
    /// </remarks>
    public interface IImagePersister
    {
        void SaveTempImage(Stream stream, string fileName);
        void SaveImage(Stream stream, string filePath, string fileName);
        void DeleteImage(string filePath, string fileName);
    }
}
