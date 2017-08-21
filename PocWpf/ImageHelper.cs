using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PocWpf
{
    public static class ImageHelper
    {
        /// <summary>
        /// Resize image keeping ratio (no distortion)
        /// </summary>
        /// <param name="image">image to resize</param>
        /// <param name="sizeBase">
        /// Size base, eg 100px. If width is bigger, 100px will be assumed for width, 
        /// and height will keep the proportion.
        /// </param>
        /// <returns>new image with the new size</returns>
        public static Image ResizeImage(this Image image, int sizeBase)
        {
            //get the min ratio
            var ratioX = (double)sizeBase / image.Width;
            var ratioY = (double)sizeBase / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            // apply ratio to measurements
            var newWidth = (int)Math.Ceiling(image.Width * ratio);
            var newHeight = (int)Math.Ceiling(image.Height * ratio);

            // build a bitmap with the new sizes
            var newImage = new Bitmap(newWidth, newHeight);

            // paint image into the bitmap
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        /// <summary>convert byte array to image</summary>
        public static Image ToImage(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var image = Image.FromStream(ms);
                return image;
            }
        }

        /// <summary>convert image to byte array</summary>
        public static byte[] ToBytes(this Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();
                return result;
            }
        }

        /// <summary>convert stream to byte array</summary>
        public static byte[] ToBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(ms);
                var result = ms.ToArray();
                return result;
            }
        }

        /// <summary>get image from file path and converts to byte array</summary>
        public static byte[] FileToByteArray(this string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
                }
            }
        }

        /// <summary>resize image from byte array.</summary>
        public static byte[] ResizeImage(byte[] imageBytes, int sizeBase)
        {
            return ResizeImage(imageBytes.ToImage(), sizeBase).ToBytes();
        }

        /// <summary>Get a square of the center of the image </summary>
        public static Image CropSquare(this Image image, int baseSize)
        {
            int newBaseSize;

            // get a base size based of minimal size
            if (image.Width > image.Height)
                newBaseSize = (baseSize * image.Width) / image.Height;
            else
                newBaseSize = (baseSize * image.Height) / image.Width;

            // resize image
            var resizedImage = image.ResizeImage(newBaseSize);

            // dispose old image
            image.Dispose();

            // get the square
            using (var bmpImage = new Bitmap(resizedImage))
            {
                // build the square
                var rectangle = new Rectangle((resizedImage.Width / 2) - (baseSize / 2), (resizedImage.Height / 2) - (baseSize / 2), baseSize, baseSize);
                // dispose this image... it won't be used anymore.
                resizedImage.Dispose();
                
                // paint the image respecting the square.
                var result = bmpImage.Clone(rectangle, bmpImage.PixelFormat);
                return result;
            }
        }
    }
}
