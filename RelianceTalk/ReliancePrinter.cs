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
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Reliance Printer is the primary handle for accessing the printer API
    /// </summary>
    public class ReliancePrinter
    {

        const int DefaultReadTimeout = 2000; /// ms
        const int DefaultBaudRate = 19200;


        private readonly SerialPort mSerialPort;

        /// <summary>
        /// Constructs a new instance of ReliancePrinter. This printer
        /// acts as a handle to all features and functions
        /// </summary>
        /// <param name="printerName">OS name of printer</param>
        public ReliancePrinter(string printerName) 
            : this(printerName, string.Empty)
        { }

        /// <summary>
        /// Constructs a new instance of ReliancePrinter. This printer
        /// acts as a handle to all features and functions. If the serial port parameter
        /// is provided, the serial connection will be opened immediately.
        /// </summary>
        /// <param name="printerName">OS name of printer</param>
        /// <param name="serialPortName">OS name of serial port</param>
        public ReliancePrinter(string printerName, string serialPortName)
        {
            PrinterName = printerName;

            // User wants a serial port
            if (!string.IsNullOrEmpty(serialPortName))
            {            
                PrintSerialReadTimeout = DefaultReadTimeout;
                PrintSerialBaudRate = DefaultBaudRate;
                PrintSerialPortName = serialPortName;

                mSerialPort = MakePort(PrintSerialPortName, PrintSerialBaudRate, PrintSerialReadTimeout);
                mSerialPort.Open();
            }
        }

        /// <summary>
        /// Destructor - Close and dispose serial port if needed
        /// </summary>
        ~ReliancePrinter()
        {
            if (mSerialPort != null)
            {
                mSerialPort.Close();
                mSerialPort.Dispose();
            }
        }

        /// <summary>
        /// Gets or replaces the name this printer was constructed with
        /// </summary>
        public string PrinterName { get; set; }

        /// <summary>
        /// Gets or sets the serial port associated with this printer.
        /// </summary>
        public string PrintSerialPortName { get; set; }

        /// <summary>
        /// Gets or sets the read timeout in milliseconds
        /// </summary>
        public int PrintSerialReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port baud rate
        /// </summary>
        public int PrintSerialBaudRate { get; set; }

        /// <summary>
        /// This command is processed in real time. The reply to this command is sent
        /// whenever it is received and does not wait for previous ESC/POS commands to be executed first.
        /// If there is no response or an invalid response, all fields of RealTimeStatus will be null.
        /// </summary>
        /// <param name="r">StatusRequest type</param>
        /// <returns>Instance of RealTimeStatus. Unset fields will be null</returns>
        public RealTimeStatus GetStatus(StatusRequests r)
        {
            // Result stored here
            var rts = new RealTimeStatus();

            // Send the real time status command, r is the argument
            var command = new byte[] { 0x10, 0x04, (byte)r };
            int respLen = (r == StatusRequests.FullStatus) ? 6 : 1;

            var data = new byte[0];
            if (mSerialPort == null)
            {
                WritePrinter(command);

                data = ReadPrinter(respLen);
            }
            else
            {
                var written = WritePort(command);

                // Collect the response
                data = ReadPort(respLen);
            }


            // Invalid response
            if(data.Length != respLen)
            {
                return rts;
            }

            switch(r)
            {
                case StatusRequests.Status:
                    // bit 3: 0- online, 1- offline        
                    rts.IsOnline = (data[0] & 0x08) == 0;
                    break;

                case StatusRequests.OffLineStatus:
                    // bit 2: 0- no error, 1- error        
                    rts.IsCoverClosed = (data[0] & 0x04) == 0;

                    // bit 3: 0- no error, 1- error        
                    rts.IsNormalFeed = (data[0] & 0x08) == 0;

                    // bit 5: 0- no error, 1- error        
                    rts.IsPaperPresent = (data[0] & 0x20) == 0;

                    // bit 6: 0- no error, 1- error        
                    rts.HasError = (data[0] & 0x40) == 0;
                 
                    break;

                case StatusRequests.ErrorStatus:
                    // bit 3: 0- okay, 1- Not okay    
                    rts.IsCutterOkay = (data[0] & 8) == 0;    

                    // bit 5: 0- No fatal (non-recoverable) error, 1- Fatal error        
                    rts.HasFatalError = (data[0] & 8) == 0;    
    
                    // bit 6: 0- No recoverable error, 1- Recoverable error        
                    rts.HasRecoverableError = (data[0] & 0x40) == 1; 
                    break;

                case StatusRequests.PaperRollStatus:
                    /// bit 2,3: 0- okay, 12- Not okay        
                    rts.IsPaperLevelOkay = (data[0] & 0x0C) == 0;

                    /// bit 5,6: 0- okay, 96- Not okay
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;                    
                    break;

                case StatusRequests.PrintStatus:
                    /// bit 2: 0- motor off, 1: motor on        
                    rts.IsPaperMotorOff = (data[0] & 4) == 0;
                    // bit 5: 0- paper present, 1: motor stopped because out of paper
                    
                    rts.IsPaperPresent = (data[0] & 4) == 0;
                    break;

                case StatusRequests.FullStatus:

                    rts.IsPaperPresent = (data[2] & 0x01) == 0;
                    rts.IsPaperLevelOkay = (data[2] & 0x04) == 0;
                    rts.IsTicketPresentAtOutput = (data[2] & 0x20) == 0;

                    // Custom specs duplicates these so if EITHER
                    // are set to open, report open
                    var covera = (data[3] & 0x01) == 0;
                    var coverb = (data[3] & 0x02) == 0;

                    rts.IsCoverClosed = covera && coverb;

                    rts.IsPaperMotorOff = (data[3] & 0x08) == 0;
                    rts.IsDiagButtonReleased = (data[3] & 0x20) == 0;

                    rts.IsHeadTemperatureOkay = (data[4] & 0x01) == 0;
                    rts.IsCommsOkay = (data[4] & 02) == 0;
                    rts.IsPowerSupplyVoltageOkay = (data[4] & 0x08) == 0;
                    rts.IsPaperPathClear = (data[4] & 0x40) == 0;

                    rts.IsCutterOkay = (data[5] & 0x01) == 0; 
                    break;
                    
            }

            return rts;
        }

        /// <summary>
        /// Common print hanlder for printing raw data bytes
        /// </summary>
        /// <param name="data"></param>
        private void WritePrinter(byte[] data)
        {
            // Gotta get a pointer on the local heap. Fun fact, the naming suggests that
            // this would be on the stack but it isn't. Windows no longer has a global heap
            // per se so these naming conventions are legacy cruft.
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);


            RawPrinterHelper.SendBytesToPrinter(PrinterName, ptr, data.Length);

            Marshal.FreeHGlobal(ptr);
        }

        private byte[] ReadPrinter(int count)
        {
            Int32 dwCount = count;
            IntPtr pBytes = new IntPtr(dwCount);

            byte[] returnbytes = new byte[dwCount];
            pBytes = Marshal.AllocCoTaskMem(dwCount);
            bool success = RawPrinterHelper.ReadFromPrinter(PrinterName, pBytes, dwCount);
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

        /// <summary>
        /// Writes data to the printer serial port
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>Number of bytes written</returns>
        private int WritePort(byte[] data)
        {
            try
            {
                // Dump anything not already processed
                mSerialPort.DiscardInBuffer();
                mSerialPort.DiscardOutBuffer();

                mSerialPort.Write(data, 0, data.Length);
                return data.Length;
            }
            catch
            { }            


            return 0;
        }

        /// <summary>
        /// Reads count bytes from serial port. If count bytes are unavailable, this
        /// function will block until the read times out. If there is an exception or
        /// not all expected data is received, an empty buffer will be returned.
        /// </summary>
        /// <param name="count">Number of bytes to read from port.</param>
        /// <returns>Bytes read from port.</returns>
        private byte[] ReadPort(int count)
        {
            
            var buff = new byte[count];

            try
            {
                // Give a slight delay to allow the buffer time to more fully fill up
                System.Threading.Thread.Sleep(100);

                // Attempt to read count bytes
                var read = mSerialPort.Read(buff, 0, count);

                // If we don't get enough, reset buffer to 0 length
                if (read != count)
                {
                    buff = new byte[0];
                }
            }
            catch
            {
                buff = new byte[0];
            }

            return buff;
        }

        /// <summary>
        /// Create a new serial port using 8 databits, no parity, 1 stop bit.
        /// </summary>
        /// <param name="portName">OS name of port</param>
        /// <param name="baudrate">Valid baud rate</param>
        /// <param name="readTimeout">Time in milliseconds to await buffer to contain data</param>
        /// <returns></returns>
        private static SerialPort MakePort(string portName, int baudrate, int readTimeout)
        {

            System.Text.Encoding W1252 = System.Text.Encoding.GetEncoding("Windows-1252");

            var port = new SerialPort();
            port.BaudRate = baudrate;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.ReadTimeout = readTimeout;
            port.WriteTimeout = 500;
            port.Encoding = W1252;
            port.DtrEnable = true;
            port.RtsEnable = true;
            port.DiscardNull = false;
            port.PortName = portName;

            return port;
        }
    }
}
