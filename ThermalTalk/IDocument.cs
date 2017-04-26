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
    public interface IDocument
    {
        /// <summary>
        /// Gets or Sets the string content for this document.
        /// All effects, scalars, and justification will be applied
        /// this this content. For newlines, inject the new line
        /// character "\n"
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// All effects to apply to content. This can be masked together to
        /// apply more than one effect. These effects will only
        /// be active during the printing of this document and then 
        /// they will be cleared.
        /// </summary>
        FontEffects Effects { get; set; }

        /// <summary>
        /// Gets or Sets justification for this document
        /// </summary>
        FontJustification Justification { get; set; }

        /// <summary>
        /// Gets or Sets the width scalar for this document
        /// </summary>
        FontWidthScalar WidthScalar { get; set; }

        /// <summary>
        /// Gets or Sets the height scalar for this document
        /// </summary>
        FontHeighScalar HeightScalar { get; set; }

        /// <summary>
        /// Auto-apply a newline after this document
        /// </summary>
        bool AutoNewline { get; set; }
    }
}
