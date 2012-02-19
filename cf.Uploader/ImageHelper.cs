using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;

namespace ImageUploader
{
    public class ImageHelper
    {
        public static WriteableBitmap GetImageSource(Stream stream, double maxWidth, double maxHeight)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(stream);

            Image img = new Image();
            //img.Effect = new DropShadowEffect() { ShadowDepth = 0, BlurRadius = 0 };
            img.Source = bmp;

            double scaleX = 1;
            double scaleY = 1;

            if (bmp.PixelHeight > maxHeight)
                scaleY = maxHeight / bmp.PixelHeight;
            if (bmp.PixelWidth > maxWidth)
                scaleX = maxWidth / bmp.PixelWidth;

            // maintain aspect ratio by picking the most severe scale
            double scale = Math.Min(scaleY, scaleX);

            int newWidth = Convert.ToInt32(bmp.PixelWidth * scale);
            int newHeight = Convert.ToInt32(bmp.PixelHeight * scale);
            WriteableBitmap result = new WriteableBitmap(newWidth, newHeight);
            result.Render(img, new ScaleTransform() { ScaleX = scale, ScaleY = scale });
            result.Invalidate();
            return result;
        }
    }
}
