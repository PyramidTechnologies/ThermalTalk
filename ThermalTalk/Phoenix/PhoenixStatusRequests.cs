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
    /// <summary>
    /// All available types of status requests. There is a lot of overlap and redundancy but this
    /// is how things are done in ESC/POS
    /// </summary>
    enum PhoenixStatusRequests
    {
        /// <summary>
        /// Transmit the printer status
        /// </summary>
        Status = 1,

        /// <summary>
        /// Transmit the off-line printer status
        /// </summary>
        OffLineStatus = 2,

        /// <summary>
        /// Transmit error status
        /// </summary>
        ErrorStatus = 3,

        /// <summary>
        /// Transmit paper roll sensor status
        /// </summary>
        PaperRollStatus = 4,

        /// <summary>
        /// Request all statuses
        /// </summary>
        FullStatus = 20,
    }
}
