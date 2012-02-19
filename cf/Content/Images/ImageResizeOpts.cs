using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Content.Images
{
    public enum ResizeMode : byte
    {
        ShrinkOnly = 0, //-- If the image is smaller than the desired dimensions then we don't do anything to the image
        ExactWidth = 1, //-- Change the width and height so that the width equals the desired width no matter if the image is bigger or smaller
        ExactHeight = 2, //-- Change the width and height so that the height equals the desired height no matter if the image is bigger or smaller
    }
    
    /// <summary>
    /// Conceptual collection of different resize options like shrink to fit, vs resize to exact dimension.
    /// </summary>
    public class ImageResizeOpts
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ResizeMode Mode { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ImageResizeOpts() { }

        /// <summary>
        /// Constructor with size and mode set
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public ImageResizeOpts(int width, int height, ResizeMode mode)
        {
            Width = width;
            Height = height;
            Mode = mode;
        }

        /// <summary>
        /// Static properties exposing common option sets
        /// </summary>
        public static ImageResizeOpts MediaImage640 { get { return new ImageResizeOpts() { Mode = ResizeMode.ShrinkOnly, Width = 640, Height = 640 }; } }
        public static ImageResizeOpts ProfileAvatar640 { get { return new ImageResizeOpts() { Mode = ResizeMode.ShrinkOnly, Width = 640, Height = 640 }; }}
        public static ImageResizeOpts ObjectAvatar240 { get { return new ImageResizeOpts() { Mode = ResizeMode.ExactWidth, Width = 240, Height = 0 }; } }
    }
}
