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
    using System.Collections.Generic;
    using System.IO;
    
    public class PrinterImage : ThermalTalk.Imaging.IPrintLogo
    {
        /// <summary>
        /// Construct a new logo from source image on disk
        /// </summary>
        /// <param name="sourcePath">String path to file. Supports all image formats.</param>
        public PrinterImage(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("sourcePath must be a non-empty string.");

            SetImageData(sourcePath);
        }

        /// <summary>
        /// Constructs a new logo from source image
        /// </summary>
        /// <param name="source">Source bitmap.</param>
        public PrinterImage(SKBitmap source)
        {
            SetImageData(source);
        }
        
        ~PrinterImage()
        {
            Dispose(false);
        }
        
        #region Properties
        /// <summary>
        /// Returns a read-only version of the backing image data
        /// </summary>
        /// <remarks>Private access, use SetImageData</remarks>
        public SKBitmap ImageData { get; private set; }

        /// <summary>
        /// Gets the current bitmap width in pixels
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the current bitmap height in pixels
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Returns the size in bytes for this bitmap
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Returns true if this image is inverted
        /// </summary>
        public bool IsInverted { get; private set; }
        #endregion
        
        public void ApplyDithering(Algorithms algorithm, byte threshold)
        {
            // Create an instance of the specified dithering algorithm
            var halftoneProcessor = DitherFactory.GetDitherer(algorithm, threshold);

            var bitmap = ImageData;

            // The big grind
            var dithered = halftoneProcessor.GenerateDithered(bitmap);

            // Update ImageData with dithered result
            SetImageData(dithered);
    
            // For Phoenix we don't care about size of logo in flash. Everything is static.
            Size = dithered.Rasterize().Length;
        }

        /// <summary>
        /// Apply color inversion to this image. Inversion is relative to the source
        /// image. The image begins in the non-inverted state. Calling ApplyColorInversion
        /// once wil put this image in the reverted state. Calling it twice will return it to
        /// the non-inverted state, etc.
        /// </summary>
        public void ApplyColorInversion()
        {
            IsInverted = !IsInverted;
            var bitmap = ImageData;
            bitmap.InvertColorChannels();
            SetImageData(bitmap);
        }

        /// <summary>
        /// Save the current state of this logo as a bitmap at the specified path
        /// </summary>
        /// <param name="outpath">Output path</param>
        /// <remarks>Supported file formats are BMP, JPEG, PNG, abd WEBP.</remarks>
        public void ExportLogo(string outpath)
        {
            using (var stream = File.OpenWrite(outpath))
            {
                var extension = Path.GetExtension(outpath);
            
                byte[] buffer;
                switch (extension)
                {
                    case ".bmp":
                        buffer = ImageData.Encode(SKEncodedImageFormat.Bmp);
                        break;
                    case ".jpg":
                    case ".jpeg":
                        buffer = ImageData.Encode(SKEncodedImageFormat.Jpeg);
                        break;
                    case ".png":
                        buffer = ImageData.Encode(SKEncodedImageFormat.Png);
                        break;
                    case ".webp":
                        buffer = ImageData.Encode(SKEncodedImageFormat.Webp);
                        break;
                    default:
                        return;
                }
            
                stream.Write(buffer, 0, buffer.Length);   
            }
        }

        /// <summary>
        /// Export the current state of this logo as a binary file at the specific path
        /// </summary>
        /// <param name="outpath">Outpuat path</param>
        public void ExportLogoBin(string outpath)
        {
            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.Rasterize();

            // Write to file
            File.WriteAllBytes(outpath, bmpData);
        }

        /// <summary>
        /// Package this bitmap as an ESC/POS raster image which is the 
        /// 1D 76 command.
        /// </summary>
        /// <returns>Byte buffer</returns>
        public byte[] GetAsRaster()
        {
            // Build up the ESC/POS 1D 76 30 command
            var buffer = new List<byte>();
            buffer.Add(0x1D);
            buffer.Add(0x76);
            buffer.Add(0x30);

            // Normal width for now
            buffer.Add(0x00);

            // Get correct dimensions
            var w = (int)Math.Ceiling((double)Width / 8);
            var h = Height;

            // https://goo.gl/FFdiZl
            // Calculate xL and xH
            var xH = (byte)(w / 256);
            var xL = (byte)(w - (xH * 256));

            // Calculate yL and yH
            var yH = (byte)(h / 256);
            var yL = (byte)(h - (yH * 256));

            // Pack up these dimensions
            buffer.Add(xL);
            buffer.Add(xH);
            buffer.Add(yL);
            buffer.Add(yH);

            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.Rasterize();
            buffer.AddRange(bmpData);

            return buffer.ToArray();
        }

        /// <summary>
        /// Export the current state of this logo as a binary file, wrapped in the 1D 76 
        /// ESC/POS bitmap command.
        /// </summary>
        /// <param name="outpath"></param>
        public void ExportLogoEscPos(string outpath)
        {
            var buffer = GetAsRaster();

            File.WriteAllBytes(outpath, buffer);
        }

        /// <summary>
        /// Returns this logo encoded as a bitmap
        /// </summary>
        /// <returns></returns>
        public string AsBase64String()
        {
            return ImageData.ToBase64String(SKEncodedImageFormat.Bmp);
        }

        /// <summary>
        /// Set the bitmap data from an encoded base64 string. Data will be null on error.
        /// </summary>
        /// <param name="base64">Base64 encoded string</param>
        public void FromBase64String(string base64)
        {
            using (var bitmap = SKBitmap.Decode(base64))
                SetImageData(bitmap);
        }

        /// <summary>
        /// Resize logo using the specific dimensions. To adjustt a single dimension, set 
        /// maintainAspectRatio to true and the first non-zero parameter. The final width
        /// and height, even if you pass in the current width and height as parmaeters
        /// will always be rounded out to be evenly divisible by 8. For example, if your
        /// image is 60x60 and you call Resize(60,60), the result will end up as 64x64
        /// due to rounding.
        /// </summary>
        /// <example>
        /// myLogo.Resize(100, 0, true);  // Scale to 100 pixels wide and maintain ratio
        /// myLogo.Resize(0, 400, true);  // Scale to 400 pixel tall and maintain ratio
        /// myLogo.Resize(100, 200, true);// Scale to 100 pixels wide because maintainAspectRatio is true and width is 1st
        /// myLogo.Resize(640, 480);      // Scale to 640x480 and don't worry about ratio
        /// </example>
        /// <param name="pixelWidth">Desired width in pixels, non-zero if maintainAspectRatio is false</param>
        /// <param name="pixelHeight">Desired height in pixels, non-zero if maintainAspectRatio is false</param>
        /// <param name="maintainAspectRatio">Keep current ratio</param>
        /// <exception cref="ImagingException">Raised if invalid dimensions are specified</exception>
        public void Resize(int pixelWidth, int pixelHeight, bool maintainAspectRatio=false)
        {
            if (maintainAspectRatio)
            {
                if (pixelWidth > 0)
                {
                    // Get required height scalar
                    var scalar = (float)((float)Width / (float)pixelWidth);

                    // Use scalar reciprocal
                    scalar = 1 / scalar;
                    
                    Width = pixelWidth;                    
                    Height = (int)(Height * scalar);
                } 
                else if (pixelHeight > 0)
                {
                    // Get required width scalar
                    var scalar = (float)((float)Height / (float)pixelHeight);

                    // Use scalar reciprocal
                    scalar = 1 / scalar;                

                    Height = pixelHeight;
                    Width = (int)(Width * scalar);
                }
                else
                {
                    throw new ImagingException("Width or Height must be non-zero");
                }
            }
            else
            {
                if(pixelWidth == 0 || pixelHeight == 0)
                {
                    throw new ImagingException("Width and Height must be non-zero");
                }

                Width = pixelWidth;
                Height = pixelHeight;
            }

            // Ensure we have byte alignment
            Width = Width.RoundUp(8);
            Height = Height.RoundUp(8);

            using (var bitmap = new SKBitmap(Width, Height, ImageData.ColorType, ImageData.AlphaType))
            {
                ImageData.ScalePixels(bitmap, SKFilterQuality.High);
                SetImageData(bitmap);
            }
        }

        /// <summary>
        /// Sets <see cref="ImageData"/>.
        /// Scales image down to MaxWidth if required. Images smaller than MaxWidth will not be scaled up.
        /// Final result CRC is calculated and assigned to CRC32 field.
        /// </summary>
        /// <param name="sourcePath">Path to source image.</param>
        private void SetImageData(string sourcePath)
        {
            using (var bitmap = SKBitmap.Decode(sourcePath))
            {
                if (bitmap == null)
                    throw new ImagingException($"Could not load image at {sourcePath}.");
                    
                SetImageData(bitmap);
            }
        }
        
        /// <summary>
        /// <inheritdoc cref="SetImageData(string)"/>
        /// </summary>
        /// <param name="bitmap">Image bitmap.</param>
        private void SetImageData(SKBitmap bitmap)
        {
            if (bitmap == ImageData)
                return;
            
            // Extract dimensions.
            Width = bitmap.Width;
            Height = bitmap.Height;

            ImageData?.Dispose();
            ImageData = bitmap.Copy();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ImageData?.Dispose();
        }
    }
}
