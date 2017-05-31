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
using System.Text;
namespace ThermalTalk
{
    /// <summary>
    /// Standard document implementation
    /// </summary>
    public class StandardSection : ISection
    {
        public virtual string Content { get; set; }

        public virtual FontEffects Effects { get; set; }

        public virtual FontJustification Justification { get; set; }

        public virtual FontWidthScalar WidthScalar { get; set; }

        public virtual FontHeighScalar HeightScalar { get; set; }

        public virtual ThermalFonts Font { get; set; }

        public virtual bool AutoNewline { get; set; }

        public virtual byte[] GetContentBuffer(CodePages codepage)
        {
            Encoding encoder = null;
            switch(codepage)
            {
                case CodePages.CP771:
                    // This is the most similar to 771
                    encoder = System.Text.Encoding.GetEncoding(866);
                    break;

                case CodePages.CP437:
                    encoder = System.Text.Encoding.GetEncoding(437);
                    break;

                case CodePages.ASCII:
                    Content = System.Text.RegularExpressions.Regex.Replace(Content,
                        @"[^\u0020-\u007E]", string.Empty);
                    encoder = System.Text.ASCIIEncoding.ASCII;
                    break;

                default:
                    encoder = System.Text.Encoding.GetEncoding(771);
                    break;
            }

            return encoder.GetBytes(Content);
        }
    }
}
