
using System;
using System.IO;
using System.Threading;
using ThermalTalk;
using ThermalTalk.Imaging;

namespace ThermalConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            const string commport = "COM1";            
            const string imgpath = @"C:\temp\security_sample.png";
            const string imgescbin = @"C:\temp\security_sample.bin";

            const int captureRate = 10; // number of seconds between capture

            Console.WriteLine("Starting Security Cam Sample");

            int count = 1;
            while (true)
            {
                using (var printer = new ReliancePrinter(commport))
                using(var image = ThermalTalk.Imaging.Webcam.GrabPicture())
                {
                    image.Save(imgpath);
                    Console.WriteLine("Image #{0} taken at {1}", count, DateTime.Now);

                    using(var logo = new BasePrintLogo(imgpath, 640, 480))
                    {
                        logo.ApplyDithering(Algorithms.JarvisJudiceNinke, 128);                       
                        logo.ExportLogoEscPos(imgescbin);

                        logo.ExportLogo(@"C:\temp\dither.png");

                        var raw = File.ReadAllBytes(imgescbin);

                        printer.PrintASCIIString(string.Format("Capture #{0}", count++));
                        printer.PrintNewline();

                        Thread.Sleep(5);
                        
                        printer.SendRaw(raw);                        
                        printer.FormFeed();


                        Thread.Sleep(captureRate * 1000);
                    }
                }

      
            }
        }
    }
}
