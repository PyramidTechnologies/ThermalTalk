
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


            // Setup the header with double width, double height, center justified
            var header = new StandardSection()
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                AutoNewline = true,
            };



            // Setup timestamp at normal scalar with bold, underline, and centered
            var timestamp = new StandardSection()
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.Bold | FontEffects.Underline,
                AutoNewline = true,
            };

            var document = new StandardDocument();
            document.Sections.Add(header);
            document.Sections.Add(new ImageSection());  // Placehold since we know we'll want an image here
            document.Sections.Add(timestamp);

            int count = 1;
            while (true)
            {
                using (var printer = new ReliancePrinter(commport))
                using(var image = Webcam.GrabPicture())
                {
             
                    var now = DateTime.Now;
                    Console.WriteLine("Image #{0} taken at {1}", count, now);


                    // Resize image if we want. This will follow ESC/POS justification
                    //logo.Resize(640, 480);
                    image.ApplyDithering(Algorithms.JarvisJudiceNinke, 128);

                    // Print the header document, update with new capture number
                    header.Content = string.Format("Capture #{0}", count);

                    // Printer the timestamp document
                    timestamp.Content = string.Format("{1}", count++, now);

                    document.Sections[1] = new ImageSection()
                        {
                            Image = image,
                        };

                    // Send the whole document + image
                    printer.PrintDocument(document);                        
                    printer.FormFeed();

                    // Wait for next capture period
                    Thread.Sleep(captureRate * 1000);
                }                      
            }
        }
    }
}
