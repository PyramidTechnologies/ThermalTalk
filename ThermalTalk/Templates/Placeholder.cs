﻿#region Copyright & License
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
    /// <summary>
    /// An implementation with zero side-effects
    /// </summary>
    public class Placeholder : ISection
    {
        public string Content { get { return string.Empty; } set { } }

        public FontEffects Effects { get { return FontEffects.None; } set { } }
   
        public FontJustification Justification { get { return FontJustification.NOP; } set { } }
   
        public FontWidthScalar WidthScalar { get { return FontWidthScalar.NOP; } set { } }

        public FontHeighScalar HeightScalar { get { return FontHeighScalar.NOP; } set { } }

        public ThermalFonts Font { get { return ThermalFonts.NOP; } set { } }

        public bool AutoNewline { get { return false; } set { } }

        /// <summary>
        /// REturns empty buffer
        /// </summary>
        /// <param name="codepage">Unused</param>
        /// <returns>zero length byte array</returns>
        public BufferAction GetContentBuffer(CodePages codepage)
        {
            return new BufferAction
            {
                Buffer = new byte[0]
            };
        }
    }
}
