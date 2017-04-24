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
namespace RelianceTalk
{
    /// <summary>
    /// Contract for all serialized connection types
    /// </summary>
    public interface ISerialConnection
    {
        /// <summary>
        /// Attempts to open serial device for communication. Returns
        /// Success only if device is found and opened successfully.
        /// </summary>
        /// <returns></returns>
        ReturnCode Open();

        /// <summary>
        /// Attempts to close and release the connection to the serial device.
        /// If there are no issues, Success will be returned.
        /// </summary>
        /// <returns></returns>
        ReturnCode Close();

        /// <summary>
        /// Gets or Sets the name that uniquely identifies this serial 
        /// device to the system
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Transmits payload and returns response. The implementation
        /// of this class is responsible for any payload packaging,
        /// checksums, etc.
        /// </summary>
        /// <param name="payload">Data to send</param>
        /// <returns>0 or more bytes response data</returns>
        byte[] Write(byte[] payload);
    }
}
