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
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class ImageExt
    {
        /// <summary>
        /// Converts into a row-major byte array. If there is no image data
        /// or the image is empty, the resulting array will be empty (zero length).
        /// </summary>
        /// <returns>byte[]</returns>
        public static byte[] ToBuffer(this Bitmap bitmap)
        {
            if (bitmap == null || bitmap.Size.IsEmpty)
            {
                return new byte[0];
            }

            BitmapData bitmapData = null;

            // This rectangle selects the entirety of the source bitmap for locking bits into memory
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            try
            {
                // Acquire a lock on the image data so we can extra into our own byte stream
                // Note: Currently only supports data as 32bit, 4 channel 8-bit color
                bitmapData = bitmap.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppPArgb);

                // Create the output buffer
                int length = Math.Abs(bitmapData.Stride) * bitmapData.Height;
                byte[] results = new byte[length];

                // Copy from unmanaged to managed memory
                IntPtr ptr = bitmapData.Scan0;
                Marshal.Copy(ptr, results, 0, length);

                return results;

            }
            finally
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// Converts this buffer into a 32bpp ARGB bitmap. The width and 
        /// height parameters must be the sum product of the imageData length.
        /// You specifcy a buffer of length 1000, and a width of 10, the height 
        /// must be 100.
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width">Width of resulting bitmap in bytes</param>
        /// <param name="height">Height of resulting bitmap in bytes</param>
        /// <returns>Bitmap instance. Be sure to dispose of it when you're done.</returns>
        public static Bitmap AsBitmap(this byte[] imageData, int width, int height)
        {
            
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
          
            // Lock the entire bitmap
            BitmapData bitmapData = result.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            // Since our data is provided in byte-expanded pixel mode, adjust the width to 4x
            width = result.Width << 2;

#if SAFE

#error Safe mode not implemented

#else
            unsafe
            {
                // Get a pointer to the beginning of the pixel data region
                // The upper-left corner
                int* pixelPtr = (int*)bitmapData.Scan0;

                // Iterate through rows and columns
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col += 4)
                    {
                        int index = row * width + col;
                        int color = imageData[index++] |
                                    imageData[index++] << 8 |
                                    imageData[index++] << 16 |
                                    imageData[index++] << 24;

                        // Set the pixel (fast!)
                        *pixelPtr = color;

                        // Update the pointer
                        pixelPtr++;
                    }
                }
            }
#endif

            // Unlock the bitmap
            result.UnlockBits(bitmapData);

            return result;

        }

        /// <summary>
        /// Inverts the pixels of this bitmap in place. Ignores alpha channel.
        /// </summary>
        /// <param name="bitmapImage"></param>
        public static void InvertColorChannels(this Bitmap bitmapImage)
        {
            var rect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);

            var bmpRO = bitmapImage.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);

            var bmpLen = bmpRO.Stride * bmpRO.Height;
            var bitmapBGRA = new byte[bmpLen];
            Marshal.Copy(bmpRO.Scan0, bitmapBGRA, 0, bmpLen);
            bitmapImage.UnlockBits(bmpRO);

            // Copy ONLY the color channels and invert - black->white, white->black
            for (int i = 0; i < bmpLen; i += 4)
            {
                bitmapBGRA[i] = (byte)(255 - bitmapBGRA[i]);
                bitmapBGRA[i + 1] = (byte)(255 - bitmapBGRA[i + 1]);
                bitmapBGRA[i + 2] = (byte)(255 - bitmapBGRA[i + 2]);
            }

            var bmpWO = bitmapImage.LockBits(
                rect,
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            Marshal.Copy(bitmapBGRA, 0, bmpWO.Scan0, bmpLen);
            bitmapImage.UnlockBits(bmpWO);
        }


        private const string LogoDelimiter = "||||";

        /// <summary>
        /// Converts this image into a base64 encoded string.
        /// </summary>
        /// <param name="bitmapImage">Source image</param>
        /// <returns>string</returns>
        public static string ToBase64String(this Bitmap bitmap)
        {
            // Do not encode null or empty image
            if (bitmap == null || (bitmap.Width == 0 && bitmap.Height == 0))
            {
                return string.Empty;
            }

            // Extract bitmap image into bitmap and save to memory
            using (MemoryStream m = new MemoryStream())
            {
                bitmap.Save(m, ImageFormat.Bmp);
                var imageBytes = m.ToArray();
                
                return Convert.ToBase64String(imageBytes);
            } 
        }

        /// <summary>
        /// Converts base64 encoded string to bitmap.
        /// </summary>
        /// <param name="content">string to convert</param>
        /// <returns>Bitmap or null on error</returns>
        public static Bitmap FromBase64String(string content)
        {
            try
            {
                var raw = Convert.FromBase64String(content);

                // Do not dispose of stream. Bitmap now owns it and will close it when disposed.
                var ms = new MemoryStream(raw, 0, raw.Length);
                return Bitmap.FromStream(ms, true) as Bitmap;                          
            }
            catch
            {                
                return null;
            }
        }

        /// <summary>
        /// Creates an MSB ordered, bit-reversed buffer of the logo data located in this bitmap.
        /// The input data's pixels are reduced into an 8bpp image. That means that 8 PC bitmap
        /// pixels are reduced into 8 bits in the resulting buffer. These bits are delivered in 
        /// reverse order (0x80: 0b10000000 -> 0x00000001).
        /// If ANY of the pixel's RGB values are non-zero, the corresponding bit index will be set
        /// in the output buffer. The alpha channel has no effect on the output buffer.
        /// The bitmap is read from the top left of the bitmap to the bottom right.
        /// </summary>
        /// <param name="bitmapImage">Bitmap</param>
        /// <returns>MSB ordered, bit-reversed Buffer</returns>
        public static byte[] Rasterize(this Bitmap bitmapImage)
        {
            // Define an area in which the bitmap bits will be locked in managed memory
            var rect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);
            var bmpReadOnly = bitmapImage.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);


            var bmpLen = bmpReadOnly.Stride * bmpReadOnly.Height;
            var bmpChannels = new byte[bmpLen];
            Marshal.Copy(bmpReadOnly.Scan0, bmpChannels, 0, bmpLen);
            bitmapImage.UnlockBits(bmpReadOnly);


            // Split into bitmap into N number of rows where each row is
            // as wide as the input bitmap's pixel count.
            var rowWidth = bmpReadOnly.Width;
            var pixels = bmpChannels.Split(bmpReadOnly.Stride);
            var byteWidth = (int)Math.Ceiling((double)rowWidth / 8);
            var tmpBuff = new List<Pixel>();

            // Result buffer - Use array because we use | operator in byte reversal
            var outBuffer = new byte[byteWidth * bmpReadOnly.Height];
            var outIndex = 0;

            // Read 1 row (aka stride) or 4-byte pixels at a time
            foreach (var row in pixels)
            {
                // Read 1st of every 4 bytes from source[colorIndex] in order into temp buffer
                foreach (var pix in row.Split(4))
                {
                    tmpBuff.Add(new Pixel(pix));
                }

                // Reverse the pixel byte, 0b10000010 -> 0x01000001
                for (var set = 0; set < byteWidth; set++)
                {
                    // Max bit tells us what bit to start shifting from
                    var maxBit = Math.Min(7, rowWidth - (set * 8) - 1);

                    // Read up to 8 bytes at a time in LSB->MSB so they are transmitted MSB->LSB to printer
                    // set offset groups into bytes
                    for (int b = maxBit, bb = 0; b >= 0; b--, bb++)
                    {
                        // Read rows right to left
                        var px = tmpBuff[b + (set * 8)];

                        // Firmware black == 1, White == 0
                        outBuffer[outIndex] |= (byte)((px.IsNotWhite() ? 1 : 0) << bb);
                    }

                    // Increments after every byte
                    outIndex++;
                }

                tmpBuff.Clear();
            }


            return outBuffer;
        }    
    }
}
