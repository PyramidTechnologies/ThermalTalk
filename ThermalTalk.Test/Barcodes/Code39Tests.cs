
using NUnit.Framework;
using ThermalTalk;

namespace ThermalTalk.Test.Barcodes
{
    using System;

    [TestFixture]
    public class Code39Tests
    {
        [Test]
        public void TestDefault()
        {
            var barcode = new Code39
            {
                EncodeThis = "Hello World2!"
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsNotEmpty(payload);

            var expected = new byte[]
            {
                0x1D, 0x68, 0x64,   // BarcodeDotHeight
                0x1D, 0x77, 0x02,   // BarcodeWidthMultiplier
                0x1D, 0x48, 0x00,   // HRI Position
                0x1D, 0x66, 0x00,   // BarcodeFont
                0x1D, 0x6B, 0x04,   // Code39 Form 1
                (byte) 'H', (byte) 'e', (byte) 'l', (byte) 'l', (byte) 'o', (byte) ' ',
                (byte) 'W', (byte) 'o', (byte) 'r', (byte) 'l', (byte) 'd', (byte) '2',
                (byte) '!', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }

        [Test]
        public void TestEmpty()
        {
            var barcode = new Code39
            {
                EncodeThis = ""
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsEmpty(payload);
        }

        [Test]
        public void TestForm2()
        {
            var barcode = new Code39
            {
                EncodeThis = "Hello World2!",
                Form = 2,
                HriPosition = HRIPositions.Below,
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsNotEmpty(payload);

            var expected = new byte[]
            {
                0x1D, 0x68, 0x64,   // BarcodeDotHeight
                0x1D, 0x77, 0x02,   // BarcodeWidthMultiplier
                0x1D, 0x48, 0x02,   // HRI Position
                0x1D, 0x66, 0x00,   // BarcodeFont
                0x1D, 0x6B, 0x45,   // Code39 Form 2
                (byte)barcode.EncodeThis.Length,
                (byte) 'H', (byte) 'e', (byte) 'l', (byte) 'l', (byte) 'o', (byte) ' ',
                (byte) 'W', (byte) 'o', (byte) 'r', (byte) 'l', (byte) 'd', (byte) '2',
                (byte) '!', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }
    }
}
