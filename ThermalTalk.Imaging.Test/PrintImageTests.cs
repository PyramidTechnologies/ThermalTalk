using NUnit.Framework;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class PrintImageTests
    {
        [Test]
        public void ApplyColorInversion()
        {
            var expected = ResourceManager.Load("black_bitmap.bmp");

            var bitmap = ResourceManager.Load("white_bitmap.bmp");
            using (var image = new PrinterImage(bitmap))
            {
                image.Resize(bitmap.Width, 0, true);

                Assert.That(image.IsInverted, Is.False);

                image.ApplyColorInversion();
                var actual = image.ImageData;
                Assert.Multiple(() =>
                {
                    Assert.That(image.IsInverted, Is.True);
                    Assert.That(actual.Bytes, Is.EqualTo(expected.Bytes));
                });

                image.ApplyColorInversion();
                Assert.That(image.IsInverted, Is.False);
            }
        }

        [Test]
        public void Resize(
            [Values]
            bool targetWidth,
            [Values(
                0.5f,
                2.0f
            )]
            float sizeMultiplier)
        {
            var bitmap = ResourceManager.Load("white_bitmap.bmp");
            using (var image = new PrinterImage(bitmap))
            {
                // Use the RoundUp method since that is done internally in PrinterImage.
                var expectedWidth = ((int)(image.Width * sizeMultiplier)).RoundUp(8);
                var expectedHeight = ((int)(image.Height * sizeMultiplier)).RoundUp(8);

                if (targetWidth)
                    image.Resize(expectedWidth, 0, true);
                else
                    image.Resize(0, expectedHeight, true);

                Assert.Multiple(() =>
                {
                    Assert.That(image.Width, Is.EqualTo(expectedWidth));
                    Assert.That(image.Height, Is.EqualTo(expectedHeight));
                });
            }
        }

        [Test]
        public void ResizeToCurrentSize()
        {
            var bitmap = ResourceManager.Load("white_bitmap.bmp");
            using (var image = new PrinterImage(bitmap))
            {
                // Use the RoundUp method since that is done internally in PrinterImage.
                var expectedWidth = bitmap.Width.RoundUp(8);
                var expectedHeight = bitmap.Height.RoundUp(8);

                Assert.Multiple(() =>
                {
                    Assert.That(image.Width, Is.EqualTo(expectedWidth));
                    Assert.That(image.Height, Is.EqualTo(expectedHeight));
                });
            }
        }

        [Test]
        public void ResizeWithInvalidParameters()
        {
            var bitmap = ResourceManager.Load("white_bitmap.bmp");
            using (var image = new PrinterImage(bitmap))
            {
                Assert.Multiple(() =>
                {
                    Assert.Throws<ImagingException>(() => image.Resize(0, 0, true));
                    Assert.Throws<ImagingException>(() => image.Resize(0, 0, false));
                });
            }
        }
    }
}