using NUnit.Framework;
using ThermalTalk;

namespace ThermalTalk.Test.Barcodes
{
    [TestFixture]
    public class ITFTests
    {

        [Test]
        public void TestDefault()
        {
            var barcode = new ITF
            {
                EncodeThis = "1234567890"
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
                0x1D, 0x6B, 0x05,   // ITF Form 1
                (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6',
                (byte) '7', (byte) '8', (byte) '9', (byte) '0', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }

        [Test]
        public void TestEmpty()
        {
            var barcode = new ITF
            {
                EncodeThis = ""
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsEmpty(payload);
        }

        [Test]
        public void TestOddLen()
        {
            var barcode = new ITF
            {
                EncodeThis = "123"
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsEmpty(payload);
        }


        [Test]
        public void TestNonDigit()
        {
            var barcode = new ITF
            {
                EncodeThis = "i'mastring"
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsEmpty(payload);
        }

        [Test]
        public void TestForm2()
        {
            var barcode = new ITF
            {
                EncodeThis = "1234567890",
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
                0x1D, 0x6B, 0x46,   // ITF Form 2
                (byte)barcode.EncodeThis.Length,
                (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6',
                (byte) '7', (byte) '8', (byte) '9', (byte) '0', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }
    }
}
