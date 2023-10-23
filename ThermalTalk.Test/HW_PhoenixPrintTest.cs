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
    public class HW_PhoenixPrintTest
    {
        // Change this to connected printer's port.
        private const string TEST_PORT = "COM1";

        [Test()]
        public void PHX_RealHardwareTests()
        {
            var printer = new PhoenixPrinter(TEST_PORT);
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

            printer.FormFeed();
        }
    }
}
