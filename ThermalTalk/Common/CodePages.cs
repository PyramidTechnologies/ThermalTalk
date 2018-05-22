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
    using System;

    /// <summary>
    /// Codepages supported by devices
    /// Note: Reliance Only
    /// </summary>
    public enum CodePages
    {
        /// <summary>
        /// Prints only ASCII 32-127
        /// </summary>
        CPSPACE = 32,
        /// <summary>
        /// Kurdish Arabic script
        /// </summary>
        CP600 = 600,
        /// <summary>
        /// Cyrillic + Euro symbol
        /// </summary>
        CP808 = 808,
        /// <summary>
        /// DOS Latin 1
        /// </summary>
        CP850 = 850,
        /// <summary>
        /// Quebec French
        /// </summary>
        CP863 = 863,
        /// <summary>
        /// ANSI Latin 1
        /// </summary>
        CP1252 = 1252,
        /// <summary>
        /// Georgian script
        /// </summary>
        CP4256 = 4256,

        /// <summary>
        /// Russian Cyrillic
        /// </summary>
        [Obsolete("Use 808")]
        CP771 = 0,

        /// <summary>
        /// Eurpoean and Greek
        /// </summary>
        CP437 = 437,

        /// <summary>
        /// Skips any non-ascii chars
        /// </summary>
        [Obsolete("Use CPSPACE")]
        ASCII
    }
}
