namespace ThermalTalk.Imaging
{
    using Emgu.CV;

    public static class Webcam
    {
        public static PrinterImage GrabPicture()
        {
            Capture capture = new Capture(0);
            using(var image = capture.QueryFrame().Bitmap)
            {
                return new PrinterImage(image);
            }
        }
    }
}
