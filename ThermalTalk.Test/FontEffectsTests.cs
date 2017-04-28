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
    public class FontEffectsTests
    {
        private const string TEST_PORT = "COM1";

        [Test()]
        public void RelianceEffectsTest()
        {
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);

            Assert.AreEqual(FontEffects.None, printer.Effects);

            // Set some effects
            printer.AddEffect(FontEffects.Bold);
            Assert.AreEqual(FontEffects.Bold, printer.Effects);

            printer.AddEffect(FontEffects.Italic);
            Assert.AreEqual(FontEffects.Bold | FontEffects.Italic, printer.Effects);

            printer.AddEffect(FontEffects.Underline);
            Assert.AreEqual(FontEffects.Bold | FontEffects.Italic | FontEffects.Underline, printer.Effects);

            printer.AddEffect(FontEffects.Rotated);
            Assert.AreEqual(FontEffects.Bold | FontEffects.Italic | FontEffects.Underline | FontEffects.Rotated, 
                printer.Effects);

            printer.AddEffect(FontEffects.Reversed);
            Assert.AreEqual(FontEffects.Bold | FontEffects.Italic | FontEffects.Underline | FontEffects.Rotated | FontEffects.Reversed,
                printer.Effects);

            // Clear them out
            printer.ClearAllEffects();
            Assert.AreEqual(FontEffects.None, printer.Effects);

            // Set some effects
            printer.AddEffect(FontEffects.Bold | FontEffects.Italic | FontEffects.Underline | FontEffects.Rotated | FontEffects.Reversed);
            Assert.AreEqual(FontEffects.Bold | FontEffects.Italic | FontEffects.Underline | FontEffects.Rotated | FontEffects.Reversed, printer.Effects);

            printer.RemoveEffect(FontEffects.Bold);
            Assert.AreEqual(FontEffects.Italic | FontEffects.Underline | FontEffects.Rotated | FontEffects.Reversed, printer.Effects);

            printer.RemoveEffect(FontEffects.Italic);
            Assert.AreEqual(FontEffects.Underline | FontEffects.Rotated | FontEffects.Reversed, printer.Effects);

            printer.RemoveEffect(FontEffects.Underline);
            Assert.AreEqual(FontEffects.Rotated | FontEffects.Reversed,
                printer.Effects);

            printer.RemoveEffect(FontEffects.Rotated);
            Assert.AreEqual(FontEffects.Reversed, printer.Effects);

            printer.RemoveEffect(FontEffects.Reversed);
            Assert.AreEqual(FontEffects.None, printer.Effects);
        }

        [Test()]
        public void RelianceJustificationTest()
        {
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);

            Assert.AreEqual(FontJustification.JustifyLeft, printer.Justification);

            // Set some justification
            printer.SetJustification(FontJustification.JustifyCenter);
            Assert.AreEqual(FontJustification.JustifyCenter, printer.Justification);

            printer.SetJustification(FontJustification.JustifyRight);
            Assert.AreEqual(FontJustification.JustifyRight, printer.Justification);

            printer.SetJustification(FontJustification.JustifyLeft);
            Assert.AreEqual(FontJustification.JustifyLeft, printer.Justification);
        }

        [Test()]
        public void RelianceScalarTest()
        {
            // Test ctor
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);

            // Ensure defaults are set
            Assert.AreEqual(FontWidthScalar.w1, printer.Width);
            Assert.AreEqual(FontHeighScalar.h1, printer.Height);

            // Set to something else and make sure the properties are messed up
            printer.SetScalars(FontWidthScalar.w8, FontHeighScalar.h2);

            Assert.AreEqual(FontWidthScalar.w8, printer.Width);
            Assert.AreEqual(FontHeighScalar.h2, printer.Height);
        }
    }
}
