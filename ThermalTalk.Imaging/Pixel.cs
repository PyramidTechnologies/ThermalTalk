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
        /// White is defined as 255,255,255 on the RGB pallete. Returns
        /// true if any RGB value is not 255.
        /// </summary>
        /// <returns>True if this pixel is non-white</returns>
        internal bool IsNotWhite()
        {
            var m = (A + R + G) / 3;
            return m != 255;
        }
    }
}
