using Emgu.CV;
using SkiaSharp;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Diagnostics.CodeAnalysis;
using ThermalTalk;
using ThermalTalk.Imaging;

namespace ThermalConsole.Commands
{
    public class CaptureCommand : BaseCommand<BaseCommandSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BaseCommandSettings settings)
        {
            // Setup the header with double width, double height, center justified.
            var header = new StandardSection
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };

            // Setup timestamp at normal scalar with bold, underline, and centered.
            var timestamp = new StandardSection
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.Italic | FontEffects.Underline,
                Font = ThermalFonts.B,
                AutoNewline = true,
            };

            // Print Status is shown as a JSON object.
            var printStatus = new StandardSection
            {
                Justification = FontJustification.JustifyLeft,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Font = ThermalFonts.C,
                AutoNewline = true,
            };

            // Document Template:
            // Capture #{}
            // Image....
            // Image....
            // Image...
            // Timestamp
            var document = new StandardDocument
            {
                // Don't forget to set your codepage!
                CodePage = CodePages.CPSPACE,
            };

            document.Sections.Add(header);
            document.Sections.Add(new Placeholder()); // Placeholder since we know we'll want an image here.
            document.Sections.Add(new Placeholder()); // Placeholder since we know we'll want a barcode here.
            document.Sections.Add(timestamp);
            document.Sections.Add(printStatus);
        
            PrinterImage image;
            using (var capture = new VideoCapture())
            using (var frame = capture.QueryFrame())
            {
                if (frame == null)
                {
                    AnsiConsole.MarkupLine("[red]An image could not be captured from the webcam.[/]");
                    return 1;
                }

                var frameBuffer = frame.GetRawData();
                var bitmapBuffer = new byte[frame.Width * frame.Height * 4];

                var frameIndex = 0;
                for (var i = 0; i < bitmapBuffer.Length; i++)
                {
                    if (i % 4 == 3)
                    {
                        bitmapBuffer[i] = 255;
                        continue;
                    }

                    bitmapBuffer[i] = frameBuffer[frameIndex];
                    frameIndex++;
                }

                using var bitmap =
                    bitmapBuffer.ToBitmap(frame.Width, frame.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                image = new PrinterImage(bitmap);
            }

            var now = DateTime.Now;
            AnsiConsole.MarkupLine("[green]Image has been taken.[/]");

            // Resize image if desired. This will follow ESC/POS justification.
            //image.Resize(640, 480);
            image.ApplyDithering(Algorithms.JarvisJudiceNinke, 128);

            // Print the header document.
            header.Content = "Webcam Capture";

            // Print the timestamp document.
            timestamp.Content = $"{now}";

            // Get the latest printer status.
            // Note that reliance and phoenix have slightly different args to this get status command.
            printStatus.Content = Printer!.GetStatus(StatusTypes.FullStatus).ToJSON(true);

            // Re-assign this image to the middle part of the document.
            // Phoenix does not currently support dynamic images over ESC/POS.
            if (Printer is ReliancePrinter)
                Printer!.SetImage(image, document, 1);

            // Update barcode.
            var barcode = Printer is ReliancePrinter
                ? new TwoDBarcode(TwoDBarcode.Flavor.Reliance)
                : new TwoDBarcode(TwoDBarcode.Flavor.Phoenix);
            barcode.EncodeThis = $"Capture occurred at {now}.";
            Printer!.SetBarcode(barcode, document, 2);

            // Send the whole document + image.
            Printer!.PrintDocument(document);
            Printer!.FormFeed();

            image.Dispose();
            return 0;
        }
    }
}