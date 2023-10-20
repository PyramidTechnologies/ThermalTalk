using NUnit.Framework;
using SkiaSharp;
using System;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class ImageExtTests
    {
        [Test, Sequential]
        public void InvertBitmapColors(
            [Values(
                "gray_bitmap.bmp",
                "white_bitmap.bmp",
                "black_bitmap.bmp",
                "red_bitmap.bmp",
                "green_bitmap.bmp",
                "blue_bitmap.bmp")]
            string fileName,
            [Values(
                new byte[] { 127, 127, 127, 255 },
                new byte[] { 0, 0, 0, 255 },
                new byte[] { 255, 255, 255, 255 },
                new byte[] { 255, 255, 0, 255 },
                new byte[] { 255, 0, 255, 255 },
                new byte[] { 0, 255, 255, 255 }
            )]
            byte[] invertedBgra)
        {
            var bitmap = ResourceManager.Load(fileName);

            var expectedBuff = RepeatBgra(invertedBgra, bitmap.Width * bitmap.Height);

            bitmap.InvertColorChannels();
            var actualBuff = bitmap.Bytes;

            Assert.That(actualBuff, Is.EqualTo(expectedBuff));
        }

        [Test]
        public void ConvertBitmapToBase64()
        {
            var bitmap = ResourceManager.Load("logo.bw.bmp");
            var expected = bitmap.Bytes;

            var base64 = bitmap.ToBase64String(SKEncodedImageFormat.Bmp);
            var actual = base64.ToBitmap().Bytes;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void RasterizeBitmap(
            [Values(
                "gray_bitmap.bmp",
                "white_bitmap.bmp",
                "black_bitmap.bmp",
                "red_bitmap.bmp",
                "green_bitmap.bmp",
                "blue_bitmap.bmp")]
            string fileName,
            [Values(
                255,
                0,
                255,
                255,
                255,
                255
            )]
            byte printedPixels)
        {
            var bitmap = ResourceManager.Load(fileName);

            var expectedBuff = Extensions.Repeated(printedPixels, bitmap.Width * bitmap.Height / 8).ToArray();
            var actualBuff = bitmap.Rasterize();

            Assert.That(actualBuff, Is.EqualTo(expectedBuff));
        }

        /// <summary>
        /// Fills a repeated BGRA pattern into a buffer of count bytes.
        /// Byte order is Blue, Green, Red, Alpha.
        /// </summary>
        /// <param name="bgra">4-byte BGRA code.</param>
        /// <param name="count">Number of times to repeat pattern.</param>
        /// <returns>New buffer.</returns>
        private static byte[] RepeatBgra(byte[] bgra, int count)
        {
            if (bgra.Length != 4)
                throw new ArgumentException("pattern length must be 4");

            if (count <= 0)
                throw new ArgumentException("count be greater than zero");

            var result = new byte[bgra.Length * count];

            for (var i = 0; i < result.Length; i += 4)
                Array.Copy(bgra, 0, result, i, 4);

            return result;
        }
    }
}