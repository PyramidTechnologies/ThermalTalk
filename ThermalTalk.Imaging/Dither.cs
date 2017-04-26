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
namespace ThermalTalk.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface IDitherable
    {
        /// <summary>
        /// Number of rows in algorithm matrix
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Numbers of columns in algorithm matrix
        /// </summary>
        int ColCount { get; }

        /// <summary>
        /// Algorithm's divisor
        /// </summary>
        int Divisor { get; }

        /// <summary>
        /// Black or white threshold limit
        /// </summary>
        byte Threshold { get; }

        /// <summary>
        /// Generates a new, dithered version of the input bitmap using the configured
        /// algorithm parameters.
        /// </summary>
        /// <param name="input">Input bitmap</param>
        /// <returns></returns>
        Bitmap GenerateDithered(Bitmap input);
    }

    /// <summary>
    /// Base dithering class
    /// </summary>
    internal class Dither : IDitherable
    {
        // 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private readonly byte[,] _mMatrixPattern;
        private readonly bool _mCanShift;
        private readonly int _mMatrixOffset;

        /// <summary>
        /// Creates an instance of this dithering class
        /// </summary>
        /// <param name="matrixPattern">algorithm in matrix form</param>
        /// <param name="divisor">algorithm divisor</param>
        /// <param name="threshold">threshhold threshold at which a pixel is considered 'black'</param>
        public Dither(byte[,] matrixPattern, int divisor, byte threshold, bool shift=false)
        {
            if (matrixPattern == null)
                throw new ArgumentNullException("matrixPattern must not be null");

            if (divisor == 0)
                throw new ArgumentException("divisor must be non-zero");

            _mMatrixPattern = matrixPattern;
            RowCount = matrixPattern.GetUpperBound(0) + 1;
            ColCount = matrixPattern.GetUpperBound(1) + 1;

            Divisor = divisor;
            Threshold = threshold;

            _mCanShift = shift;

            // Find first non-zero coefficient column in matrix. This value must
            // always be in the first row of the matrix
            for (int i = 0; i < ColCount; i++)
            {
                if (matrixPattern[0, i] != 0)
                {
                    _mMatrixOffset = (byte)(i - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Number of rows in algorithm matrix
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Numbers of columns in algorithm matrix
        /// </summary>
        public int ColCount { get; private set; }

        /// <summary>
        /// Algorithm's divisor
        /// </summary>
        public int Divisor { get; private set; }

        /// <summary>
        /// Black or white threshold limit
        /// </summary>
        public byte Threshold { get; private set; }

        /// <summary>
        /// Create a copy of this bitmap with a dithering algorithm applied
        /// </summary>
        /// <param name="bitmap">Input bitmap</param>
        /// <returns>New, dithered bitmap</returns>
        public virtual Bitmap GenerateDithered(Bitmap bitmap)
        {
            var bmpBuff = bitmap.ToBuffer();
            var pixels = new List<Pixel>();

            // Convert all bytes into pixels
            foreach(var pix in bmpBuff.Split(4))
            {
                pixels.Add(new Pixel(pix));
            }

            // Dither away
            for (int x = 0; x < bitmap.Height; x++)
            {
                for (int y = 0; y < bitmap.Width; y++)
                {
                    var index = x * bitmap.Width + y;
                    var colored = pixels[index];
                    var grayed = ApplyGrayscale(colored);
                    pixels[index] = grayed;

                    ApplySmoothing(pixels, colored, grayed, y, x, bitmap.Width, bitmap.Height);
                }
            }

            // Dump results into output
            var output = new byte[pixels.Count << 2];
            var j = 0;
            for (var i = 0; i < pixels.Count; i++)
            {
                var p = pixels[i];
                output[j++] = p.B;
                output[j++] = p.G;
                output[j++] = p.R;

                // RT-15 - force alpha to be 0xFF because in optimized mode,
                // the .NET client may send strange bitmap data.
                output[j++] = 0xFF;
            }

            return output.AsBitmap(bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Apply grayscale to this pixel and return result
        /// </summary>
        /// <param name="pix">Pixel to transform</param>
        /// <returns>color reduced (grayscale) pixel</returns>
        protected virtual Pixel ApplyGrayscale(Pixel pix)
        {
            // Magic numbers for converting RGB to monochrome space. These achieve a balanced grayscale
            byte grayPoint = (byte)(0.299 * pix.R + 0.587 * pix.G + 0.114 * pix.B);
      
            // Do not alter the alpha channel, otherwise the entire image may go opaque
            Pixel grayed;
            grayed.A = pix.A;

            if (grayPoint < Threshold)
            {
                grayed.R = grayed.G = grayed.B = 0;
            }
            else
            {
                grayed.R = grayed.G = grayed.B = 255;
            }

            return grayed;
        }

        /// <summary>
        /// Apply Dithering algorithm
        /// </summary>
        /// <param name="imageData">image in row-major order to dither against</param>
        /// <param name="colored">Pixel source</param>
        /// <param name="grayed">Pixel source</param>
        /// <param name="x">column position of Pixel</param>
        /// <param name="y">y row position of Pixel</param>
        /// <param name="width">width of imageData</param>
        /// <param name="height">height of imageData</param>
        protected virtual void ApplySmoothing(
            IList<Pixel> imageData,
            Pixel colored,
            Pixel grayed,
            int x,
            int y,
            int width,
            int height)
        {
            int redError = colored.R - grayed.R;
            int blueError = colored.G - grayed.G;
            int greenError = colored.B - grayed.B;

            for (int row = 0; row < RowCount; row++)
            {
                // Convert row to row-major index
                int ypos = y + row;

                for (int col = 0; col < ColCount; col++)
                {
                    int coefficient = _mMatrixPattern[row, col];

                    // Convert col to row-major index
                    int xpos = x + (col - _mMatrixOffset);

                    // Do not process outside of image, 1st row/col, or if pixel is 0
                    if (coefficient != 0 &&
                        xpos > 0 &&
                        xpos < width &&
                        ypos > 0 &&
                        ypos < height)
                    {
                        int offset = ypos * width + xpos;
                        Pixel dithered = imageData[offset];

                        int newR, newG, newB;

                        // Calculate the dither effect on each color channel
                        if (_mCanShift)
                        {
                            newR = (redError * coefficient) >> Divisor;
                            newG = (greenError * coefficient) >> Divisor;
                            newB = (blueError * coefficient) >> Divisor;
                        }
                        else
                        {
                            newR = (redError * coefficient) / Divisor;
                            newG = (greenError * coefficient) / Divisor;
                            newB = (blueError * coefficient) / Divisor;
                        }

                        // Be sure not to overflow
                        dithered.R = safe_tobyte(dithered.R + newR);
                        dithered.G = safe_tobyte(dithered.G + newG);
                        dithered.B = safe_tobyte(dithered.B + newB);

                        // Apply new color
                        imageData[offset] = dithered;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a integer as a byte and handle any over/underflow
        /// </summary>
        /// <param name="val">int</param>
        /// <returns>byte</returns>
        private static byte safe_tobyte(int val)
        {
            if (val < 0)
            {
                val = 0;
            }
            else if (val > 255)
            {
                val = 255;
            }
            return (byte)val;
        }
    }

    /// <summary>
    /// One bpp converts the image to a 1 bit per pixel image
    /// </summary>
    internal class OneBPP : Dither
    {
        public OneBPP(byte threshold)
            : base(new byte[,] { { 0 }}, 1, threshold)
        { }

        /// <summary>
        /// Override returns a 1bpp copy of the input bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public override Bitmap GenerateDithered(Bitmap bitmap)
        {
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            return bitmap.Clone(rectangle, PixelFormat.Format1bppIndexed);
        }
    }


}
