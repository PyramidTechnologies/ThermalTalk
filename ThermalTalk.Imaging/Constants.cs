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
namespace ThermalTalk.Imaging
{
    public static class Constants
    {
        /// <summary>
        /// File filter for every support image type
        /// </summary>
        public static readonly string ImageFileFilter = "All Images (*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*" +
                            ".rle;*.gif;*.tif;*.tiff)|*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;" +
                            "*.bmp;*.dib;*.rle;*.gif;*.tif;*.tiff|Windows Enhanced Metafile (*.emf)|" +
                            "*.emf|Windows Metafile (*.wmf)|*.wmf|JPEG File Interchange Format (*.jpg;" +
                            "*.jpeg;*.jfif;*.jpe)|*.jpg;*.jpeg;*.jfif;*.jpe|Portable Networks Graphic (*.png)|" +
                            "*.png|Windows Bitmap (*.bmp;*.dib;*.rle)|*.bmp;*.dib;*.rle|Graphics Interchange Format (*.gif)|" +
                            "*.gif|Tagged Image File Format (*.tif;*.tiff)|*.tif;*.tiff|All files (*.*)|*.*";
    }
}
