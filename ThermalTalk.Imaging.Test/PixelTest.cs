using NUnit.Framework;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class PixelTest
    {
        [Test]
        public void PixelCtorTest()
        {
            var pxDefault = new Pixel();
            Assert.IsNotNull(pxDefault);
            Assert.AreEqual(0, pxDefault.A);
            Assert.AreEqual(0, pxDefault.R);
            Assert.AreEqual(0, pxDefault.G);
            Assert.AreEqual(0, pxDefault.B);

            var pxByte = new Pixel((byte)0, (byte)1, (byte)2, (byte)3);
            Assert.IsNotNull(pxDefault);
            Assert.AreEqual(0, pxByte.A);
            Assert.AreEqual(1, pxByte.R);
            Assert.AreEqual(2, pxByte.G);
            Assert.AreEqual(3, pxByte.B);

            var pxInt = new Pixel(0, 1, 2, 3);
            Assert.IsNotNull(pxDefault);
            Assert.AreEqual(0, pxInt.A);
            Assert.AreEqual(1, pxInt.R);
            Assert.AreEqual(2, pxInt.G);
            Assert.AreEqual(3, pxInt.B);

            var pxBytes = new Pixel(new byte[] { 0, 1, 2, 3 });
            Assert.IsNotNull(pxDefault);
            Assert.AreEqual(0, pxInt.A);
            Assert.AreEqual(1, pxInt.R);
            Assert.AreEqual(2, pxInt.G);
            Assert.AreEqual(3, pxInt.B);
        }

        [Test]
        public void PixelWhiteTest()
        {
            Pixel input;

            input = new Pixel(new byte[] { 0, 0, 0, 0 });   // Black/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 255, 0, 0, 0 }); // Black/Full alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 255, 0, 0 }); // Blue/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 0, 255, 0 }); // Green/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 0, 0, 255 }); // Red/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 255, 255, 255, 255 }); // White/Full alpha is white
            Assert.False(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 255, 255, 255 });   // White/0 alpha is white
            Assert.False(input.IsNotWhite());
        }
    }
}
