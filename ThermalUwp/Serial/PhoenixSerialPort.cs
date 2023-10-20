using Windows.Devices.SerialCommunication;

namespace ThermalUwp.Serial
{
    public class PhoenixSerialPort : BaseSerialPort
    {
        private const uint DefaultBaudRate = 9600;
        private const ushort DefaultDataBits = 8;
        private const SerialParity DefaultParity = SerialParity.None;
        private const SerialStopBitCount DefaultStopBits = SerialStopBitCount.One;
        private const SerialHandshake DefaultHandshake = SerialHandshake.None;

        public PhoenixSerialPort(SerialDevice device, uint baud = DefaultBaudRate)
            : base(device, baud, DefaultDataBits, DefaultParity, DefaultStopBits, DefaultHandshake)
        {
        }
    }
}