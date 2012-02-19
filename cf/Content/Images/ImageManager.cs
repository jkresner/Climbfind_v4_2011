using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using NetFrameworkExtensions.Net;

namespace cf.Content.Images
{
    /// <summary>
    /// Handles all functionality related to manipulating, saving and deleting images
    /// </summary>
    public class ImageManager
    {
        public const string TempPath = "/temp/";
        public const string LogoIndoorPath = "/places/id/";
        public const string ClimbPath = "/places/cl/";
        public const string ClimbinAreaPath = "/places/ar/";
        public const string ClimbingIndoorPath = "/places/id/";
        public const string ClimbingOutdoorPath = "/places/od/";
        public const string MediaPhotosPath = "/media/";
        public const string MediaPhotosTmPath = "/media/tm/";
        public const string UserMainPicPath = "/users/main/";
        public const string UserMainPicTmPath = "/users/mainTm/";
        public const string UserMainPic240Path = "/users/main240/";
        
        /// <summary>
        /// The class responsible for how and where our images are persisted to storage
        /// </summary>
        IImagePersister ImagePersister;

        /// <summary>
        /// Default constructor using default Amazon S3 cloud blog storage
        /// </summary>
        public ImageManager() { ImagePersister = new AwsS3ImagePersister(); }

        /// <summary>
        /// Constructor accepting alternate class for image persistence
        /// </summary>
        public ImageManager(IImagePersister imagePersister) { ImagePersister = imagePersister; }
        
        /// <summary>
        /// Take an existing image somewhere on the internet and save it using the destination path/name, crop and resize options.
        /// </summary>
        /// <param name="originalImgUrl"></param>
        /// <param name="destFilePath"></param>
        /// <param name="destFileName"></param>
        /// <param name="cropOptions"></param>
        /// <param name="resizeOptions"></param>
        /// <remarks>
        /// This method is useful when saving images from "web" (external sites). It is also useful during the upload / cropping process
        /// </remarks>
        public void ProcessAndSaveImageFromWebUrl(string originalImgUrl, string destFilePath, string destFileName, 
            ImageCropOpts cropOptions, ImageResizeOpts resizeOptions, ImageCrompressOpts compressOptions)
        {
            using (Stream imageStream = new ImageDownloader().DownloadImageAsStream(originalImgUrl))
            {
                ProcessAndSaveImageFromStream(imageStream, destFilePath, destFileName, cropOptions, resizeOptions, compressOptions, null);
            }
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destFilePath"></param>
        /// <param name="destFileName"></param>
        /// <param name="cropOptions"></param>
        /// <param name="resizeOptions"></param>
        /// <param name="compressOptions"></param>
        public void ProcessAndSaveImageFromStream(Stream stream, string destFilePath, string destFileName, 
            ImageCropOpts cropOptions, ImageResizeOpts resizeOptions, ImageCrompressOpts compressOptions, ImageCropOpts maxDimensionsAftersize)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using (var image = Image.FromStream(stream))
            {
                //-- Set the processed image to the original image incase we don't apply any cropping or resizing we'll save the image as is.
                Image processedImg = image;
                if (cropOptions != null) { processedImg = ImageCropper.Crop(processedImg, cropOptions); }
                if (resizeOptions != null) { processedImg = ImageResizer.ResizeImage(processedImg, resizeOptions); }

                //-- Clip the width or height if it's still too large after resized
                if (maxDimensionsAftersize != null)
                {
                    if ((maxDimensionsAftersize.W != 0) && (processedImg.Width > maxDimensionsAftersize.W)) {
                        var widthCropOptions = new ImageCropOpts(0, 0, maxDimensionsAftersize.W, processedImg.Height);
                        processedImg = ImageCropper.Crop(processedImg, widthCropOptions);
                    }
                    if ((maxDimensionsAftersize.H != 0) && (processedImg.Height > maxDimensionsAftersize.H)) {
                        var heightCropOptions = new ImageCropOpts(0, 0, processedImg.Width, maxDimensionsAftersize.H);
                        processedImg = ImageCropper.Crop(processedImg, heightCropOptions);
                    }
                }

                //-- TODO: think about putting in a max dimensions crop, where if an image is resized by exact width or height, we clip
                //-- the corresponding height or width
                //if (maxDimensionsAftersize != null) { }

                using (var compressedImgStream = ImageCompressor.CompressImage(processedImg, compressOptions))
                {
                    //-- If there's been no cropping or resizing & the original image size is small enough
                    //-- then we're going to save the original stream to avoid image quality degradation
                    if (processedImg.Height == image.Height && processedImg.Width == processedImg.Width)
                    {
                        bool originalImageSizeUnderThreshold = (stream.Length < compressOptions.OriginalByteLengthThreshold);

                        //-- If it's under the threshold then we just use the original image would compounding the compression
                        if (originalImageSizeUnderThreshold)
                        {
                            stream.CopyTo(compressedImgStream); //http://stackoverflow.com/questions/230128/best-way-to-copy-between-two-stream-instances-c-sharp
                            stream.Position = compressedImgStream.Position = 0;
                            
                            ImagePersister.SaveImage(compressedImgStream, destFilePath, destFileName);
                            return;
                        }
                    }
                    
                    ImagePersister.SaveImage(compressedImgStream, destFilePath, destFileName);
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destFilePath"></param>
        /// <param name="destFileName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <remarks>
        /// Used for profile Avatar thumbs
        /// </remarks>
        public void SaveThumb40x40_HighCompressed(Stream stream, string destFilePath, string destFileName, ImageCropOpts cropOpts)
        {
            var compressOptions = new ImageCrompressOpts(90, ImageConstants.Kb05);
            SaveThumbImageWithResizeThenCrop(stream, destFilePath, destFileName, 40, 40, cropOpts, compressOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destFilePath"></param>
        /// <param name="destFileName"></param>
        /// <remarks>
        /// Used for media thumbs
        /// </remarks>
        public void SaveThumb75x75_MediumCompressed(Stream stream, string destFilePath, string destFileName)
        {
            var compressOptions = new ImageCrompressOpts(45, ImageConstants.Kb05);
            SaveThumbImageWithResizeThenCrop(stream, destFilePath, destFileName, 75, 75, null, compressOptions);
        }

        /// <summary>
        /// Save a thumbnail of an image with x/y dimensions specified by thumbW & thumbH
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destFilePath"></param>
        /// <param name="destFileName"></param>
        /// <param name="thumbW"></param>
        /// <param name="thumbH"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="compressOptions"></param>
        private void SaveThumbImageWithResizeThenCrop(Stream stream, string destFilePath, string destFileName, int thumbW, int thumbH,
             ImageCropOpts cropOpts, ImageCrompressOpts compressOptions)
        {
            var thumbPixelBuffer = 0;
            ImageResizeOpts resizeOpts = new ImageResizeOpts() { Mode = ResizeMode.ShrinkOnly, Height = thumbW + thumbPixelBuffer, Width = thumbH + thumbPixelBuffer };

            stream.Seek(0, SeekOrigin.Begin);
            using (var image = Image.FromStream(stream))
            {
                //-- Set the processed image to the original image incase we don't apply any cropping or resizing we'll save the image as is.
                Image processedImg = image;

                //-- Do the initial crop
                if (cropOpts != null) { processedImg = ImageCropper.Crop(image, cropOpts); }

                //-- If our cropped image dimensions are bigger than the Thumb buffer dimensions
                if (processedImg.Width > resizeOpts.Width && processedImg.Height > resizeOpts.Height)
                {
                    processedImg = ImageResizer.ResizeImage(processedImg, resizeOpts);
                }
                
                //-- In case the image width is smaller than the desired thumb width, we enlarge it
                if (processedImg.Width < thumbW)
                {
                    //-- If the image is smaller we first enlarge the image based of width, if the height is still to small we enlarge it again
                    var enlargeWidthOptions = new ImageResizeOpts() { Mode = ResizeMode.ExactWidth, Height = 0, Width = thumbH };
                    
                    //-- First try the width
                    processedImg = ImageResizer.ResizeImage(processedImg, enlargeWidthOptions);
                }

                //-- In case the image height is smaller than the desired Thumb height, we enlarge it
                if (processedImg.Height < thumbH)
                {
                    ImageResizeOpts enlargeHeightOptions = new ImageResizeOpts() { Mode = ResizeMode.ExactHeight, Height = thumbW, Width = 0 };
                    processedImg = ImageResizer.ResizeImage(processedImg, enlargeHeightOptions);
                }

                //-- If needed, crop the width to be exact
                if (processedImg.Width > thumbW)
                {
                    var widthCropOptions = new ImageCropOpts(0, 0, thumbW, processedImg.Height);
                    processedImg = ImageCropper.Crop(processedImg, widthCropOptions);
                }

                //-- If needed, crop the height to be exact 
                if (processedImg.Height > thumbH)
                {
                    var heightCropOptions = new ImageCropOpts(0, 0, processedImg.Width, thumbH);
                    processedImg = ImageCropper.Crop(processedImg, heightCropOptions);
                }

                //-- Save the thumb compressed heavily
                using (var compressedImage = ImageCompressor.CompressImage(processedImg, compressOptions))
                {
                    ImagePersister.SaveImage(compressedImage, destFilePath, destFileName);
                }
            }
        }
      
        /// <summary>
        /// Save an image to the temp file store and reduce it to reasonable size so we don't kill our server by reducing bandwidth & storage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public void SaveTempImage(Stream stream, string fileName) 
        {
            //-- We using save media images as 640x640 max so we give 720 to allow for some cropping
            var resizeOpts = new ImageResizeOpts(720, 720, ResizeMode.ShrinkOnly);

            ProcessAndSaveImageFromStream(stream, TempPath, fileName, null, resizeOpts, ImageCrompressOpts.TempImage, null); 
        }
        
        /// <summary>
        /// Delete an image
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void DeleteImage(string filePath, string fileName) { ImagePersister.DeleteImage(filePath, fileName); }
    }
}
