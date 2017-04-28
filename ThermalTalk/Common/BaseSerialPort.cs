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
    using System.IO.Ports;
    using System.Threading;

    abstract class BaseSerialPort : ISerialConnection
    {
        #region Fields
        protected readonly SerialPort _mPort;
        protected int _mReadTimeout;
        protected int _mWriteTimeout;
        /// <summary>
        /// Set to size that works for your printer implementation.
        /// This is to prevent sending too much data at once and
        /// overruning the target buffer.
        /// TODO expose this as a configurable on the concrete implementation
        /// </summary>
        protected int _mChunkSize;
        #endregion

        #region Constructor
        protected BaseSerialPort(string portName,
            int baud,
            int databits,
            Parity parity,
            StopBits stopbits,
            Handshake handshake)
        {
            Name = portName;

            _mPort = new SerialPort(portName, baud, parity, databits, stopbits);            
            _mPort.Handshake = handshake;
            _mPort.WriteTimeout = _mWriteTimeout = 500;
            _mPort.ReadTimeout = _mReadTimeout = 500;
            _mPort.Handshake = Handshake.None;
            _mPort.WriteBufferSize = 4 * 1024;
            _mPort.ReadBufferSize = 4 * 1024;
            _mPort.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            _mPort.DtrEnable = true;
            _mPort.RtsEnable = true;
            _mPort.DiscardNull = false;


            // Virtual comm ports use a bulk transfer endpoint which is
            // typically 64 bytes in size. High speed can use 512 bytes but
            // I've never found a VCOM that implements this.
            if (SerialPortUtils.IsVirtualComPort(portName))
            {
                _mChunkSize = 64;
            }
            else
            {
                // Good 'ol hardware RS-232 can handle a bit more but still be careful
                // the target device might have a tiny circular buffer so keep this modest.
                _mChunkSize = 256;
            }
        }

        ~BaseSerialPort()
        {
            Dispose(false);
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
            catch (System.IO.IOException)
            {
                return ReturnCode.ConnectionNotFound;
            }
            catch (System.AccessViolationException)
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mPort != null)
                {
                    Close();
                    _mPort.Dispose();
                }
            }
        }

        #region Protected
        /// <summary>
        /// Writes data to the printer serial port
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>Number of bytes written</returns>
        protected int WritePort(byte[] data)
        {
            try
            {
                // Split into chunks to avoid overrunning target buffer
                foreach (var s in data.Split(_mChunkSize))
                {
                    _mPort.Write(s, 0, s.Length);
                    Thread.Sleep(10);
                }

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
        protected byte[] ReadPort(int count)
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
