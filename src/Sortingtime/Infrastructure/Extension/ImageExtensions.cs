using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Sortingtime.Infrastructure
{
    public static class ImageExtensions
    {
        // http://stackoverflow.com/questions/1922040/resize-an-image-c-sharp

        public static Bitmap ResizeImage(this Bitmap image, Size maxSize)
        {
            var ratioX = (double)maxSize.Width / image.Width;
            var ratioY = (double)maxSize.Height / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            newImage.MakeTransparent();

            using (Graphics sizeGraph = Graphics.FromImage(newImage))
            {
                sizeGraph.CompositingQuality = CompositingQuality.HighQuality;
                sizeGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                sizeGraph.SmoothingMode = SmoothingMode.Default;
                sizeGraph.InterpolationMode = InterpolationMode.Default;

                sizeGraph.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
