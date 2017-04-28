using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ThermalTalk.Imaging.Test
{
    public enum ImageConvertResults
    {
        Success,
        ErrToBuffer,
        ErrToBitmap,
    }

    public static class ImageTestHelpers
    {
        public static ImageConvertResults TestBitmapConversion(Bitmap bmp, byte[] expectedBuff)
        {

            var actualBuff = bmp.ToBuffer();
            if (!expectedBuff.SequenceEqual(actualBuff))
            {
                return ImageConvertResults.ErrToBuffer;
            }


            // Now convert back to bitmap
            var expectedBmp = new Bitmap(bmp);
            var actualBmp = actualBuff.AsBitmap(expectedBmp.Width, expectedBmp.Height);

            //expectedBmp.Save(@"C:\temp\expected.bmp");
            //actualBmp.Save(@"C:\temp\actual.bmp");

            if (!CompareMemCmp(expectedBmp, actualBmp))
            {
                return ImageConvertResults.ErrToBitmap;
            }

            return ImageConvertResults.Success;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (!b1.Size.Equals(b2.Size)) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        /// <summary>
        /// Fills a repeated BGRA pattern into a buffer of count bytes. Byte order is
        /// Blue, Green, Red, Alpha
        /// </summary>
        /// <param name="pattern">4-byte BGRA code</param>
        /// <param name="count">Number of times to repeat pattern</param>
        /// <returns>new buffer</returns>
        public static byte[] BGRAGenerator(byte[] pattern, int count)
        {
            if (pattern.Length != 4)
                throw new ArgumentException("pattern length must be 4");

            if (count <= 0)
                throw new ArgumentException("count be greater than zero");

            var result = new byte[pattern.Length * count];

            for (int i = 0; i < result.Length; i += 4)
            {
                Array.Copy(pattern, 0, result, i, 4);
            }

            return result;
        }
    }
}
