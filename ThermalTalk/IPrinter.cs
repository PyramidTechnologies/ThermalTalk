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
namespace ThermalTalk
{
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
        /// Applies the specified scalars
        /// </summary>
        /// <param name="w">Width scalar</param>
        /// <param name="h">Height scalar</param>
        void SetScalars(FontWidthScalar w, FontHeighScalar h);

        /// <summary>
        /// Applies the specified justification
        /// </summary>
        /// <param name="justification">Justification to use</param>
        void SetJustification(FontJustification justification);

        /// <summary>
        /// Activates effect for next print. This effect
        /// may be bitwise OR'd to apply multiple effects at
        /// one time. If there are any conflicting effects, the
        /// printer has final say on the defined behavior. 
        /// </summary>
        /// <param name="effect">Font effect to apply</param>
        void AddEffect(FontEffects effect);

        /// <summary>
        /// Remove effect from the active effect list. If effect
        /// is not currently in the list of active effects, nothing
        /// will happen.
        /// </summary>
        /// <param name="effect">Effect to remove</param>
        void RemoveEffect(FontEffects effect);

        /// <summary>
        /// Remove all effects immediately. Only applies
        /// to data that has not yet been transmitted.
        /// </summary>
        void ClearAllEffects();

        /// <summary>
        /// Sets all ESC/POS options to default
        /// </summary>
        void Reinitialize();

        /// <summary>
        /// Print string as ASCII text. Any effects that are currently
        /// active will be applied to this string.
        /// </summary>
        /// <param name="str">ASCII stirng to print</param>
        void PrintASCIIString(string str);

        /// <summary>
        /// Prints the specified document
        /// </summary>
        /// <param name="doc">Document to print</param>
        void PrintDocument(IDocument doc);

        /// <summary>
        /// Emit one newline character and return print
        /// position to start of line.
        /// </summary>
        void PrintNewline();

        /// <summary>
        /// Mark ticket as complete and present
        /// </summary>
        void FormFeed();

        /// <summary>
        /// Send raw buffer to target printer.
        /// </summary>
        /// <param name="raw"></param>
        void SendRaw(byte[] raw);
    }
}
