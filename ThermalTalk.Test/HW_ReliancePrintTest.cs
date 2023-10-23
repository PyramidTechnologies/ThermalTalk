/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
using NUnit.Framework;

namespace ThermalTalk.Test
{
    [TestFixture()]
    public class HW_ReliancePrintTest
    {
        // Change this to connected printer's port.
        private const string TEST_PORT = "COM4";

        [Test()]
        public void REL_RealHardwareTests()
        {
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);

            printer.Reinitialize();

            printer.PrintASCIIString("No effects - Left");
            printer.PrintNewline();

            printer.AddEffect(FontEffects.Bold);
            printer.PrintASCIIString("This is bold");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Bold);
            printer.AddEffect(FontEffects.Italic);
            printer.PrintASCIIString("This is italic");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Italic);
            printer.AddEffect(FontEffects.Underline);
            printer.PrintASCIIString("This is underline");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Underline);
            printer.AddEffect(FontEffects.Rotated);
            printer.PrintASCIIString("This is rotated");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Rotated);
            printer.AddEffect(FontEffects.Reversed);
            printer.PrintASCIIString("This is reversed");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Reversed);
            printer.AddEffect(FontEffects.UpsideDown);
            printer.PrintASCIIString("This is upsideDown");
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

            printer.RemoveEffect(FontEffects.Italic);
            printer.AddEffect(FontEffects.Underline);
            printer.PrintASCIIString("This is underline");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Underline);
            printer.AddEffect(FontEffects.Rotated);
            printer.PrintASCIIString("This is rotated");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Rotated);
            printer.AddEffect(FontEffects.Reversed);
            printer.PrintASCIIString("This is reversed");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Reversed);
            printer.AddEffect(FontEffects.UpsideDown);
            printer.PrintASCIIString("This is upsideDown");
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

            printer.RemoveEffect(FontEffects.Italic);
            printer.AddEffect(FontEffects.Underline);
            printer.PrintASCIIString("This is underline");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Underline);
            printer.AddEffect(FontEffects.Rotated);
            printer.PrintASCIIString("This is rotated");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Rotated);
            printer.AddEffect(FontEffects.Reversed);
            printer.PrintASCIIString("This is reversed");
            printer.PrintNewline();

            printer.RemoveEffect(FontEffects.Reversed);
            printer.AddEffect(FontEffects.UpsideDown);
            printer.PrintASCIIString("This is upsideDown");
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

        [Test()]
        public void REL_RealHardwareTest2DBarcodes()
        {
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);

            printer.Print2DBarcode("0123456789AMANO");
            printer.FormFeed();
        }
    }
}
