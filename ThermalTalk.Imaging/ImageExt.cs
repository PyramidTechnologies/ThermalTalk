#region Copyright & License

/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

#endregion

namespace ThermalTalk.Imaging
{
    using SkiaSharp;
    using System;
    using System.Runtime.InteropServices;

    public static class ImageExt
    {
        private const string LogoDelimiter = "||||";

        /// <summary>
        /// Converts bytes into a bitmap.
        /// The width * height should equal byte length / bytes per pixel (i.e. 10 * 10 == 400 / 4).
        /// </summary>
        /// <param name="buffer">Bytes.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="colorType">Interpretation of pixel components.</param>
        /// <param name="alphaType">Interpretation of alpha component.</param>
        /// <returns>
        /// Disposable bitmap if buffer is not null nor empty, and no error occurs; otherwise, null.
        /// </returns>
        public static SKBitmap ToBitmap(this byte[] buffer, int width, int height, SKColorType colorType,
            SKAlphaType alphaType)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            var bitmap = new SKBitmap(width, height, colorType, alphaType);

            if (width * height * bitmap.BytesPerPixel != buffer.Length)
                return null;

            var pixelsPointer = bitmap.GetPixels();
            Marshal.Copy(buffer, 0, pixelsPointer, buffer.Length);

            return bitmap;
        }

        /// <summary>
        /// Inverts the pixels of bitmap in place. Ignores alpha channel.
        /// </summary>
        public static void InvertColorChannels(this SKBitmap bitmap)
        {
            using (var tempBitmap = bitmap.Copy(SKColorType.Rgba8888))
            {
                var buffer = tempBitmap.Bytes;
                for (var i = 0; i < buffer.Length; i++)
                {
                    if (i % 4 == 3)
                        continue;

                    buffer[i] = (byte)(255 - buffer[i]);
                }

                var pixelsPointer = tempBitmap.GetPixels();
                Marshal.Copy(buffer, 0, pixelsPointer, buffer.Length);

                tempBitmap.CopyTo(bitmap, bitmap.ColorType);
            }
        }

        /// <summary>
        /// Converts bitmap into a base64 encoded string.
        /// </summary>
        /// <param name="bitmap">Bitmap.</param>
        /// <param name="format">Encoded format.</param>
        /// <returns>
        /// Encoded string if bitmap is not null nor empty nor invalid, a valid format is passed, and no errors occur;
        /// otherwise an empty string.
        /// </returns>
        /// <remarks>
        /// Valid formats are <see cref="SKEncodedImageFormat.Png" />, <see cref="SKEncodedImageFormat.Jpeg" />,
        /// <see cref="SKEncodedImageFormat.Webp" />, and <see cref="SKEncodedImageFormat.Bmp" />.
        /// If <see cref="SKEncodedImageFormat.Bmp" /> is passed, the bitmap's color type will be converted to
        /// <see cref="SKColorType.Bgra8888" />.
        /// </remarks>
        public static string ToBase64String(this SKBitmap bitmap, SKEncodedImageFormat format)
        {
            if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
                return string.Empty;

            try
            {
                var buffer = bitmap.Encode(format);
                return buffer.Length == 0 ? string.Empty : Convert.ToBase64String(buffer);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts base64 encoded string to bitmap.
        /// </summary>
        /// <returns>
        /// Disposable bitmap if the encoded string is not null nor empty, and no errors occur; otherwise, null.
        /// </returns>
        public static SKBitmap ToBitmap(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            try
            {
                var buffer = Convert.FromBase64String(base64);
                return SKBitmap.Decode(buffer);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a bitmap into data for a thermal printer raster image.
        /// Each bit represents a pixel where 1 is black and 0 is white.
        /// A colored pixel in the bitmap is considered black whilst a white pixel is considered white.
        /// </summary>
        /// <returns>Nonempty byte array if bitmap is not null nor empty; otherwise, an empty array.</returns>
        public static byte[] Rasterize(this SKBitmap bitmap)
        {
            if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
                return Array.Empty<byte>();

            byte[] input;
            if (bitmap.ColorType == SKColorType.Gray8)
                input = bitmap.Bytes;
            else
            {
                using (var convertedBitmap = bitmap.Copy(SKColorType.Gray8))
                    input = convertedBitmap.Bytes;
            }

            var rasterByteWidth = (int)Math.Ceiling((float)bitmap.Width / 8);
            var output = new byte[rasterByteWidth * bitmap.Height];

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixelIndex = y * bitmap.Width + x;
                    var printPixel = input[pixelIndex] < 255 ? 1 : 0;
                    
                    var byteIndex = y * rasterByteWidth + x / 8;
                    output[byteIndex] |= (byte)(printPixel << (7 - x % 8));
                }
            }

            return output;
        }

        /// <summary>
        /// Encodes bitmap using the specified format.
        /// </summary>
        /// <param name="bitmap">Bitmap.</param>
        /// <param name="format">File format used to encode.</param>
        /// <returns>
        /// Encoded bytes if bitmap is not null nor empty, and a valid format is passed; otherwise, an empty array.
        /// </returns>
        public static byte[] Encode(this SKBitmap bitmap, SKEncodedImageFormat format)
        {
            if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
                return Array.Empty<byte>();

            switch (format)
            {
                case SKEncodedImageFormat.Bmp:
                    BmpSharp.Bitmap bmp;
                    if (bitmap.ColorType == SKColorType.Bgra8888)
                        bmp = new BmpSharp.Bitmap(bitmap.Width, bitmap.Height, bitmap.Bytes,
                            BmpSharp.BitsPerPixelEnum.RGBA32);
                    else
                    {
                        using (var convertedBitmap = bitmap.Copy(SKColorType.Bgra8888))
                            bmp = new BmpSharp.Bitmap(convertedBitmap.Width, convertedBitmap.Height,
                                convertedBitmap.Bytes, BmpSharp.BitsPerPixelEnum.RGBA32);
                    }

                    return bmp.GetBmpBytes(true);

                default:
                    return bitmap.Encode(format, 100)?.ToArray() ?? Array.Empty<byte>();
            }
        }
    }
}