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
    using SkiaSharp;
    
    public interface IPrintLogo : System.IDisposable
    {
        /// <summary>
        /// Inverts the color palette on this bitmap, pixel-by-pixel
        /// </summary>
        void ApplyColorInversion();

        /// <summary>
        /// Applies the specified dithering algorithm to this logo
        /// </summary>
        /// <param name="algorithm">Name of algorithm</param>
        /// <param name="threshhold">Gray threshold. Values below this are considered white</param>
        void ApplyDithering(Algorithms algorithm, byte threshhold);

        /// <summary>
        /// Returns this bitmap, in its current state, as a base64 encoded string
        /// </summary>
        /// <returns></returns>
        string AsBase64String();

        /// <summary>
        /// Observable bitmap image source that represents bitmap in its current state
        /// and will reflect any changes due to resize, dither, or inversion.
        /// </summary>
        SKBitmap ImageData { get; }

        /// <summary>
        /// Width of this bitmap in pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of this bitmap in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Total size of this bitmap in bytes
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Returns true if this bitmap is in an inverted state
        /// </summary>
        bool IsInverted { get; }
    }
}
