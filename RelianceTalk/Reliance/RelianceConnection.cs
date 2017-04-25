namespace ThermalTalk
{
    using System;
    using System.IO.Ports;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Reliance serial port implementation 
    /// </summary>
    class RelianceConnection : ISerialConnection
    {
        #region Default SerialPort Params
        const int DefaultBaudRate = 19200;
        const int DefaultDatabits = 8;
        const Parity DefaultParity = Parity.None;
        const StopBits DefaultStopbits = StopBits.One;
        const Handshake DefaultHandshake = Handshake.None;
        #endregion

        #region Fields
        private readonly SerialPort _mPort;
        private int _mReadTimeout;
        private int _mWriteTimeout;
        #endregion

        #region Constructor
        public RelianceConnection(string portName)
            : this(portName, DefaultBaudRate)
        { }

        public RelianceConnection(string portName, int baud)
            : this(portName, baud, DefaultDatabits, DefaultParity, DefaultStopbits, DefaultHandshake)
        { }

        public RelianceConnection(string portName, 
            int baud, 
            int databits, 
            Parity parity, 
            StopBits stopbits, 
            Handshake handshake)
        {
            Name = portName;

            _mPort = new SerialPort(portName, baud, parity, databits, stopbits);
            _mPort.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            _mPort.Handshake = handshake;
            _mPort.WriteTimeout = _mWriteTimeout = 500;
            _mPort.ReadTimeout = _mReadTimeout = 500;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the serial port name for this connection
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or Sets the read timeout in milliseconds
        /// </summary>
        public int ReadTimeoutMS
        {
            get { return _mReadTimeout; }
            set
            {
                _mReadTimeout = value;
                _mPort.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or Sets the write timeout in milliseconds
        /// </summary>
        public int WriteTimeoutMS
        {
            get { return _mWriteTimeout; }
            set
            {
                _mWriteTimeout = value;
                _mPort.WriteTimeout = value;
            }
        }

        #endregion

        public ReturnCode Open()
        {
            if (_mPort.IsOpen) return ReturnCode.Success;

            try
            {
                _mPort.Open();
                return ReturnCode.Success;
            } 
            catch(System.IO.IOException)
            {
                return ReturnCode.ConnectionNotFound;
            }
            catch(System.AccessViolationException)
            {
                return ReturnCode.ConnectionAlreadyOpen;
            }
        }

        public ReturnCode Close()
        {
            if (!_mPort.IsOpen) return ReturnCode.Success;

            try
            {
                _mPort.Close();
                return ReturnCode.Success;
            }
            catch
            {
                return ReturnCode.ExecutionFailure;
            }
        }


        public int Write(byte[] payload)
        {
            return WritePort(payload);
        }

        public byte[] Read(int n)
        {
            return ReadPort(n);
        }

        public void Dispose()
        {
            if(_mPort != null)
            {
                Close();
                _mPort.Dispose();
            }
        }


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

        #region Private
        /// <summary>
        /// Writes data to the printer serial port
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>Number of bytes written</returns>
        private int WritePort(byte[] data)
        {
            try
            {
                _mPort.Write(data, 0, data.Length);

                return data.Length;
            }
            catch
            { 
                return 0; 
            }
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

            byte[] buff = new byte[count];

            try
            {

                // Attempt to read count bytes
                var read = _mPort.Read(buff, 0, count);

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
        #endregion
    }
}
