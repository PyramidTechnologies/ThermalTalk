using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Drawing;
using System.IO;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class PrintImageTests
    {

        private const string baseDir = "test_data";

        [OneTimeSetUp]
        public void Setup()
        {
            var dir = Path.GetDirectoryName(typeof(PrintImageTests).Assembly.Location);
            Directory.SetCurrentDirectory(dir);

            if (Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, true);
            }

            Directory.CreateDirectory(baseDir);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, true);
            }
        }

        [Test()]
        public void ApplyColorInversionTest()
        {
            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, inverted, expected;

            input = Properties.Resources.white_bitmap;
            var path = Path.Combine(baseDir, "white_inverse_test.bmp");
            input.Save(path);
            using (var logo = new PrinterImage(path))
            {

                logo.Resize(input.Width, 0, true);

                Assert.IsFalse(logo.IsInverted);

                logo.ApplyColorInversion();

                inverted = logo.ImageData;
                expected = Properties.Resources.black_bitmap;

                // White should ivnert to black
                Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, inverted));
                Assert.True(logo.IsInverted);

                // Flip back to white, test that the inversion flag is cleared
                logo.ApplyColorInversion();
                Assert.IsFalse(logo.IsInverted);
            };
        }

        [Test()]
        public void ResizeWidthTest()
        {
            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input = Properties.Resources.white_bitmap;
            using (var logo = new PrinterImage(input))
            {
                // Use the RoundUp method since that is done internally
                // in PrinterImage. This is to allow for Assert.AreEqual tests

                // First scale down by 50%
                var shrink = (input.Width / 2).RoundUp(8);
                var expectedH = (input.Height / 2).RoundUp(8);
                logo.Resize(shrink, 0, true);

                Assert.AreEqual(shrink, logo.Width);
                Assert.AreEqual(expectedH, logo.Height);

                // Now double in size
                var grow = (input.Width * 2).RoundUp(8);
                expectedH = (input.Height * 2).RoundUp(8);
                logo.Resize(grow, 0, true);

                Assert.AreEqual(grow, logo.Width);
                Assert.AreEqual(expectedH, logo.Height);
            };
        }

        [Test()]
        public void ResizeHeightTest()
        {
            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input = Properties.Resources.white_bitmap;
            using (var logo = new PrinterImage(input))
            {
                // Use the RoundUp method since that is done internally
                // in PrinterImage. This is to allow for Assert.AreEqual tests

                // First scale down by 50%
                var shrink = (input.Height / 2).RoundUp(8);
                var expectedW = (input.Width / 2).RoundUp(8);
                logo.Resize(0, shrink, true);

                Assert.AreEqual(shrink, logo.Height);
                Assert.AreEqual(expectedW, logo.Width);

                // Now double in size
                var grow = (input.Height * 2).RoundUp(8);
                expectedW = (input.Width * 2).RoundUp(8);
                logo.Resize(0, grow, true);

                Assert.AreEqual(grow, logo.Height);
                Assert.AreEqual(expectedW, logo.Width);
            };
        }


        [Test()]
        public void ResizeNoneTest()
        {
            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input = Properties.Resources.white_bitmap;
            using (var logo = new PrinterImage(input))
            {
                var oldW = logo.Width.RoundUp(8);
                var oldH = logo.Height.RoundUp(8);

                logo.Resize(logo.Width, logo.Height, false);
                Assert.AreEqual(oldW, logo.Width);
                Assert.AreEqual(oldH, logo.Height);
            }
        }

        [Test()]
        public void ResizeZeroExceptionTest()
        {
            Bitmap input = Properties.Resources.white_bitmap;
            using (var logo = new PrinterImage(input))
            {
                Assert.Throws<ImagingException>(() => logo.Resize(0, 0, true));           
            }
            using (var logo = new PrinterImage(input))
            {
                Assert.Throws<ImagingException>(() => logo.Resize(0, 0, false));
            }
        }
    }
}
