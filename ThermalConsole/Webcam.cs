namespace ThermalConsole
{
    using Emgu.CV;
    using ThermalTalk.Imaging;

    public static class Webcam
    {
        /// <summary>
        /// Using the first available webcam, take a picture and return
        /// the resulting image.
        /// </summary>
        /// <returns>PrinterImage</returns>
        public static PrinterImage GrabPicture()
        {
            Capture capture = new Capture(0);
            using (var image = capture.QueryFrame().Bitmap)
            {
                return new PrinterImage(image);
            }
        }
    }
}
