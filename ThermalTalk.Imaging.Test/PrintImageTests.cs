using NUnit.Framework;
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

                inverted = logo.ImageData.ToBitmap();
                expected = Properties.Resources.black_bitmap;

                // White should ivnert to black
                Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, inverted));
                Assert.True(logo.IsInverted);

                // Flip back to white, test that the inversion flag is cleared
                logo.ApplyColorInversion();
                Assert.IsFalse(logo.IsInverted);
            };
        }
    }
}
