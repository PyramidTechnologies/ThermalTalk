namespace ThermalTalk.Imaging
{
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using System.Drawing;

    public static class Webcam
    {
        public static Bitmap GrabPicture()
        {
            Capture capture = new Capture(0);
            Bitmap image = capture.QueryFrame().Bitmap; //take a picture

            return image;
        }
    }
}
