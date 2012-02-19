using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Content.Images
{
    /// <summary>
    /// Collection of top left co-ordinate and width and height of cropping selection which can be used to crop an image
    /// </summary>
    public class ImageCropOpts
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ImageCropOpts() { }

        /// <summary>
        /// Construct options with X,Y,W,H set
        /// </summary>
        /// <param name="x">X co-ordinate of top left corner</param>
        /// <param name="y">Y co-ordinate of top left corner</param>
        /// <param name="w">Width of selection in px</param>
        /// <param name="h">Height of selection in px</param>
        /// <remarks>
        /// Really just for nice syntax in code so we can new up an initialized object
        /// </remarks>
        public ImageCropOpts(int x, int y, int w, int h) 
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
