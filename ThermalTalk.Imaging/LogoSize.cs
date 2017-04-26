namespace ThermalTalk.Imaging
{
    /// <summary>
    /// Encapsulates a bitmap's dimensions
    /// </summary>
    public class LogoSize
    {
        /// <summary>
        /// Width of image in bytes
        /// </summary>
        public int WidthBytes { get; set; }

        /// <summary>
        /// Width of image in thermal printer dots (def. 203 DPI)
        /// </summary>
        public int WidthDots { get; set; }

        /// <summary>
        /// Height of image in dots (and bytes, they're the same metric here)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes for this logo
        /// </summary>
        public int SizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the size of the logo stored in flash.
        /// This will always be greater than or equal to SizeInBytes
        /// </summary>
        public int SizeInBytesOnFlash { get; set; }

        /// <summary>
        /// Returns dimension string as WidthxHeight
        /// </summary>
        /// <returns></returns>
        public object GetBitmapSizeString()
        {
            return string.Format("{0}x{1}", WidthDots, Height);
        }
    }
}
