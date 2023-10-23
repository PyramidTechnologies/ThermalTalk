using NUnit.Framework;

namespace ThermalTalk.Imaging.Test
{
    public class PixelTest
    {
        [Test]
        public void InitializePixel()
        {
            var pxDefault = new Pixel();
            Assert.Multiple(() =>
            {
                Assert.That(pxDefault.A, Is.EqualTo(0));
                Assert.That(pxDefault.R, Is.EqualTo(0));
                Assert.That(pxDefault.G, Is.EqualTo(0));
                Assert.That(pxDefault.B, Is.EqualTo(0));
            });

            var pxByteCreated = new Pixel((byte)0, (byte)1, (byte)2, (byte)3);
            Assert.Multiple(() =>
            {
                Assert.That(pxByteCreated.A, Is.EqualTo(0));
                Assert.That(pxByteCreated.R, Is.EqualTo(1));
                Assert.That(pxByteCreated.G, Is.EqualTo(2));
                Assert.That(pxByteCreated.B, Is.EqualTo(3));
            });

            var pxIntCreated = new Pixel(0, 1, 2, 3);
            Assert.Multiple(() =>
            {
                Assert.That(pxIntCreated.A, Is.EqualTo(0));
                Assert.That(pxIntCreated.R, Is.EqualTo(1));
                Assert.That(pxIntCreated.G, Is.EqualTo(2));
                Assert.That(pxIntCreated.B, Is.EqualTo(3));
            });

            var pxArrayCreated = new Pixel(new byte[] { 3, 2, 1, 0 });
            Assert.Multiple(() =>
            {
                Assert.That(pxArrayCreated.A, Is.EqualTo(0));
                Assert.That(pxArrayCreated.R, Is.EqualTo(1));
                Assert.That(pxArrayCreated.G, Is.EqualTo(2));
                Assert.That(pxArrayCreated.B, Is.EqualTo(3));
            });
        }

        [Test, Sequential]
        public void IsPixelNotWhite(
            [Values(
                new byte[] { 0, 0, 0, 0 },
                new byte[] { 255, 0, 0, 0 },
                new byte[] { 0, 255, 0, 0 },
                new byte[] { 0, 0, 255, 0 },
                new byte[] { 0, 0, 0, 255 },
                new byte[] { 255, 255, 255, 255 },
                new byte[] { 255, 255, 255, 0 })]
            byte[] bgra,
            [Values(
                true,
                true,
                true,
                true,
                true,
                false,
                false)]
            bool isNotWhite)
        {
            var input = new Pixel(bgra);
            Assert.That(input.IsNotWhite(), Is.EqualTo(isNotWhite));
        }
    }
}