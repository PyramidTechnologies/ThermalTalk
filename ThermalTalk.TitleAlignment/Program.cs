using System;

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

            var dataTableTitleSection = new StandardSection()
            {
                Content = "DESCRIPTION".PadRight(9) + "ARCHIVE".PadLeft(7) + "WEEKLY".PadLeft(7) + "DAILY".PadLeft(7),
                Justification = FontJustification.JustifyLeft,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.None,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };

            document.Sections.Add(dataTableTitleSection);
            printer.PrintDocument(document);
            printer.FormFeed();
        }
    }
}