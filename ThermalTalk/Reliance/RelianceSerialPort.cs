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
    using System;
    using System.IO.Ports;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Reliance serial port implementation 
    /// </summary>
    class RelianceSerialPort : BaseSerialPort
    {
        #region Default SerialPort Params
        private const int DefaultBaudRate = 19200;
        private const int DefaultDatabits = 8;
        private const Parity DefaultParity = Parity.None;
        private const StopBits DefaultStopbits = StopBits.One;
        private const Handshake DefaultHandshake = Handshake.None;
        #endregion

        #region Constructor
        public RelianceSerialPort(string portName)
            : this(portName, DefaultBaudRate)
        { }

        public RelianceSerialPort(string portName, int baud)
            : base(portName, baud, DefaultDatabits, DefaultParity, DefaultStopbits, DefaultHandshake)
        { }

        #endregion

        /// <summary>
        /// Write raw data to printer through its Windows print handle
        /// </summary>
        /// <param name="data">buffer to send</param>
        /// <returns>Return code</returns>
        public static ReturnCode WritePrinter(string printerName, byte[] data)
        {
            // Gotta get a pointer on the local heap. Fun fact, the naming suggests that
            // this would be on the stack but it isn't. Windows no longer has a global heap
            // per se so these naming conventions are legacy cruft.
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);


            bool result = RawPrinterHelper.SendBytesToPrinter(printerName, ptr, data.Length);

            Marshal.FreeHGlobal(ptr);

            return result ? ReturnCode.Success : ReturnCode.ExecutionFailure;
        }
     
        /// <summary>
        /// Reads count bytes from printer
        /// </summary>
        /// <param name="printerName">Win32 printer name</param>
        /// <param name="count">Count of bytes to read</param>
        /// <returns>Response data</returns>
        public static byte[] ReadPrinter(string printerName, int count)
        {
            Int32 dwCount = count;
            IntPtr pBytes = new IntPtr(dwCount);

            byte[] returnbytes = new byte[dwCount];
            pBytes = Marshal.AllocCoTaskMem(dwCount);
            bool success = RawPrinterHelper.ReadFromPrinter(printerName, pBytes, dwCount);
            if (success)
            {
                Marshal.Copy(returnbytes, 0, pBytes, dwCount);
            }
            else
            {
                returnbytes = new byte[0];
            }

            return returnbytes;
        }   
    }
}
