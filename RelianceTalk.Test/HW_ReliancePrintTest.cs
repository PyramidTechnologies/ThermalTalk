using NUnit.Framework;

namespace RelianceTalk.Test
{
    [TestFixture()]
    public class HW_ReliancePrintTest
    {
        private const string TEST_PORT = "COM1";

        [Test()]
        public void RealHardwareTests()
        {
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);


            printer.PrintASCIIString("No effects - Left");
            printer.PrintNewline();

            printer.AddEffect(FontEffects.Bold);
            printer.PrintASCIIString("This is bold");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Bold);
            printer.AddEffect(FontEffects.Italic);
            printer.PrintASCIIString("This is italic");
            printer.PrintNewline();


            printer.Reinitialize();
            printer.SetJustification(FontJustification.JustifyCenter);
            printer.PrintASCIIString("No effects - Center");
            printer.PrintNewline();

            printer.AddEffect(FontEffects.Bold);
            printer.PrintASCIIString("This is bold");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Bold);
            printer.AddEffect(FontEffects.Italic);
            printer.PrintASCIIString("This is italic");
            printer.PrintNewline();


            printer.Reinitialize();
            printer.SetJustification(FontJustification.JustifyRight);
            printer.PrintASCIIString("No effects - Right");            
            printer.PrintNewline();

            printer.AddEffect(FontEffects.Bold);
            printer.PrintASCIIString("This is bold");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Bold);
            printer.AddEffect(FontEffects.Italic);
            printer.PrintASCIIString("This is italic");
            printer.PrintNewline();


            printer.Reinitialize();
            printer.PrintASCIIString("Scalars");
            printer.PrintNewline();

            printer.PrintASCIIString("WH1x");
            printer.SetScalars(FontWidthScalar.w2, FontHeighScalar.h2);
            printer.PrintASCIIString("WH2x");
            printer.SetScalars(FontWidthScalar.w3, FontHeighScalar.h3);
            printer.PrintASCIIString("WH3x");
            printer.SetScalars(FontWidthScalar.w4, FontHeighScalar.h4);
            printer.PrintASCIIString("WH4x");
            printer.SetScalars(FontWidthScalar.w5, FontHeighScalar.h5);
            printer.PrintASCIIString("WH5x");
            printer.SetScalars(FontWidthScalar.w6, FontHeighScalar.h6);
            printer.PrintASCIIString("WH6x");
            printer.SetScalars(FontWidthScalar.w7, FontHeighScalar.h7);
            printer.PrintASCIIString("WH7x");
            printer.SetScalars(FontWidthScalar.w8, FontHeighScalar.h8);
            printer.PrintASCIIString("WH8x");
            printer.PrintNewline();

            printer.PrintASCIIString("H1x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h2);
            printer.PrintASCIIString("H2x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h3);
            printer.PrintASCIIString("H3x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h4);
            printer.PrintASCIIString("H4x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h5);
            printer.PrintASCIIString("H5x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h6);
            printer.PrintASCIIString("H6x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h7);
            printer.PrintASCIIString("H7x");
            printer.SetScalars(FontWidthScalar.w1, FontHeighScalar.h8);
            printer.PrintASCIIString("H8x");
            printer.PrintNewline();

            printer.PrintASCIIString("W1x");
            printer.SetScalars(FontWidthScalar.w2, FontHeighScalar.h1);
            printer.PrintASCIIString("W2x");
            printer.SetScalars(FontWidthScalar.w3, FontHeighScalar.h1);
            printer.PrintASCIIString("W3x");
            printer.SetScalars(FontWidthScalar.w4, FontHeighScalar.h1);
            printer.PrintASCIIString("W4x");
            printer.SetScalars(FontWidthScalar.w5, FontHeighScalar.h1);
            printer.PrintASCIIString("W5x");
            printer.SetScalars(FontWidthScalar.w6, FontHeighScalar.h1);
            printer.PrintASCIIString("W6x");
            printer.SetScalars(FontWidthScalar.w7, FontHeighScalar.h1);
            printer.PrintASCIIString("W7x");
            printer.SetScalars(FontWidthScalar.w8, FontHeighScalar.h1);
            printer.PrintASCIIString("W8x");
            printer.PrintNewline();


            printer.PrintNewline();
            printer.FormFeed();
        }
    }
}
