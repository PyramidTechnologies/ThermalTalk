using QRCoder;
using ThermalTalk.Imaging;

namespace ThermalTalk.BarcodePrinting
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            // Use index 1...5
            const int indexForQrCode = 1;            
            // Change for your printer:
            const string portName = "COM26";

            // Sample 500 character string to encode
            const string stringToEncode1 = "HUv0hBlbCPPFXJy3U5b5rI+RfvpiKH1IJ6vEsrGb4AvaAltdl5k3W9SlHYOVBmrQ;BwIAAACkAABSU0EygAEAAAEAAQD7SQxChCQ2J/y5HYjzeGtNjz00NmLC45ceEnhTf/EXbdvHWgNlB2X7skdDq9xnS7btNIqX8B1k2x2JwSUw/QrIOKVka51xVuWHxTA1yG5BA2NbdfJMaRZaKclxpefcfMs1KxHtldR76hfnyQCuyEMgvJC3mAp3d8/JiyFBqrbPqcnuApKAb85eJg6mEfuJishdJAiVQ3FjAiqbWpdhBFNz37+eeouNSwbJFgVeodP6qpUNAFwX+HKIjdmzA4o3sLfLDHDAMXduKLCrsb1mZ7TTa1++P7rSuVo=";

            // Connect to printer - CHANGE THIS TO YOUR PRINTER'S COM PORT
            var printer = new PhoenixPrinter(portName);

            var document = new StandardDocument
            {
                CodePage = CodePages.CPSPACE,
            };

            var headerSection = new StandardSection
            {
                Content = "REGISTER MACHINE",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.Bold, 
                Font = ThermalFonts.C,
                AutoNewline = true,
            };

            var storeIdSection = new StandardSection
            {
                Content = "# STORE: 1234",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Effects = FontEffects.Bold,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };
            
            var terminalIdSection = new StandardSection
            {
                Content = "# Terminal: 1234",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Effects = FontEffects.Bold,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };
            
            var dateSection = new StandardSection
            {
                Content = "Date: 1234",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Effects = FontEffects.Bold,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };
            
            var timeSection = new StandardSection
            {
                Content = "Time: 1234",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Effects = FontEffects.Bold,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };

            // add text
            document.Sections.Add(headerSection);
            document.Sections.Add(storeIdSection);
            document.Sections.Add(terminalIdSection);
            document.Sections.Add(dateSection);
            document.Sections.Add(timeSection);

            // Use QRCoder library to generate QR code image
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(stringToEncode1, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrData);
            var qrCodeImage = qrCode.GetGraphic(3);

            // set printer image
            var printerImage = new PrinterImage(qrCodeImage);
            printer.SetImage(printerImage, document, indexForQrCode);

            // Print sample image
            printer.PrintDocument(document);
            printer.FormFeed();
        }
    }
}