namespace ThermalTalk.Imaging
{
    using System;

    public interface IPrintLogo : IDisposable
    {
        /// <summary>
        /// Inverts the color palette on this bitmap, pixel-by-pixel
        /// </summary>
        void ApplyColorInversion();

        /// <summary>
        /// Applies the specified dithering algorithm to this logo
        /// </summary>
        /// <param name="algorithm">Name of algorithm</param>
        /// <param name="threshhold">Gray threshold. Values below this are considered white</param>
        void ApplyDithering(Algorithms algorithm, byte threshhold);

        /// <summary>
        /// Returns this bitmap, in its current state, as a base64 encoded string
        /// </summary>
        /// <returns></returns>
        string AsBase64String();

        /// <summary>
        /// Observable bitmap image source that represents bitmap in its current state
        /// and will reflect any changes due to resize, dither, or inversion.
        /// </summary>
        System.Windows.Media.Imaging.BitmapImage ImageData { get; }

        /// <summary>
        /// Width of this bitmap in pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of this bitmap in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Total size of this bitmap in bytes
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Returns true if this bitmap is in an inverted state
        /// </summary>
        bool IsInverted { get; }
    }
}
