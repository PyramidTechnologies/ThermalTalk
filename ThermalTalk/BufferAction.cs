using System;

namespace ThermalTalk
{
    public class BufferAction
    {
        public byte[] Buffer { get; set; }

        public TimeSpan AfterSendDelay { get; set; } = TimeSpan.Zero;
    }
}