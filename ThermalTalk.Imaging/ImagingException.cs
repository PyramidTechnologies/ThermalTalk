namespace ThermalTalk.Imaging
{
    using System;

    public class ImagingException : Exception
    {
        public ImagingException(string message)
            : base(message)
        { }
    }
}
