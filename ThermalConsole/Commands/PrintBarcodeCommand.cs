using QRCoder;
using SkiaSharp;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ThermalTalk;
using ThermalTalk.Imaging;

namespace ThermalConsole.Commands
{
    public class PrintBarcodeCommandSettings : BaseCommandSettings
    {
        [CommandArgument(2, "<text>")]
        [Description("Text to encode into barcode.")]
        public string? Text { get; set; }
    }

    public class PrintBarcodeCommand : BaseCommand<PrintBarcodeCommandSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] PrintBarcodeCommandSettings settings)
        {
            var document = new StandardDocument
            {
                CodePage = CodePages.CPSPACE,
            };

            var headerSection = new StandardSection
            {
                Content = "Header",
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Effects = FontEffects.Bold,
                Font = ThermalFonts.A,
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

            // Add text.
            document.Sections.Add(headerSection);

            // Use QRCoder library to generate QR code image.
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(settings.Text!, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrData);
            var qrBmpBytes = qrCode.GetGraphic(3);
            using var qrCodeBitmap = SKBitmap.Decode(qrBmpBytes);

            // Set printer image.
            using var printerImage = new PrinterImage(qrCodeBitmap);
            Printer!.SetImage(printerImage, document, 1);
            document.Sections.Add(storeIdSection);

            // Print sample image.
            Printer!.PrintDocument(document);
            Printer!.FormFeed();

            AnsiConsole.MarkupLine("[green]Barcode has been printed.[/]");

            return 0;
        }
    }
}