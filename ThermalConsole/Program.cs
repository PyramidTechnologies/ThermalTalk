#region Copyright & License
/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion
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
            const string phoenixPort = "COM1";
            const string reliancePort = "COM4";            

            const int captureRate = 10; // number of seconds between capture
            
            Console.WriteLine("Starting Security Camera Sample");


            // Setup the header with double width, double height, center justified
            var header = new StandardSection()
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };



            // Setup timestamp at normal scalar with bold, underline, and centered
            var timestamp = new StandardSection()
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.Italic | FontEffects.Underline,
                Font = ThermalFonts.B,
                AutoNewline = true,
            };

            // Print Status is shown as a JSON object
            var printStatus = new StandardSection()
            {
                Justification = FontJustification.JustifyLeft,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Font = ThermalFonts.C,
                AutoNewline = true,
            };

            // Document template
            // Capture #{}
            // Image....
            // Image....
            // Image...
            // Timestamp
            var document = new StandardDocument();
            document.Sections.Add(header);
            document.Sections.Add(new Placeholder());  // Placeholder since we know we'll want an image here
            document.Sections.Add(timestamp);
            document.Sections.Add(printStatus);

            int count = 1;
            while (true)
            {

                // Select one printer or the other. Phoenix does not currently 
                // support dynamic images over ESC/POS. Images will only 
                // be transmitted through the print queue but no examples have
                // been prepared for this.
                //using (var printer = new PhoenixPrinter(phoenixPort))
                using (var printer = new ReliancePrinter(reliancePort))
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

                    // Get the latest printer status. Note that reliance and phoenix have
                    // slightly different args to this get status command
                    printStatus.Content = printer.GetStatus(StatusTypes.FullStatus).ToJSON(true);

                    // Re-assign this image to the middle part of the document
                    printer.SetImage(image, document, 1);

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
