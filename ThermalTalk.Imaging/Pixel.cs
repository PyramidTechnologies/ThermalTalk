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

    internal struct Pixel
    {
        public byte B;	//< Blue
        public byte G;	//< Green
        public byte R;	//< Red
        public byte A;	//< Alpha

        /// <summary>
        /// Unchecked constructor expects a 4-byte input buffer
        /// Order: ARGB
        /// </summary>
        /// <param name="slice4"></param>
        internal Pixel(byte[] slice4)
            : this(slice4[3], slice4[2], slice4[1], slice4[0])
        { }

        /// <summary>
        /// Ordered constructor: ARGB
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>       
		public Pixel(int a, int r, int g, int b)
			: this((byte)a, (byte)r, (byte)g, (byte)b)
        {}

        /// <summary>
        /// Ordered constructor: ARGB
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>   
		public Pixel(byte a, byte r, byte g, byte b)
		{
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// White is defined as 255,255,255 on the RGB palette. Returns
        /// true if any RGB value is not 255.
        /// </summary>
        /// <returns>True if this pixel is non-white</returns>
        internal bool IsNotWhite()
        {
            var m = (B + R + G) / 3;
            return m != 255;
        }
    }
}
