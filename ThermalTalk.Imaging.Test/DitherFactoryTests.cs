using NUnit.Framework;
using System;

namespace ThermalTalk.Imaging.Test
{
    public class DitherFactoryTests
    {
        /// <summary>
        /// Generates various perfect-gray input dithers and compares to them to good, known dither generators (i.e. Photoshop).
        /// </summary>
        [Test]
        public void GenerateDitheredBitmap([Values] Algorithms algorithm)
        {
            // TODO: Find a good False Floyd Stein generator.
            if (algorithm == Algorithms.FloydSteinbergFalse)
                return;

            var fileName = algorithm == Algorithms.None
                ? "gray_bitmap.bmp"
                : $"gray_{Enum.GetName(typeof(Algorithms), algorithm)?.ToLower()}.bmp";
            var expected = ResourceManager.Load(fileName);
        
            var input = ResourceManager.Load("gray_bitmap.bmp");
            var actual = DitherFactory.GetDitherer(algorithm).GenerateDithered(input);

            if (algorithm == Algorithms.None)
            {
                var temp = actual;
                actual = temp.Copy(expected.ColorType);
                temp.Dispose();
            }
        
            Assert.That(actual.Bytes, Is.EqualTo(expected.Bytes));
        }

        [Test]
        public void RejectBadDitherParameters()
        {
            Assert.Multiple(() =>
            {
                // Cannot have a null algorithm matrix.
                Assert.Throws<ArgumentNullException>(() => _ = new Dither(null, 0, 1));
            
                // Cannot allow a zero divisor.
                Assert.Throws<ArgumentException>(() => _ = new Dither(new byte[,] { }, 0, 0));
            });
        }
    }
}