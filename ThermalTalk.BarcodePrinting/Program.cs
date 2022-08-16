using QRCoder;
using ThermalTalk.Imaging;

namespace ThermalTalk.BarcodePrinting
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Sample 500 character string to encode
            const string stringToEncode = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo " +
                                          "ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis " +
                                          "dis parturient montes, nascetur ridiculus mus. Donec quam felis, " +
                                          "ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa " +
                                          "quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, " +
                                          "arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. " +
                                          "Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu";
            
            // Connect to printer - CHANGE THIS TO YOUR PRINTER'S COM PORT
            var printer = new PhoenixPrinter("COM26");
            
            // Use QRCoder library to generate QR code image
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(stringToEncode, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrData);
            var qrCodeImage = qrCode.GetGraphic(3);
            
            // set printer image
            var printerImage = new PrinterImage(qrCodeImage);
            var document = new StandardDocument();
            printer.SetImage(printerImage, document, 0);

            // Print sample image
            printer.PrintDocument(document);
            printer.FormFeed();
        }
    }
}