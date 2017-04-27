using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class ImageExtTests
    {
        /// <summary>
        /// Given a known bitmap, esnure that it generates the correct colorspace buffer with full opacity
        /// </summary>
        [Test()]
        public void BitmapToBufferTest()
        {
            var inbmp = Properties.Resources.gray_bitmap;
            var expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 128, 128, 128, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = Properties.Resources.white_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 255, 255, 255, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = Properties.Resources.black_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 0, 0, 0, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = Properties.Resources.red_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 0, 0, 255, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = Properties.Resources.green_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 0, 255, 0, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = Properties.Resources.blue_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 255, 0, 0, 255 }, inbmp.Height * inbmp.Width);
            Assert.AreEqual(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

        }

        /// <summary>
        /// Ensure null or empty bitmap are not processed
        /// </summary>
        [Test()]
        public void BitmapToBufferNullTest()
        {
            Bitmap inbmpNull = null;
            var expectedBuff = new byte[0];


            var actualBuff = inbmpNull.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);
        }

        [Test()]
        public void BitmapImageToBitmapTest()
        {
            var bmps = new List<Bitmap>();
            bmps.Add(Properties.Resources.gray_bitmap);
            bmps.Add(Properties.Resources.white_bitmap);
            bmps.Add(Properties.Resources.black_bitmap);
            bmps.Add(Properties.Resources.red_bitmap);
            bmps.Add(Properties.Resources.green_bitmap);
            bmps.Add(Properties.Resources.blue_bitmap);

            foreach (var inbmp in bmps)
            {
                using (var memory = new MemoryStream())
                {
                    inbmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var id = new BitmapImage();
                    id.BeginInit();
                    id.StreamSource = memory;
                    id.CacheOption = BitmapCacheOption.OnLoad;
                    id.EndInit();

                    Assert.IsTrue(ImageTestHelpers.CompareMemCmp(inbmp, id.ToBitmap()));
                }
            }
        }


        /// <summary>
        /// Given a known bitmap, esnure that it generates the correct colorspace buffer with full opacity
        /// </summary>
        [Test()]
        public void BitmapInvertColorChannelsTest()
        {
            var inbmp = Properties.Resources.gray_bitmap;
            var expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 128, 128, 128, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            var actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.white_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 0, 0, 0, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.black_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 255, 255, 255, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.red_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 255, 255, 0, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.green_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 255, 0, 255, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.blue_bitmap;
            expectedBuff = ImageTestHelpers.BGRAGenerator(new byte[] { 0, 255, 255, 255 }, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);
        }


        [Test()]
        public void BitmapToLogoBufferSimpleTest()
        {
            var inbmp = Properties.Resources.gray_bitmap;
            var expectedBuff = Extensions.Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            var actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.white_bitmap;
            expectedBuff = Extensions.Repeated<byte>(0, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.black_bitmap;
            expectedBuff = Extensions.Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.red_bitmap;
            expectedBuff = Extensions.Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.green_bitmap;
            expectedBuff = Extensions.Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);

            inbmp = Properties.Resources.blue_bitmap;
            expectedBuff = Extensions.Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.AreEqual(expectedBuff, actualBuff);
        }

        [Test()]
        public void BitmapToBase64StringTest()
        {
            var bitmap = Properties.Resources.logo_bw;
            var expected = bitmap.ToBuffer();

            var content = bitmap.ToBase64String();
            var restored = ImageExt.FromBase64String(content);

            var actual = restored.ToBuffer();
            Assert.AreEqual(expected, actual);

        }
    }
}
