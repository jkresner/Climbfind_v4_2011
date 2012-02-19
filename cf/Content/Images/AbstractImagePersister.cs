using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using NetFrameworkExtensions.Drawing;

namespace cf.Content.Images
{
    /// <summary>
    /// Base class for saving images to abstract storage
    /// </summary>
    public abstract class AbstractImagePersister : IImagePersister
    {
        /// <summary>
        /// Saves an image to persistent storage at the specified file path with the specified name
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public abstract void SaveImage(Stream stream, string filePath, string fileName);

        /// <summary>
        /// Save image to temporary file location
        /// </summary>
        public void SaveTempImage(Stream stream, string fileName)
        {
            SaveImage(stream, ImageManager.TempPath, fileName);
        }
        
        /// <summary>
        /// Delete an image at the specified file path with the specified name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public abstract void DeleteImage(string filePath, string fileName);
    }
}
