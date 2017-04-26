namespace ThermalTalk.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class BasePrintLogo : IDisposable, ThermalTalk.Imaging.IPrintLogo
    {
        /// <summary>
        /// Construct a new logo from source image and scale to ratio.
        /// Set maxWidthPixels to 0 for full size (no change).
        /// </summary>
        /// <param name="sourcePath">String path to file. Supports all image formats.</param>
        /// <param name="maxWidth">Maximum width in pixels to enforce. 0 to ignore.</param>
        /// <param name="maxHeight">Maximum height in pixels to engore. 0 to ignore.</param>
        public BasePrintLogo(string sourcePath, int maxWidth = 0, int maxHeight = 0, int forceWidth = 0)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("sourcePath must be a non-empty string.");
            }

            // MaxWidth must always be byte aligned (units are in pixels)
            MaxWidth = maxWidth.RoundUp(8);
            MaxHeight = maxHeight;
            ForceWidth = forceWidth;

            SetImageData(sourcePath);
        }

        #region Properties
        /// <summary>
        /// Temporary path for this logo that is used to avoid heap allocating
        /// a buffer just to pass into libcore.
        /// </summary>
        public string TransmitPath { get; set; }

        /// <summary>
        /// CRC of logo in transmit mode
        /// </summary>
        public uint TransmitCRC { get; set; }

        /// <summary>
        /// Gets the raw image data
        /// </summary>
        /// <remarks>Private access, use SetImageData</remarks>
        public BitmapImage ImageData { get; private set; }

        /// <summary>
        /// Gets the dimensions for the current state of the image
        /// </summary>
        public LogoSize Dimensions { get; private set; }

        /// <summary>
        /// Gets the ideal width of this image. The ideal
        /// width is the scaled width set at instantiation time.
        /// </summary>
        public int IdealWidth { get; private set; }

        /// <summary>
        /// Gets the ideal height of this image. The ideal
        /// height is the scaled height set at instantiation time.
        /// </summary>
        public int IdealHeight { get; private set; }

        /// <summary>
        /// Gets the enforced max width. Set to 0 to ignore.
        /// </summary>
        public int MaxHeight { get; private set; }

        /// <summary>
        /// Gets the enforced max height. Set to 0 to ignore.
        /// </summary>
        public int MaxWidth { get; private set; }

        /// <summary>
        /// Gets the width to force. Image will be scaled up or down to meet
        /// this requirement. 0 to ignore.
        /// </summary>
        public int ForceWidth { get; private set; }

        /// <summary>
        /// Returns true if this image is inverted
        /// </summary>
        public bool IsInverted { get; private set; }
        #endregion

        public void ApplyDithering(Algorithms algorithm, byte threshhold)
        {
            // Create an instance of the specified dithering algorithm
            var algo = (Algorithms)algorithm;
            var halftoneProcessor = DitherFactory.GetDitherer(algo, threshhold);

            var bitmap = ImageData.ToBitmap();

            // The big grind
            var dithered = halftoneProcessor.GenerateDithered(bitmap);

            // Update ImageData with dithered result
            SetImageData(dithered);
    
            // For Phoenix we don't care about size of logo in flash. Everything is static.
            Dimensions.SizeInBytes = dithered.ToLogoBuffer().Length;
            Dimensions.SizeInBytesOnFlash = Dimensions.SizeInBytes;
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
            var bitmap = ImageData.ToBitmap();
            bitmap.InvertColorChannels();
            SetImageData(bitmap);
        }

        /// <summary>
        /// Save the current state of this logo as a bitmap at the specified path
        /// </summary>
        /// <param name="outpath">Output path</param>
        public void ExportLogo(string outpath)
        {
            ImageData.ToBitmap().Save(outpath);
        }

        /// <summary>
        /// Export the current state of this logo as a binary file at the specific path
        /// </summary>
        /// <param name="outpath">Outpuat path</param>
        public void ExportLogoBin(string outpath)
        {
            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.ToBitmap().ToLogoBuffer();

            // Write to file
            File.WriteAllBytes(outpath, bmpData);
        }

        /// <summary>
        /// Export the current state of this logo as a binary file, wrapped in the 1D 76 
        /// ESC/POS bitmap command.
        /// </summary>
        /// <param name="outpath"></param>
        public void ExportLogoEscPos(string outpath)
        {
            // Build up the ESC/POS 1D 76 30 command
            var buffer = new List<byte>();
            buffer.Add(0x1D);
            buffer.Add(0x76);
            buffer.Add(0x30);

            // Normal width for now
            buffer.Add(0x00);

            // Get correct dimensions
            var w = Dimensions.WidthBytes;
            var h = Dimensions.Height;

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
            var bmpData = ImageData.ToBitmap().ToLogoBuffer();
            buffer.AddRange(bmpData);

            // Write to file
            File.WriteAllBytes(outpath, buffer.ToArray());
        }

        /// <summary>
        /// Returns this logo encoded as a bitmap
        /// </summary>
        /// <returns></returns>
        public string AsBase64String()
        {
            using(var bitmap = ImageData.ToBitmap())
            {
                return bitmap.ToBase64String();
            }            
        }

        /// <summary>
        /// Set the bitmap data from an encoded base64 string
        /// </summary>
        /// <param name="base64">Base64 encoded string</param>
        public void FromBase64String(string base64)
        {
            using(Bitmap bitmap = ImageExt.FromBase64String(base64))
            {
                SetImageData(bitmap);
            }
        }

        /// <summary>
        /// Opens image located at sourcepath. Scales image down to MaxWidth if required.
        /// Images smaller than MaxWidth will not be scaled up. Result is stored in ImageData field.
        /// Final result CRC is calculated and assigned to CRC32 field.
        /// </summary>
        /// <param name="sourcePath">String path to source image</param>
        private void SetImageData(string sourcePath)
        {

            using (Bitmap bitmap = (Bitmap)Image.FromFile(sourcePath))
            {
                // extract dimensions
                var actualWidth = bitmap.Width;
                var actualHeight = bitmap.Height;


                // Adjust width if needed
                if(ForceWidth != 0 && actualWidth != ForceWidth)
                {
                    IdealWidth = ForceWidth;
                }
                else if (MaxWidth != 0 && MaxWidth < actualWidth)
                {
                    IdealWidth = MaxWidth;
                }
                else
                {
                    IdealWidth = actualWidth;
                }


                // Limit height if needed
                if (MaxHeight != 0 && MaxHeight < actualHeight)
                {
                    IdealHeight = MaxHeight;
                }
                else
                {
                    IdealHeight = actualHeight;
                }

                // First, scale width to ideal size
                if (actualWidth > IdealWidth)
                {
                    // Scale down
                    float factor = (float)IdealWidth / (float)actualWidth;
                    actualWidth = (int)(factor * actualWidth);
                    actualHeight = (int)(factor * actualHeight);
                }
                else if (actualWidth < IdealWidth)
                {
                    // Scale up
                    float factor = (float)IdealWidth / (float)actualWidth;
                    actualWidth = (int)(factor * actualWidth);
                    actualHeight = (int)(factor * actualHeight);
                }
                else
                {
                    // Width need not be scaled
                }


                // Second scale height -- down only
                // and don't touch the width, just cut it off
                if (actualHeight > IdealHeight)
                {
                    // Scale down
                    float factor = (float)IdealHeight / (float)actualHeight;
                    actualHeight = (int)(factor * actualHeight);
                }


                // Ensure that whatever width we have is byte aligned
                if (actualWidth % 8 != 0)
                {
                    actualWidth = actualWidth.RoundUp(8);
                }

                // Ensure that our width property matches the final scaled width
                IdealWidth = actualWidth;
                IdealHeight = actualHeight;

                using (Bitmap resized = new Bitmap(bitmap, new Size(IdealWidth, IdealHeight)))
                {
                    SetImageData(resized);
                }
            }
        }

        /// <summary>
        /// Opens image located at sourcepath. Scales image down to MaxWidth if required.
        /// Images smaller than MaxWidth will not be scaled up. Result is stored in ImageData field.
        /// </summary>
        /// <param name="sourcePath">String path to source image</param>>
        private void SetImageData(Bitmap bitmap)
        {
            // Extract dimension info
            Dimensions = new LogoSize();
            Dimensions.Height = bitmap.Height;
            Dimensions.WidthDots = bitmap.Width;
            Dimensions.WidthBytes = (int)Math.Ceiling((double)Dimensions.WidthDots / 8);
            Dimensions.SizeInBytes = bitmap.Height * bitmap.Width;
            Dimensions.SizeInBytesOnFlash = Dimensions.SizeInBytes;

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                ImageData = new BitmapImage();
                ImageData.BeginInit();
                ImageData.StreamSource = memory;
                ImageData.CacheOption = BitmapCacheOption.OnLoad;
                ImageData.EndInit();
            }
        }

        public void Dispose()
        {
            if(ImageData != null)
            {
                ImageData.StreamSource.Close();
            }
        }
    }
}
