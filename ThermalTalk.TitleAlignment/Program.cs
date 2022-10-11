using System;
using System.Text;

namespace ThermalTalk.TitleAlignment
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // Change to your port!
            var port = "COM26";

            var printer = new PhoenixPrinter(port);
            var document = new StandardDocument
            {
                CodePage = CodePages.CPSPACE,
            };

            var firstPadding = 11;
            var secondPadding = 10;
            var thirdPadding = 10;
            var fourthPadding = 11;
            
            var dataTableTitleSection = new StandardSection
            {
                Content = "DESCRIPTION".PadRight(firstPadding) + 
                          "ARCHIVE".PadLeft(secondPadding) + 
                          "WEEKLY".PadLeft(thirdPadding) + 
                          "DAILY".PadLeft(fourthPadding),
                Justification = FontJustification.JustifyLeft,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.None,
                Font = ThermalFonts.B,
                AutoNewline = true,
            };
            document.Sections.Add(dataTableTitleSection);

            var date = DateTime.Now;
            var lastClearDate = new StandardSection
            {
                Content = "LSTCLRDATE ".PadRight(firstPadding) 
                          + date.ToString("dd-MM-yyyy").PadLeft(secondPadding) 
                          + date.ToString("dd-MM-yyyy").PadLeft(thirdPadding) 
                          + date.ToString("dd-MM-yyyy").PadLeft(fourthPadding),
                AutoNewline = true,
                Font = ThermalFonts.B,
            };
            document.Sections.Add(lastClearDate);
            
            var lastClearTime = new StandardSection
            {
                Content = "LSTCLRDATE ".PadRight(firstPadding) 
                          + date.ToString("HH:mm:ss").PadLeft(secondPadding) 
                          + date.ToString("HH:mm:ss").PadLeft(thirdPadding) 
                          + date.ToString("HH:mm:ss").PadLeft(fourthPadding),
                AutoNewline = true,
                Font = ThermalFonts.B,
            };
            document.Sections.Add(lastClearTime);
            
            printer.PrintDocument(document);
            printer.PrintASCIIString("\n\n\n\n");
            
            // form feed to print ticket
            printer.FormFeed();
        }
    }
}