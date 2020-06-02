#region Copyright & License
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
#endregion
namespace ThermalTalk
{
    using ThermalTalk.Imaging;

    interface IPrinter : System.IDisposable
    {
        /// <summary>
        /// Gets the active font effects      
        /// </summary>
        FontEffects Effects { get; }

        /// <summary>
        /// Gets or sets the active justification
        /// </summary>
        FontJustification Justification { get; }

        /// <summary>
        /// Gets or Sets the font's height scalar        
        /// </summary>
        FontHeighScalar Height { get; }

        /// <summary>
        /// Gets or Sets the font's width scalar
        /// </summary>
        FontWidthScalar Width { get; }

        /// <summary>
        /// Gets the active font
        /// </summary>
        ThermalFonts Font { get; }
        
        /// <summary>
        /// Gets or set the logger for IPrinter
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Returns the sepcified status report for this printer
        /// </summary>
        /// <param name="type">Status query type</param>
        /// <returns>Status report</returns>
        StatusReport GetStatus(StatusTypes type);

        /// <summary>
        /// Sets the active font to this
        /// </summary>
        /// <param name="font">Font to use</param>
        ReturnCode SetFont(ThermalFonts font);

        /// <summary>
        /// Applies the specified scalars
        /// </summary>
        /// <param name="w">Width scalar</param>
        /// <param name="h">Height scalar</param>
        ReturnCode SetScalars(FontWidthScalar w, FontHeighScalar h);

        /// <summary>
        /// Applies the specified justification
        /// </summary>
        /// <param name="justification">Justification to use</param>
        ReturnCode SetJustification(FontJustification justification);

        /// <summary>
        /// Activates effect for next print. This effect
        /// may be bitwise OR'd to apply multiple effects at
        /// one time. If there are any conflicting effects, the
        /// printer has final say on the defined behavior. 
        /// </summary>
        /// <param name="effect">Font effect to apply</param>
        ReturnCode AddEffect(FontEffects effect);

        /// <summary>
        /// Remove effect from the active effect list. If effect
        /// is not currently in the list of active effects, nothing
        /// will happen.
        /// </summary>
        /// <param name="effect">Effect to remove</param>
        ReturnCode RemoveEffect(FontEffects effect);

        /// <summary>
        /// Remove all effects immediately. Only applies
        /// to data that has not yet been transmitted.
        /// </summary>
        ReturnCode ClearAllEffects();

        /// <summary>
        /// Sets all ESC/POS options to default
        /// </summary>
        ReturnCode Reinitialize();

        /// <summary>
        /// Print string as ASCII text. Any effects that are currently
        /// active will be applied to this string.
        /// </summary>
        /// <param name="str">ASCII stirng to print</param>
        ReturnCode PrintASCIIString(string str);

        /// <summary>
        /// Prints the specified document
        /// </summary>
        /// <param name="doc">Document to print</param>
        ReturnCode PrintDocument(IDocument doc);

        /// <summary>
        /// Sets this logo to a position inside doc specified by index.        
        /// </summary>
        /// <example>
        /// 
        /// var header = new StandardSection()
        /// {
        ///     Justification = FontJustification.JustifyCenter,
        ///     HeightScalar = FontHeighScalar.h2,
        ///     WidthScalar = FontWidthScalar.w2,
        ///     AutoNewline = true,
        /// };
        /// 
        /// 
        /// var document = new StandardDocument();
        /// document.Sections.Add(header);
        /// 
        /// // Adds this image to after the header
        /// var someImage = Webcam.GrabPicture()
        /// myPrinter.SetImage(someImage, document, 1); 
        /// </example>
        /// <param name="image">Image to add</param>
        /// <param name="doc">Document to add</param>
        /// <param name="index">Index to insert. If this index exceeds the current length
        /// placeholders will be inserted until index is reached.</param>
        ReturnCode SetImage(PrinterImage image, IDocument doc, int index);

        /// <summary>
        /// Emit one newline character and return print
        /// position to start of line.
        /// </summary>
        ReturnCode PrintNewline();

        /// <summary>
        /// Mark ticket as complete and present
        /// </summary>
        ReturnCode FormFeed();

        /// <summary>
        /// Send raw buffer to target printer.
        /// </summary>
        /// <param name="raw"></param>
        ReturnCode SendRaw(byte[] raw);
    }
}
