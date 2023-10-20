using System;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using ThermalTalk;

namespace ThermalUwp.Serial
{
    public abstract class BaseSerialPort : ISerialConnection
    {
        #region Constructor

        protected BaseSerialPort(
            SerialDevice device,
            uint baud,
            ushort dataBits,
            SerialParity parity,
            SerialStopBitCount stopBits,
            SerialHandshake handshake)
        {
            Device = device;
            DeviceOutput = new DataWriter(Device.OutputStream);
            DeviceInput = new DataReader(Device.InputStream);
            Name = Device.PortName;
            WriteTimeout = 5000;
            ReadTimeout = 5000;
            ChunkSize = 64;
            IsDisposed = false;

            Device.BaudRate = baud;
            Device.DataBits = dataBits;
            Device.Parity = parity;
            Device.StopBits = stopBits;
            Device.Handshake = handshake;
            Device.WriteTimeout = TimeSpan.FromMilliseconds(WriteTimeout);
            Device.ReadTimeout = TimeSpan.FromMilliseconds(ReadTimeout);
            Device.IsDataTerminalReadyEnabled = true;
            Device.IsRequestToSendEnabled = true;
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
            get => ReadTimeout;
            set
            {
                ReadTimeout = value;
                Device.ReadTimeout = TimeSpan.FromMilliseconds(value);
            }
        }

        /// <summary>
        /// Gets or Sets the write timeout in milliseconds
        /// </summary>
        public int WriteTimeoutMS
        {
            get => WriteTimeout;
            set
            {
                WriteTimeout = value;
                Device.WriteTimeout = TimeSpan.FromMilliseconds(value);
            }
        }

        protected SerialDevice Device { get; set; }

        protected DataWriter DeviceOutput { get; set; }

        protected DataReader DeviceInput { get; set; }

        protected int ReadTimeout { get; set; }

        protected int WriteTimeout { get; set; }

        /// <summary>
        /// Set to size that works for your printer implementation.
        /// This is to prevent sending too much data at once and overrunning the target buffer.
        /// </summary>
        protected int ChunkSize { get; set; }

        protected bool IsDisposed { get; set; }

        #endregion

        public virtual ReturnCode Open()
        {
            return !IsDisposed ? ReturnCode.Success : ReturnCode.ConnectionNotFound;
        }

        public virtual ReturnCode Close()
        {
            return ReturnCode.Success;
        }

        public virtual int Write(byte[] payload)
        {
            try
            {
                // Split into chunks to avoid overrunning target buffer.
                foreach (var chunk in payload.Split(ChunkSize))
                {
                    DeviceOutput.WriteBytes(chunk);
                    DeviceOutput.StoreAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                return payload.Length;
            }
            catch
            {
                return 0;
            }
        }

        public virtual byte[] Read(int n)
        {
            byte[] buffer;

            try
            {
                // Attempt to read n bytes.
                var receivedBytes = DeviceInput.LoadAsync((uint)n).AsTask().ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                // If we don't get enough, reset buffer to 0 length.
                if (receivedBytes != n)
                    buffer = Array.Empty<byte>();
                else
                {
                    buffer = new byte[n];
                    DeviceInput.ReadBytes(buffer);
                }
            }
            catch
            {
                buffer = Array.Empty<byte>();
            }

            return buffer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            DeviceOutput.DetachStream();
            DeviceOutput.Dispose();
            DeviceInput.DetachStream();
            DeviceInput.Dispose();
            IsDisposed = true;
        }
    }
}