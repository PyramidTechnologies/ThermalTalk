
using System;
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
            const int captureRate = 10; // number of seconds between capture
            
            Console.WriteLine("Starting Security Camera Sample");

            int count = 1;
            while (true)
            {
                using (var printer = new ReliancePrinter(commport))
                using(var image = ThermalTalk.Imaging.Webcam.GrabPicture())
                {
             
                    var now = DateTime.Now;
                    Console.WriteLine("Image #{0} taken at {1}", count, now);

                    // Resize image if we want. This will follow ESC/POS justification
                    //logo.Resize(640, 480);
                    image.ApplyDithering(Algorithms.JarvisJudiceNinke, 128);

                    // Center justify the capture count number
                    printer.SetJustification(FontJustification.JustifyCenter);
                    printer.PrintASCIIString(string.Format("Capture #{0}", count));
                    printer.PrintNewline();

                    // Bold (and still centered) timestamp
                    printer.AddEffect(FontEffects.Bold);
                    printer.PrintASCIIString(string.Format("{1}", count++, now));
                    printer.RemoveEffect(FontEffects.Bold);
                    printer.PrintNewline();

                    // Send the whole image. This will take sometime    
                    printer.SendRaw(image.GetAsEscBuffer());                        
                    printer.FormFeed();

                    // Wait for next capture period
                    Thread.Sleep(captureRate * 1000);
                }                      
            }
        }
    }
}
