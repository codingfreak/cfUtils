namespace codingfreaks.cfUtils.Logic.Base.Extensions
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using Standard.Extensions;

    /// <summary>
    /// Provides helper methods for dealing with images.
    /// </summary>
    public static class ImageExtension
    {
        #region methods

        /// <summary>
        /// Gets an area out of an image.
        /// </summary>
        /// <param name="image">The complete image.</param>
        /// <param name="area">The area to crop.</param>
        /// <returns>The cropped area.</returns>
        public static Image CropImage(this Image image, Rectangle area)
        {
            var copy = new Bitmap(image);
            return copy.Clone(area, copy.PixelFormat);
        }

        /// <summary>
        /// Converts the content of an image into a byte array.
        /// </summary>
        /// <remarks>
        /// Uses a memory stream internally.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="format">The image-format of the image.</param>
        /// <returns>An array of bytes describing the image.</returns>
        public static byte[] GetByteArrayFromImage(this Image image, ImageFormat format)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Retrieves the default file name extension for an <paramref name="image"/>.
        /// </summary>
        /// <param name="image">The image to extend.</param>
        /// <param name="throwOnError"><c>true</c> if an exception should be thrown on any codec problem.</param>
        /// <returns>The extension.</returns>
        public static string GetExtension(this Image image, bool throwOnError = false)
        {
            var format = image.RawFormat;
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
            if (codec == null)
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException("Invalid image format.");
                }
                return null;
            }
            var result = codec.FilenameExtension.Split(';').First();
            if (!result.IsNullOrEmpty())
            {
                result = result.Replace("*.", "");
            }
            return result;
        }

        /// <summary>
        /// Converts a byte-array into an image.
        /// </summary>
        /// <remarks>
        /// Uses a memory stream internally.
        /// </remarks>
        /// <param name="bytes">The byte-array containing the image-data.</param>
        /// <returns>An instance of System.Drawing.Image.</returns>
        public static Image GetImageFromByteArray(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Image.FromStream(stream);
            }
        }

        /// <summary>
        /// Retrieves the mime content type for an <paramref name="image"/>.
        /// </summary>
        /// <param name="image">The image to extend.</param>
        /// <param name="throwOnError"><c>true</c> if an exception should be thrown on any codec problem.</param>
        /// <returns>The mime content type label.</returns>
        public static string GetMimeContentType(this Image image, bool throwOnError = false)
        {
            var format = image.RawFormat;
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
            if (codec == null)
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException("Invalid image format.");
                }
                return null;
            }
            return codec.MimeType;
        }

        /// <summary>
        /// Resizes an image to a given size.
        /// </summary>
        /// <param name="original">The image in the old size.</param>
        /// <param name="newSize">The new size.</param>
        /// <returns>The image in the new size.</returns>
        public static Image ResizeImage(this Image original, Size newSize)
        {
            var originalWidth = original.Width;
            var originalHeight = original.Height;
            var percentage = 0f;
            var percentageWidth = 0f;
            var percentageHeight = 0f;

            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

            percentageWidth = (newSize.Width / (float)originalWidth);
            percentageHeight = (newSize.Height / (float)originalHeight);
            percentage = percentageHeight < percentageWidth ? percentageHeight : percentageWidth;
            // calculate new dimensions in pixels
            var newWidth = (int)(originalWidth * percentage);
            var newHeight = (int)(originalHeight * percentage);
            // generate new image
            var bitmap = new Bitmap(newWidth, newHeight);
            var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(original, 0, 0, newWidth, newHeight);
            g.Dispose();
            return bitmap;
        }

        #endregion
    }
}