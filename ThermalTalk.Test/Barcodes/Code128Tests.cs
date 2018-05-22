using NUnit.Framework;
using ThermalTalk;

namespace ThermalTalk.Test.Barcodes
{
    [TestFixture]
    public class Code128Tests
    {

        [Test]
        public void TestDefault()
        {
            var barcode = new Code128
            {
                EncodeThis = "AbC1234567890!"
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
                0x1D, 0x6B, 0x08,   // Code128 Form 1
                0x7B, 0x41,         // Mode A
                (byte) 'A', (byte) 'b', (byte) 'C',
                (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6',
                (byte) '7', (byte) '8', (byte) '9', (byte) '0', (byte) '!', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }

        [Test]
        public void TestEmpty()
        {
            var barcode = new Code128
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
            var barcode = new Code128
            {
                EncodeThis = "AbC1234567890!",
                Form = 2,
                HriPosition = HRIPositions.Above,
                Mode = Code128.Modes.B
            };

            var payload = barcode.Build();
            Assert.NotNull(payload);
            Assert.IsNotEmpty(payload);

            var expected = new byte[]
            {
                0x1D, 0x68, 0x64,   // BarcodeDotHeight
                0x1D, 0x77, 0x02,   // BarcodeWidthMultiplier
                0x1D, 0x48, 0x01,   // HRI Position
                0x1D, 0x66, 0x00,   // BarcodeFont
                0x1D, 0x6B, 0x49,   // ITF Form 2                
                (byte)barcode.EncodeThis.Length,
                0x7B, 0x42,         // Mode B
                (byte) 'A', (byte) 'b', (byte) 'C',
                (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6',
                (byte) '7', (byte) '8', (byte) '9', (byte) '0', (byte) '!', (byte) '\0',
            };
            Assert.AreEqual(expected, payload);
        }
    }
}
